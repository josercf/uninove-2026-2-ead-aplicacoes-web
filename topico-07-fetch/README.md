# Tópico 07: APIs e integração de dados (fetch)

Este exemplo liga o front React do tópico 05 à API REST do tópico 06. O arquivo `dados.js`, que guardava as três consultas dentro do front, deixa de existir. O componente `App` busca as consultas com `fetch` em `http://localhost:5084/api/consultas`.

Os componentes `FiltroEspecialidade` e `TabelaConsultas` continuam iguais aos do tópico 05. O `App` ganhou dois estados a mais, carregamento e erro, porque a resposta de uma API leva tempo para chegar e pode não chegar.

## Arquivos

- `src/api.js`: a função `buscarConsultas`, que chama `fetch`, confere o status e converte o corpo em JSON.
- `src/carregar.js`: a função `carregarConsultas`, que entrega ao componente os estados `carregando`, `pronto` e `erro`.
- `src/App.jsx`: chama a busca num `useEffect`, desenha o aviso de carregamento, a mensagem de erro ou a agenda.
- `src/FiltroEspecialidade.jsx`, `src/TabelaConsultas.jsx`, `src/agenda.js`, `src/main.jsx`, `index.html`, `estilos.css`: cópias do tópico 05, sem mudança.
- `tests/api.test.js`: sete testes com `node --test` e um servidor HTTP falso, sem rede real. Cobrem sucesso, tempo de resposta, carregamento e três tipos de erro.
- `src/__tests__/`: dez testes com Vitest e Testing Library, com o `fetch` trocado por uma função falsa.
- `api/`: cópia da API do tópico 06 com CORS liberado para `http://localhost:5173`. A escolha está em `docs/adrs/ADR-005-api-do-topico-07-como-copia-com-cors.md`.
- `ClinicaVida.sln` e `requisicoes/`: a solução e os corpos de exemplo da API, copiados do tópico 06.

## Pré-requisitos

1. Node.js 22.12 ou superior e npm.

   ```bash
   node --version
   ```

   Resultado esperado: um número igual ou maior que `v22.12.0`.

2. .NET SDK 10.0 ou superior.

   ```bash
   dotnet --version
   ```

   Resultado esperado: um número que começa por `10.`.

## Como executar

### Passo 1: Testar a API

A partir da pasta raiz do repositório:

```bash
dotnet test topico-07-fetch/ClinicaVida.sln
```

Resultado esperado: `Passed!  - Failed:     0, Passed:    16`. Quatro desses testes, os da classe `CorsTests`, conferem o cabeçalho `Access-Control-Allow-Origin`.

### Passo 2: Testar o front

```bash
cd topico-07-fetch
npm ci
npm test
```

Resultado esperado: sete testes com `✔` do `node --test`, com `ℹ pass 7`, e em seguida `Tests  10 passed (10)` do Vitest.

### Passo 3: Subir a API

Em um terminal, a partir da pasta raiz do repositório:

```bash
cd topico-07-fetch/api/ClinicaVida.Api
dotnet run
```

Resultado esperado: a linha `Now listening on: http://localhost:5084`. Deixe esse terminal aberto.

### Passo 4: Conferir o cabeçalho CORS

Em outro terminal:

```bash
curl -i -H "Origin: http://localhost:5173" http://localhost:5084/api/consultas
```

Resultado esperado: `HTTP/1.1 200 OK` e, entre os cabeçalhos, `Access-Control-Allow-Origin: http://localhost:5173`. Com `-H "Origin: http://localhost:9999"` a resposta continua 200, sem esse cabeçalho.

### Passo 5: Subir o front

Em um terceiro terminal:

```bash
cd topico-07-fetch
npm run dev
```

Abra `http://localhost:5173/` no navegador.

Resultado esperado: a tabela mostra Ana Souza, Bruno Lima e Carla Mendes e o texto `3 consulta(s) encontrada(s).`. Com Cardiologia, ficam 2 linhas.

### Passo 6: Ver o estado de erro

Volte ao terminal do Passo 3 e aperte `Ctrl+C`. Recarregue a página do front.

Resultado esperado: a tela mostra `Não foi possível carregar a agenda (Failed to fetch).` no lugar da tabela. No console do DevTools aparece a requisição que falhou.

## Se der errado

### Erro 1: a tela mostra `Failed to fetch` com a API no ar

Se o console do DevTools escreve `has been blocked by CORS policy`, o front está numa porta diferente de 5173 e a API não o libera.

Correção: suba o front com `npm run dev` e use o endereço `http://localhost:5173/`, ou ajuste a origem em `api/ClinicaVida.Api/Program.cs`.

### Erro 2: a API não sobe e escreve `address already in use`

Outro programa usa a porta 5084, como a API do tópico 06 ainda aberta.

Correção: encerre o outro programa com `Ctrl+C` e rode `dotnet run` de novo.

### Erro 3: `npm test` falha em `tests/api.test.js` com `ERR_MODULE_NOT_FOUND`

O comando foi rodado fora da pasta `topico-07-fetch`.

Correção: entre na pasta com `cd topico-07-fetch` e repita o comando.

## Experimente mudar

### Mudança 1: atrasar a resposta

No `api/ClinicaVida.Api/Controllers/ConsultasController.cs`, acrescente `await Task.Delay(2000);` no início do método `Listar`, tornando-o `async`. Recarregue o front.

Pergunta: por quantos segundos a tela mostra `Carregando a agenda...`, e o que o DevTools mostra na aba Network?

### Mudança 2: trocar a origem liberada

No `api/ClinicaVida.Api/Program.cs`, troque `http://localhost:5173` por `http://localhost:5174` e reinicie a API.

Pergunta: o que a tela mostra, que código de status o `curl` recebe e qual cabeçalho some da resposta?
