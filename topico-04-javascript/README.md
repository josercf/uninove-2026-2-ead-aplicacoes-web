# Tópico 04: Introdução ao JavaScript

Este exemplo parte da página do tópico 03 e acrescenta um filtro por especialidade à agenda do dia. A tabela começa vazia no HTML, e o JavaScript cria as linhas a partir de uma lista de consultas. Ao trocar a especialidade no campo de seleção, a tabela e o total de consultas são redesenhados.

A página usa módulos JavaScript (`import` e `export`), que o navegador só carrega a partir de um servidor. Por isso o tópico usa o Vite como servidor de desenvolvimento.

## Arquivos

- `index.html`, `estilos.css` e `logo.svg`: a página do tópico 03, com o campo de seleção `#filtro`, o parágrafo `#total` e o `tbody` da agenda vazio.
- `dados.js`: a lista fixa de três consultas. No tópico de APIs, ela será trocada por uma chamada ao servidor.
- `agenda.js`: duas funções puras, `filtrarPorEspecialidade` e `formatarDataHora`, que recebem dados e devolvem dados sem tocar na página.
- `main.js`: liga as funções à página. Lê o filtro, cria as linhas da tabela e escreve o total.
- `testes/agenda.test.js`: seis testes das duas funções puras, que rodam no Node sem navegador.
- `package.json`: declara o Vite como dependência de desenvolvimento e os comandos `npm run dev` e `npm test`.

Os três arquivos `.js` trazem um comentário em português em cada linha relevante.

## Pré-requisitos

1. Node.js 18 ou superior e npm.

   ```bash
   node --version
   ```

   Resultado esperado: um número igual ou maior que `v18.0.0`.

2. Um navegador atualizado (Chrome, Edge ou Firefox).

## Como executar

### Passo 1: Instalar as dependências

No terminal, a partir da pasta raiz do repositório:

```bash
cd topico-04-javascript
npm install
```

Resultado esperado: a pasta `node_modules` aparece dentro de `topico-04-javascript`, e a saída termina com `found 0 vulnerabilities`.

### Passo 2: Rodar os testes

```bash
npm test
```

Resultado esperado: seis linhas com `✔`, seguidas de `ℹ pass 6` e `ℹ fail 0`.

### Passo 3: Subir o servidor de desenvolvimento

```bash
npm run dev
```

Resultado esperado: o Vite mostra `Local:   http://localhost:5173/`. Deixe esse terminal aberto enquanto usa a página.

### Passo 4: Abrir a página

Abra `http://localhost:5173/` no navegador.

Resultado esperado: a agenda do dia mostra 3 consultas, com a data e a hora no formato `05/10/2026 09:00`, e o texto `3 consulta(s) encontrada(s).` acima da tabela.

### Passo 5: Usar o filtro

No campo "Especialidade", escolha Cardiologia e depois Dermatologia.

Resultado esperado: com Cardiologia, a tabela mostra 2 linhas, de Bruno Lima e Carla Mendes, e o texto `2 consulta(s) encontrada(s).`. Com Dermatologia, a tabela fica sem linhas, e o texto mostra `0 consulta(s) encontrada(s).`.

Para parar o servidor, volte ao terminal do Passo 3 e aperte `Ctrl+C`.

## Se der errado

### Erro 1: a tabela aparece vazia e o texto do total não aparece

Isso acontece quando o `index.html` é aberto com duplo clique, por um endereço que começa por `file://`. Módulos JavaScript exigem servidor: o navegador bloqueia o `main.js`, e o console do DevTools mostra `Access to script at 'file:///.../main.js' from origin 'null' has been blocked by CORS policy`.

Correção: rode `npm run dev` na pasta `topico-04-javascript` e abra o endereço `http://localhost:5173/` que o Vite mostra.

### Erro 2: `npm run dev` responde "vite: command not found" ou "'vite' não é reconhecido"

Isso significa que as dependências não foram instaladas nesta pasta.

Correção: rode `npm install` dentro de `topico-04-javascript` e repita `npm run dev`.

### Erro 3: `npm test` responde "Cannot find module" apontando para `agenda.js`

Isso significa que o `agenda.js` foi apagado, renomeado ou movido para outra pasta. O teste importa o arquivo pelo caminho `../agenda.js`, a partir da pasta `testes`.

Correção: deixe o `agenda.js` na pasta `topico-04-javascript`, ao lado do `index.html`, e rode `npm test` de novo.

### Erro 4: a porta 5173 está ocupada

O Vite avisa `Port 5173 is in use, trying another one...` e sobe em outra porta, como 5174.

Correção: abra o endereço que o Vite mostrar na linha `Local:`, ou encerre o outro servidor com `Ctrl+C` no terminal dele.

## Experimente mudar

Faça cada mudança separadamente e desfaça antes da próxima.

### Mudança 1: acrescentar uma consulta

No `dados.js`, acrescente `{ id: 4, paciente: 'Diego Rocha', especialidade: 'Dermatologia', dataHora: '2026-10-07T08:15:00' },` antes do `];`. Salve e olhe a página.

Pergunta: quantas consultas aparecem com o filtro em Todas e com o filtro em Dermatologia? Você precisou recarregar a página?

### Mudança 2: uma data em outro formato

Troque a `dataHora` da Ana Souza por `'05/10/2026 09:00'` e salve.

Pergunta: o que aparece na tabela? Abra o console do DevTools (`F12`, aba Console) e leia a mensagem de erro.

### Mudança 3: quebrar um teste de propósito

No `agenda.js`, troque `` `${dia}/${mes}/${ano} ${hora}:${minuto}` `` por `` `${mes}/${dia}/${ano} ${hora}:${minuto}` `` e rode `npm test`.

Pergunta: qual teste falha, e quais valores aparecem em `expected` e `actual`?
