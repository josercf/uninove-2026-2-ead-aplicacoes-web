# Tópico 05: Frameworks front-end modernos (React)

Este exemplo reescreve em React o filtro da agenda do tópico 04. A página, os dados, as opções do filtro e a mensagem de contagem são os mesmos: com Cardiologia, a tabela mostra Bruno Lima e Carla Mendes e o texto `2 consulta(s) encontrada(s).`.

O que muda é quem atualiza a tela. No tópico 04, a função `desenhar` do `main.js` cria e troca as linhas da tabela com chamadas ao DOM. Aqui, o componente `App` guarda a especialidade escolhida com `useState`, e o React redesenha a agenda sempre que esse valor muda.

## Arquivos

- `index.html`, `estilos.css` e `logo.svg`: a página do tópico 04, com a seção `#agenda-do-dia` vazia. O React desenha o conteúdo dessa seção.
- `src/main.jsx`: ponto de entrada. Liga o React à seção `#agenda-do-dia` e desenha o `App` dentro dela.
- `src/App.jsx`: componente raiz. Guarda a especialidade escolhida no estado e entrega os dados aos dois componentes filhos por props.
- `src/FiltroEspecialidade.jsx`: o rótulo e o campo de seleção. Mostra o valor recebido e avisa a troca pela prop `aoMudar`.
- `src/TabelaConsultas.jsx`: a tabela da agenda. Desenha uma linha por consulta recebida na prop `consultas`.
- `src/agenda.js` e `src/dados.js`: cópias exatas dos arquivos do tópico 04. Um teste confere, a cada execução, que continuam iguais.
- `src/__tests__/`: oito testes com Vitest e Testing Library, que rodam no Node com uma página simulada pelo jsdom.
- `vite.config.js`: liga o plugin do React ao Vite e escolhe o jsdom como ambiente dos testes.
- `package.json` e `package-lock.json`: versões exatas das dependências, para que `npm ci` instale sempre o mesmo conjunto.

Os arquivos `.jsx` trazem um comentário em português em cada linha relevante. A escolha do Vitest está registrada em `docs/adrs/ADR-003-vitest-para-componentes-react.md`.

## Pré-requisitos

1. Node.js 22.12 ou superior e npm. O Vite 8 e o jsdom 30 não instalam em versões anteriores.

   ```bash
   node --version
   ```

   Resultado esperado: um número igual ou maior que `v22.12.0`.

2. Um navegador atualizado (Chrome, Edge ou Firefox).

## Como executar

### Passo 1: Instalar as dependências

No terminal, a partir da pasta raiz do repositório:

```bash
cd topico-05-react
npm ci
```

Resultado esperado: a saída mostra `added 92 packages` e termina com `found 0 vulnerabilities`.

### Passo 2: Rodar os testes

```bash
npm test
```

Resultado esperado: oito linhas com `✓`, seguidas de `Test Files  3 passed (3)` e `Tests  8 passed (8)`.

### Passo 3: Subir o servidor de desenvolvimento

```bash
npm run dev
```

Resultado esperado: o Vite mostra `Local:   http://localhost:5173/`. Deixe esse terminal aberto enquanto usa a página.

### Passo 4: Usar o filtro

Abra `http://localhost:5173/` no navegador e escolha Cardiologia no campo "Especialidade".

Resultado esperado: a tabela mostra 2 linhas, de Bruno Lima e Carla Mendes, e o texto `2 consulta(s) encontrada(s).`. Com Dermatologia, a tabela fica sem linhas e o texto mostra `0 consulta(s) encontrada(s).`. A tela é a mesma do tópico 04.

Para parar o servidor, volte ao terminal do Passo 3 e aperte `Ctrl+C`.

### Passo 5: Gerar a versão de publicação

```bash
npm run build
```

Resultado esperado: a pasta `dist` aparece com `index.html`, um arquivo `.css` e um arquivo `.js` de cerca de 221 kB, e a última linha mostra `built in` seguido de um tempo.

## Se der errado

### Erro 1: a seção "Agenda do dia" aparece vazia

Isso acontece quando o `index.html` é aberto com duplo clique, por um endereço que começa por `file://`. O navegador não lê JSX e bloqueia o `main.jsx`; o console do DevTools mostra `Access to script at 'file:///src/main.jsx' from origin 'null' has been blocked by CORS policy`.

Correção: rode `npm run dev` na pasta `topico-05-react` e abra o endereço `http://localhost:5173/` que o Vite mostra.

### Erro 2: `npm ci` mostra `npm warn EBADENGINE`, e o `npm test` ou o `npm run dev` falha

Isso significa que o Node instalado é anterior ao 22.12.

Correção: instale o Node 22 (versão LTS) ou superior, confira com `node --version` e repita o Passo 1.

### Erro 3: o teste "cópia exata do agenda.js" falha

Isso significa que o `src/agenda.js` deste tópico ou o `agenda.js` do tópico 04 foi alterado. Os dois precisam continuar iguais, porque o tópico 05 reaproveita a função de filtro sem mudança.

Correção: desfaça a alteração com `git restore src/agenda.js ../topico-04-javascript/agenda.js` e rode `npm test` de novo.

### Erro 4: a porta 5173 está ocupada

O Vite avisa `Port 5173 is in use, trying another one...` e sobe em outra porta, como 5174.

Correção: abra o endereço que o Vite mostrar na linha `Local:`, ou encerre o outro servidor com `Ctrl+C` no terminal dele.

## Experimente mudar

Faça cada mudança separadamente, com o `npm run dev` no ar e o console do DevTools aberto, e desfaça antes da próxima.

### Mudança 1: tirar o `onChange` do campo

No `src/FiltroEspecialidade.jsx`, apague o trecho `onChange={(evento) => aoMudar(evento.target.value)}` do `select` e salve.

Pergunta: o que acontece com o campo e com a tabela quando você escolhe Cardiologia? Que mensagem aparece no console?

### Mudança 2: tirar a `key` das linhas

No `src/TabelaConsultas.jsx`, troque `<tr key={consulta.id}>` por `<tr>` e salve.

Pergunta: a tabela continua funcionando? Que aviso o React escreve no console, e qual componente ele aponta?

### Mudança 3: acrescentar uma consulta

No `src/dados.js`, acrescente `{ id: 4, paciente: 'Diego Rocha', especialidade: 'Dermatologia', dataHora: '2026-10-07T08:15:00' },` antes do `];`. Salve e rode `npm test`.

Pergunta: quais testes falham, e por quê? Quantas linhas aparecem na página com o filtro em Dermatologia?
