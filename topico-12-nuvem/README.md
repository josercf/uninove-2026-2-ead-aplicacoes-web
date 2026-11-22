# Tópico 12: implantação em nuvem

Este tópico reúne os arquivos de configuração e o checklist de implantação da Clínica Vida+ para qualquer provedor de nuvem. Nada aqui é publicado em conta real: a aula apresenta os serviços principais dos três maiores provedores e o que a implantação exige em todos eles.

A API é a do tópico 11, com duas mudanças. A rota `GET /health` responde 200 sem login, e o `compose.yaml` lê todos os valores que mudam por ambiente de variáveis de ambiente, com padrão para o computador local. O front do tópico 11 não entra aqui.

## O que há na pasta

| Peça | Arquivo | O que faz |
|---|---|---|
| API | `api/ClinicaVida.Api` | A API do tópico 11 mais `AddHealthChecks` e `MapHealthChecks("/health")` no `Program.cs` |
| Imagem | `api/Dockerfile` | A mesma do tópico 11, em dois estágios, usuário `app` |
| Orquestração | `compose.yaml` | A API na porta 5084, variáveis por ambiente e `healthcheck` |
| Modelo de variáveis | `.env.exemplo` | Nomes e valores de exemplo, sem segredo; a cópia `.env` fica fora do Git |
| Teste de configuração | `api/ClinicaVida.Api.Tests` | 21 verificações: sem segredo nos arquivos versionados, `/health` 200, CORS por configuração |
| Teste de saúde no container | `teste-de-saude.sh` | 5 verificações com `curl` e `docker compose ps` |
| Demonstração de variáveis | `demonstra-variaveis.sh` | A mesma imagem com duas configurações, com a saída de cada uma |
| Checklist | `CHECKLIST-IMPLANTACAO.md` | O que conferir antes de implantar em qualquer provedor |

## Pré-requisitos

1. Docker com Compose v2, conferido com `docker compose version` (resultado esperado: uma linha `Docker Compose version v2...`).
2. SDK do .NET 10, para os testes de configuração (`dotnet --version` mostra `10.x`).
3. `curl` e `bash`.
4. A porta 5084 livre no computador.

## Como executar

### Passo 1: Rodar os testes de configuração

A partir da pasta `topico-12-nuvem`:

```bash
dotnet test ClinicaVida.sln
```

Resultado esperado: `Passed!  - Failed:     0, Passed:    21, Skipped:     0, Total:    21`.

Os testes do arquivo `SegredoVersionadoTests.cs` leem `appsettings.json`, `appsettings.Development.json`, `compose.yaml` e `.env.exemplo` e procuram senha em connection string, chave nomeada como segredo, credencial dentro de URL, chave de acesso da AWS e chave privada PEM. Os de `SaudeTests.cs` conferem `/health` e a origem do CORS vinda da configuração.

### Passo 2: Subir a API no container local

```bash
docker compose up -d --build --wait
```

Resultado esperado: as linhas `Container clinica-vida-nuvem-api-1 Started` e `Container clinica-vida-nuvem-api-1 Healthy`. A opção `--wait` espera a verificação de saúde passar.

### Passo 3: Rodar o teste de saúde

```bash
./teste-de-saude.sh
```

Resultado esperado:

```text
ok    GET /health: HTTP 200
ok    corpo de /health: Healthy
ok    GET /api/consultas sem login: HTTP 401
ok    Access-Control-Allow-Origin: http://localhost:5173
ok    docker compose ps: Up 6 seconds (healthy)
Teste de saúde: todas as verificações passaram.
```

### Passo 4: Ver a configuração por variável de ambiente

```bash
./demonstra-variaveis.sh
```

Resultado esperado, sem as linhas `Container ... Running`, `Recreate` e `Healthy` do Compose:

```text
=== 1. Valores padrão (sem .env) ===
http://localhost:5173
Data Source=/dados/clinica.db
--- Origin enviada: http://localhost:5173
Access-Control-Allow-Origin: http://localhost:5173

=== 2. Mesma imagem, outras variáveis ===
https://clinica.exemplo.org
Data Source=/dados/outro.db
--- Origin enviada: https://clinica.exemplo.org
Access-Control-Allow-Origin: https://clinica.exemplo.org
https://clinica.exemplo.org
Data Source=/dados/outro.db
--- Origin enviada: http://localhost:5173
(sem Access-Control-Allow-Origin)
--- Arquivos de banco no volume:
clinica.db
outro.db

=== 3. Configuração resolvida pelo compose, sem subir nada ===
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__Clinica: Data Source=/dados/clinica.db
      Cors__Origem: https://clinica.exemplo.org
```

A mesma imagem muda de comportamento só pelas variáveis `CORS_ORIGEM` e `CONEXAO_CLINICA`. Para guardar valores de forma permanente, copie `.env.exemplo` para `.env` e edite; o `.gitignore` do tópico impede que o `.env` seja versionado.

### Passo 5: Parar e remover tudo

```bash
docker compose down -v
```

Resultado esperado: o container, a rede e o volume `clinica-vida-nuvem_dados` aparecem como `Removed`.

## Se der errado

### Erro 1: `port is already allocated`

Outro programa usa a 5084, por exemplo a API do tópico 11. Rode `docker compose down` na pasta do outro tópico ou defina `PORTA_API=5085`. Com outra porta, rode também `API=http://localhost:5085 ./teste-de-saude.sh`.

### Erro 2: o build falha com `NETSDK1064`

A pasta `obj` do computador entrou no contexto do build. O `api/.dockerignore` lista `**/bin/` e `**/obj/` para evitar isso; confirme que o arquivo não foi alterado.

### Erro 3: o teste de saúde falha em `docker compose ps`

O container ainda está em `health: starting`. Rode `docker compose up -d --wait` e repita.

### Erro 4: um teste de segredo falha

A mensagem traz o arquivo e a linha. Tire o valor do arquivo, passe-o para uma variável de ambiente e, se ele já foi versionado, troque o segredo.

## Experimente mudar

### Mudança 1: introduzir um segredo falso

Crie `api/ClinicaVida.Api/appsettings.Production.json` com `"Clinica": "Server=db;User Id=sa;Password=Abc12345"` e rode `dotnet test`. O teste `Arquivo_versionado_nao_traz_segredo` falha com `linha 1: senha em connection string`. Apague o arquivo em seguida.

### Mudança 2: derrubar a verificação de saúde

Comente `app.MapHealthChecks("/health");` no `Program.cs` e rode os testes. O teste `Health_responde_200_sem_login` falha com `Expected: OK, Actual: NotFound`.
