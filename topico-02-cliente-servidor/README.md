# Tópico 02: Cliente-servidor

Este exemplo mostra a arquitetura cliente-servidor na prática. Uma API ASP.NET Core guarda a agenda da Clínica Vida+ em memória, e um front React busca essa agenda pela rede e mostra o resultado em uma tabela.

## Pré-requisitos

Antes de começar, confira se você tem o que precisa.

1. .NET SDK instalado.

   Comando para confirmar:

   ```bash
   dotnet --version
   ```

   Resultado esperado: um número igual ou maior que `10.0.0`. Se aparecer "comando não encontrado", veja a seção "Se der errado" mais abaixo.

2. Node.js e npm instalados.

   Comando para confirmar:

   ```bash
   node --version
   npm --version
   ```

   Resultado esperado: o `node` responde com um número igual ou maior que `18.0.0`, e o `npm` responde com qualquer número de versão.

3. O repositório clonado na sua máquina. Se ainda não clonou, siga a seção "Como começar" do `README.md` da raiz deste repositório.

   Comando para confirmar (rode dentro da pasta do repositório):

   ```bash
   ls
   ```

   Resultado esperado: a lista de arquivos mostra a pasta `topico-02-cliente-servidor`.

## Como executar

Siga os passos na ordem. Rode cada comando a partir da pasta raiz do repositório (a pasta chamada `uninove-2026-2-ead-aplicacoes-web`), a não ser que o passo diga outra coisa.

### Passo 1: Restaurar as dependências da API

```bash
dotnet restore topico-02-cliente-servidor/ClinicaVida.sln
```

Resultado esperado: o terminal mostra mensagens de "Restaurado" (ou "Restored") e não mostra nenhuma linha de erro.

### Passo 2: Rodar os testes automatizados da API

```bash
dotnet test topico-02-cliente-servidor/ClinicaVida.sln
```

Resultado esperado: a última linha mostra `Passed! - Failed: 0, Passed: 5, Skipped: 0, Total: 5`.

### Passo 3: Subir a API

```bash
dotnet run --project topico-02-cliente-servidor/api/ClinicaVida.Api
```

Resultado esperado: o terminal mostra a linha `Now listening on: http://localhost:5084`. Deixe este terminal aberto e rodando; a API precisa continuar de pé para o front conseguir conversar com ela. Se a sua máquina imprimir uma porta diferente de `5084`, anote o número real e use-o no lugar de `5084` em todos os passos seguintes.

### Passo 4: Conferir a API isolada, sem o front

Em um novo terminal, com a API do Passo 3 ainda rodando:

```bash
curl -s http://localhost:5084/api/consultas
```

Resultado esperado: uma linha de JSON com duas consultas, cada uma com `id`, `paciente`, `especialidade` e `dataHora`. A seção "Saída esperada" mostra o texto completo.

### Passo 5: Instalar as dependências do front

Em um novo terminal (deixe os dois anteriores abertos):

```bash
cd topico-02-cliente-servidor/web
npm install
```

Resultado esperado: o terminal termina com uma linha do tipo `added N packages` e não mostra `npm error`.

### Passo 6: Subir o front

Ainda dentro da pasta `topico-02-cliente-servidor/web`:

```bash
npm run dev
```

Resultado esperado: o terminal mostra `VITE ready` e um endereço, normalmente `http://localhost:5173/`.

### Passo 7: Ver a tabela no navegador

Abra o endereço do Passo 6 no navegador.

Resultado esperado: a página mostra o título "Agenda da Clínica Vida+" e, logo abaixo, uma tabela com duas linhas: Ana Souza (Clínica geral) e Bruno Lima (Cardiologia).

### Passo 8: Ver a requisição na aba Network

Com a página do Passo 7 aberta, abra o DevTools do navegador (tecla `F12` ou `Ctrl+Shift+I`, no macOS `Cmd+Option+I`), clique na aba "Network" e recarregue a página.

Resultado esperado: aparece uma linha chamada `consultas` com status `200`. Clicando nela, a aba "Response" (ou "Preview") mostra o JSON cru devolvido pela API, o mesmo conteúdo que o Passo 4 mostrou pelo terminal.

## Saída esperada

Esta é a saída exata do `curl -s http://localhost:5084/api/consultas` do Passo 4. Compare com o que aparece na sua tela; apenas a porta pode mudar, se a sua máquina usar uma porta diferente de `5084`.

```json
[{"id":1,"paciente":"Ana Souza","especialidade":"Clínica geral","dataHora":"2026-10-05T09:00:00"},{"id":2,"paciente":"Bruno Lima","especialidade":"Cardiologia","dataHora":"2026-10-05T10:30:00"}]
```

Na tabela do navegador (Passo 7), a mesma informação aparece formatada, uma consulta por linha.

## Se der errado

### Erro 1: "dotnet: comando não encontrado" ou "'dotnet' não é reconhecido"

Isso significa que o SDK do .NET não está instalado ou não está no PATH do terminal.

Correção no Windows:

```powershell
winget install Microsoft.DotNet.SDK.10
```

Correção no macOS:

```bash
brew install --cask dotnet-sdk
```

Depois de instalar, feche o terminal, abra um novo e rode `dotnet --version` de novo para confirmar.

### Erro 2: a tabela não aparece e o navegador mostra "Não foi possível carregar a agenda"

Isso significa que o front não conseguiu conversar com a API. A causa mais comum é a API do Passo 3 não estar rodando, ou estar rodando em uma porta diferente da que o `vite.config.js` espera.

Correção: confira se o terminal do Passo 3 ainda mostra a API rodando. Se a porta impressa for diferente de `5084`, abra `topico-02-cliente-servidor/web/vite.config.js` e troque `5084` pela porta real, depois pare o front com `Ctrl+C` e rode `npm run dev` de novo.

### Erro 3: "Could not find project or directory" ou caminho não encontrado

Isso significa que o comando foi rodado de dentro da pasta errada.

Correção: volte para a pasta raiz do repositório antes de rodar o comando.

```bash
cd caminho/para/uninove-2026-2-ead-aplicacoes-web
dotnet run --project topico-02-cliente-servidor/api/ClinicaVida.Api
```

Troque `caminho/para/` pelo caminho real onde você clonou o repositório na sua máquina.

### Erro 4: "Failed to bind to address http://127.0.0.1:5084: address already in use"

Isso significa que a porta `5084` já está ocupada, normalmente por uma instância anterior da própria API que ficou rodando em outro terminal.

Correção no macOS e no Linux: descubra qual processo está usando a porta e encerre-o.

```bash
lsof -i :5084
kill <PID>
```

Troque `<PID>` pelo número que aparecer na coluna `PID` do comando `lsof`. Depois, rode `dotnet run --project topico-02-cliente-servidor/api/ClinicaVida.Api` de novo.

Correção no Windows (PowerShell):

```powershell
netstat -ano | findstr :5084
taskkill /PID <PID> /F
```

### Erro 5: "sh: vite: command not found" (ou "'vite' não é reconhecido") ao rodar `npm run dev`

Isso significa que o Passo 5 (`npm install`) não foi rodado antes do Passo 6, então a pasta `node_modules` com o Vite não existe ainda.

Correção: volte ao Passo 5, dentro de `topico-02-cliente-servidor/web`, rode `npm install` e espere terminar sem `npm error`. Só depois rode `npm run dev` de novo.

## Experimente mudar

Faça cada mudança separadamente. Depois de cada uma, repita os passos 3 a 8 e compare com a sua resposta.

### Mudança 1: acrescentar uma terceira consulta

No arquivo `topico-02-cliente-servidor/api/ClinicaVida.Api/Controllers/ConsultasController.cs`, dentro da lista `Agenda`, adicione um terceiro item:

```csharp
new() { Id = 3, Paciente = "Carla Reis", Especialidade = "Dermatologia",
        DataHora = new DateTime(2026, 10, 5, 14, 0, 0) },
```

Pergunta: depois de salvar e reiniciar a API, quantas linhas a tabela do navegador mostra? Foi preciso reiniciar o front também, ou só a API?

### Mudança 2: pedir uma consulta que não existe

No navegador, abra `http://localhost:5084/api/consultas/9999` diretamente.

Pergunta: qual status HTTP aparece na aba Network? Compare com o status da consulta que existe, no Passo 8, e explique com suas palavras por que os dois números são diferentes.

### Mudança 3: derrubar a API de propósito

Com o front do Passo 6 ainda aberto no navegador, vá até o terminal da API (Passo 3) e aperte `Ctrl+C` para encerrá-la. Depois, recarregue a página do front no navegador.

Pergunta: o que a página mostra agora, a tabela antiga, uma tabela vazia ou uma mensagem de erro? Isso confirma que o front guarda os dados, ou que ele depende da API estar de pé a cada carregamento?
