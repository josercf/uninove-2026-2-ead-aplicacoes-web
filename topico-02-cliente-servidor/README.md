# Topico 02: Cliente-servidor

Este exemplo mostra a arquitetura cliente-servidor na pratica: uma API ASP.NET Core que guarda a agenda da Clinica Vida+ em memoria, e um front React que busca essa agenda pela rede e mostra o resultado em uma tabela.

## Pre-requisitos

Antes de comecar, confira se voce tem o que precisa.

1. .NET SDK instalado.

   Comando para confirmar:

   ```bash
   dotnet --version
   ```

   Resultado esperado: um numero igual ou maior que `10.0.0`. Se aparecer "comando nao encontrado", veja a secao "Se der errado" mais abaixo.

2. Node.js e npm instalados.

   Comando para confirmar:

   ```bash
   node --version
   npm --version
   ```

   Resultado esperado: o `node` responde com um numero igual ou maior que `18.0.0`, e o `npm` responde com qualquer numero de versao.

3. O repositorio clonado na sua maquina.

   Comando para confirmar (rode dentro da pasta do repositorio):

   ```bash
   ls
   ```

   Resultado esperado: a lista de arquivos mostra a pasta `topico-02-cliente-servidor`.

## Como executar

Siga os passos na ordem. Rode cada comando a partir da pasta raiz do repositorio (a pasta chamada `uninove-2026-2-ead-aplicacoes-web`), a nao ser que o passo diga outra coisa.

### Passo 1: Restaurar as dependencias da API

```bash
dotnet restore topico-02-cliente-servidor/ClinicaVida.sln
```

Resultado esperado: o terminal mostra mensagens de "Restaurado" (ou "Restored") e nao mostra nenhuma linha de erro.

### Passo 2: Rodar os testes automatizados da API

```bash
dotnet test topico-02-cliente-servidor/ClinicaVida.sln
```

Resultado esperado: a ultima linha mostra `Passed! - Failed: 0, Passed: 3, Skipped: 0, Total: 3`.

### Passo 3: Subir a API

```bash
dotnet run --project topico-02-cliente-servidor/api/ClinicaVida.Api
```

Resultado esperado: o terminal mostra a linha `Now listening on: http://localhost:5084`. Deixe este terminal aberto e rodando; a API precisa continuar de pe para o front conseguir conversar com ela. Se a sua maquina imprimir uma porta diferente de `5084`, anote o numero real e use-o no lugar de `5084` em todos os passos seguintes.

### Passo 4: Conferir a API isolada, sem o front

Em um novo terminal, com a API do Passo 3 ainda rodando:

```bash
curl -s http://localhost:5084/api/consultas
```

Resultado esperado: uma linha de JSON com duas consultas, cada uma com `id`, `paciente`, `especialidade` e `dataHora`. A secao "Saida esperada" mostra o texto completo.

### Passo 5: Instalar as dependencias do front

Em um novo terminal (deixe os dois anteriores abertos):

```bash
cd topico-02-cliente-servidor/web
npm install
```

Resultado esperado: o terminal termina com uma linha do tipo `added N packages` e nao mostra `npm error`.

### Passo 6: Subir o front

Ainda dentro da pasta `topico-02-cliente-servidor/web`:

```bash
npm run dev
```

Resultado esperado: o terminal mostra `VITE ready` e um endereco, normalmente `http://localhost:5173/`.

### Passo 7: Ver a tabela no navegador

Abra o endereco do Passo 6 no navegador.

Resultado esperado: a pagina mostra o titulo "Agenda da Clinica Vida+" e, logo abaixo, uma tabela com duas linhas: Ana Souza (Clinica geral) e Bruno Lima (Cardiologia).

### Passo 8: Ver a requisicao na aba Network

Com a pagina do Passo 7 aberta, abra o DevTools do navegador (tecla `F12` ou `Ctrl+Shift+I`, no macOS `Cmd+Option+I`), clique na aba "Network" e recarregue a pagina.

Resultado esperado: aparece uma linha chamada `consultas` com status `200`. Clicando nela, a aba "Response" (ou "Preview") mostra o JSON cru devolvido pela API, o mesmo conteudo que o Passo 4 mostrou pelo terminal.

## Saida esperada

Esta e a saida exata do `curl -s http://localhost:5084/api/consultas` do Passo 4. Compare com o que aparece na sua tela; apenas a porta pode mudar, se a sua maquina usar uma porta diferente de `5084`.

```json
[{"id":1,"paciente":"Ana Souza","especialidade":"Clinica geral","dataHora":"2026-10-05T09:00:00"},{"id":2,"paciente":"Bruno Lima","especialidade":"Cardiologia","dataHora":"2026-10-05T10:30:00"}]
```

Na tabela do navegador (Passo 7), a mesma informacao aparece formatada, uma consulta por linha.

## Se der errado

### Erro 1: "dotnet: comando nao encontrado" ou "'dotnet' nao e reconhecido"

Isso significa que o SDK do .NET nao esta instalado ou nao esta no PATH do terminal.

Correcao no Windows:

```powershell
winget install Microsoft.DotNet.SDK.10
```

Correcao no macOS:

```bash
brew install --cask dotnet-sdk
```

Depois de instalar, feche o terminal, abra um novo e rode `dotnet --version` de novo para confirmar.

### Erro 2: a tabela nao aparece e o navegador mostra "Nao foi possivel carregar a agenda"

Isso significa que o front nao conseguiu conversar com a API. A causa mais comum e a API do Passo 3 nao estar rodando, ou estar rodando em uma porta diferente da que o `vite.config.js` espera.

Correcao: confira se o terminal do Passo 3 ainda mostra a API rodando. Se a porta impressa for diferente de `5084`, abra `topico-02-cliente-servidor/web/vite.config.js` e troque `5084` pela porta real, depois pare o front com `Ctrl+C` e rode `npm run dev` de novo.

### Erro 3: "Could not find project or directory" ou caminho nao encontrado

Isso significa que o comando foi rodado de dentro da pasta errada.

Correcao: volte para a pasta raiz do repositorio antes de rodar o comando.

```bash
cd caminho/para/uninove-2026-2-ead-aplicacoes-web
dotnet run --project topico-02-cliente-servidor/api/ClinicaVida.Api
```

Troque `caminho/para/` pelo caminho real onde voce clonou o repositorio na sua maquina.

## Experimente mudar

Faca cada mudanca separadamente. Depois de cada uma, repita os passos 3 a 8 e compare com a sua resposta.

### Mudanca 1: acrescentar uma terceira consulta

No arquivo `topico-02-cliente-servidor/api/ClinicaVida.Api/Controllers/ConsultasController.cs`, dentro da lista `Agenda`, adicione um terceiro item:

```csharp
new() { Id = 3, Paciente = "Carla Reis", Especialidade = "Dermatologia",
        DataHora = new DateTime(2026, 10, 5, 14, 0, 0) },
```

Pergunta: depois de salvar e reiniciar a API, quantas linhas a tabela do navegador mostra? Foi preciso reiniciar o front tambem, ou so a API?

### Mudanca 2: pedir uma consulta que nao existe

No navegador, abra `http://localhost:5084/api/consultas/9999` diretamente.

Pergunta: qual status HTTP aparece na aba Network? Compare com o status da consulta que existe, no Passo 8, e explique com suas palavras por que os dois numeros sao diferentes.

### Mudanca 3: derrubar a API de proposito

Com o front do Passo 6 ainda aberto no navegador, va ate o terminal da API (Passo 3) e aperte `Ctrl+C` para encerra-la. Depois, recarregue a pagina do front no navegador.

Pergunta: o que a pagina mostra agora, a tabela antiga, uma tabela vazia ou uma mensagem de erro? Isso confirma que o front guarda os dados, ou que ele depende da API estar de pe a cada carregamento?
