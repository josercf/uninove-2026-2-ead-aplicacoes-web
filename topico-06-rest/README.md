# Tópico 06: API REST da agenda (MVC, REST e microsserviços)

Este exemplo completa a API da Clínica Vida+ do tópico 01. A rota continua `/api/consultas`, e o JSON de cada consulta continua com os campos `id`, `paciente`, `especialidade` e `dataHora`. O tópico de APIs e integração de dados troca o `dados.js` do tópico 05 por uma chamada a esta API.

O que muda em relação ao tópico 01:

- A API responde aos cinco pedidos de um recurso REST: listar, buscar um, criar, substituir e remover.
- As consultas ficam num repositório em memória, atrás da interface `IConsultaRepositorio`. O controller conhece só a interface; um tópico futuro troca a lista em memória por um banco de dados sem mexer no controller.
- A agenda começa com as mesmas 3 consultas do `dados.js` do tópico 05.

| Verbo e rota | Sucesso | Falha |
|---|---|---|
| `GET /api/consultas` | 200 com a lista | |
| `GET /api/consultas/{id}` | 200 com a consulta | 404 se o id não existe |
| `POST /api/consultas` | 201 com a consulta e o cabeçalho `Location` | 400 se faltar paciente ou especialidade |
| `PUT /api/consultas/{id}` | 204 sem corpo | 404 se o id não existe; 400 se faltar paciente ou especialidade |
| `DELETE /api/consultas/{id}` | 204 sem corpo | 404 se o id não existe |

## Arquivos

- `api/ClinicaVida.Api/Models/Consulta.cs`: o modelo, com os mesmos campos do tópico 01 e `[Required]` no paciente e na especialidade.
- `api/ClinicaVida.Api/Repositorios/IConsultaRepositorio.cs`: a interface com as operações que o controller usa.
- `api/ClinicaVida.Api/Repositorios/ConsultaRepositorioEmMemoria.cs`: a implementação com uma lista em memória.
- `api/ClinicaVida.Api/Controllers/ConsultasController.cs`: as cinco ações, uma por verbo.
- `api/ClinicaVida.Api/Program.cs`: registra o repositório e liga as rotas.
- `api/ClinicaVida.Api/ClinicaVida.Api.http`: uma requisição por verbo, para enviar pelo editor.
- `api/ClinicaVida.Api.Tests/ConsultasRestTests.cs`: doze testes de integração com `WebApplicationFactory`, um por verbo e por código de status.
- `requisicoes/*.json`: os corpos usados nos comandos `curl` de POST e PUT.

A escolha do repositório atrás de interface está registrada em `docs/adrs/ADR-004-repositorio-em-memoria-atras-de-interface.md`.

## Pré-requisitos

1. .NET SDK 10.0 ou superior.

   ```bash
   dotnet --version
   ```

   Resultado esperado: um número igual ou maior que `10.0.0`.

2. `curl`. No macOS e no Linux, ele já vem instalado. No Windows 10 e 11, o programa se chama `curl.exe`; no PowerShell, digite `curl.exe`, com a extensão, porque no Windows PowerShell 5.1 a palavra `curl` sozinha chama outro comando.

## Como executar

Rode os comandos a partir da pasta raiz do repositório, salvo quando o passo disser outra coisa.

### Passo 1: Rodar os testes

```bash
dotnet test topico-06-rest/ClinicaVida.sln
```

Resultado esperado: a última linha começa como a linha abaixo. A duração varia a cada execução.

```text
Passed!  - Failed:     0, Passed:    12, Skipped:     0, Total:    12
```

### Passo 2: Subir a API

```bash
dotnet run --project topico-06-rest/api/ClinicaVida.Api
```

Resultado esperado: a linha `Now listening on: http://localhost:5084`. Deixe este terminal aberto. A porta é a mesma do tópico 01; pare a API do tópico 01 antes, se ela estiver no ar.

### Passo 3: Chamar cada verbo

Abra um segundo terminal e entre na pasta do tópico, onde ficam os arquivos de `requisicoes/`.

```bash
cd topico-06-rest
```

Envie as requisições na ordem abaixo. No Windows, troque `curl` por `curl.exe`; o resto do comando é igual.

```bash
curl -i http://localhost:5084/api/consultas
curl -i http://localhost:5084/api/consultas/2
curl -i http://localhost:5084/api/consultas/99
curl -i -X POST http://localhost:5084/api/consultas -H "Content-Type: application/json" --data "@requisicoes/nova-consulta.json"
curl -i -X POST http://localhost:5084/api/consultas -H "Content-Type: application/json" --data "@requisicoes/consulta-sem-paciente.json"
curl -i -X PUT http://localhost:5084/api/consultas/1 -H "Content-Type: application/json" --data "@requisicoes/consulta-alterada.json"
curl -i -X DELETE http://localhost:5084/api/consultas/3
curl -i -X DELETE http://localhost:5084/api/consultas/3
```

Resultado esperado: a primeira linha de cada resposta traz, na ordem, `200 OK`, `200 OK`, `404 Not Found`, `201 Created`, `400 Bad Request`, `204 No Content`, `204 No Content` e `404 Not Found`. A seção "Saída esperada" mostra as respostas completas.

O `-i` mostra a linha de status e os cabeçalhos antes do corpo. O `--data "@arquivo"` envia o conteúdo do arquivo como corpo; as aspas evitam que o PowerShell interprete o `@`.

No PowerShell 7, o `Invoke-RestMethod` faz o mesmo pedido e guarda o status numa variável:

```powershell
Invoke-RestMethod -Method Post -Uri http://localhost:5084/api/consultas -ContentType "application/json" -InFile requisicoes/nova-consulta.json -StatusCodeVariable status
$status
```

Resultado esperado: uma tabela com `id` 4 e `paciente` Diego Rocha, seguida do número `201`. O `Invoke-RestMethod` mostra a data no formato do idioma do sistema, por exemplo `07/10/2026 08:00:00`.

## Saída esperada

Saídas colhidas em 14/10/2026 com a API recém-iniciada. Os campos `Date` e `traceId` mudam a cada requisição.

```text
$ curl -i http://localhost:5084/api/consultas/2
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Wed, 14 Oct 2026 02:50:13 GMT
Server: Kestrel
Transfer-Encoding: chunked

{"id":2,"paciente":"Bruno Lima","especialidade":"Cardiologia","dataHora":"2026-10-05T10:30:00"}

$ curl -i -X POST http://localhost:5084/api/consultas -H "Content-Type: application/json" --data "@requisicoes/nova-consulta.json"
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Date: Wed, 14 Oct 2026 02:50:13 GMT
Server: Kestrel
Location: http://localhost:5084/api/consultas/4
Transfer-Encoding: chunked

{"id":4,"paciente":"Diego Rocha","especialidade":"Dermatologia","dataHora":"2026-10-07T08:00:00"}

$ curl -i -X POST http://localhost:5084/api/consultas -H "Content-Type: application/json" --data "@requisicoes/consulta-sem-paciente.json"
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json; charset=utf-8
Date: Wed, 14 Oct 2026 02:50:13 GMT
Server: Kestrel
Transfer-Encoding: chunked

{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"Paciente":["Informe o paciente."]},"traceId":"00-f6a65d00450d6293c42fb4e66ed5a27f-54b7e6284d4c5697-00"}

$ curl -i -X PUT http://localhost:5084/api/consultas/1 -H "Content-Type: application/json" --data "@requisicoes/consulta-alterada.json"
HTTP/1.1 204 No Content
Date: Wed, 14 Oct 2026 02:50:13 GMT
Server: Kestrel

$ curl -i -X DELETE http://localhost:5084/api/consultas/3
HTTP/1.1 404 Not Found
Content-Type: application/problem+json; charset=utf-8
Date: Wed, 14 Oct 2026 02:50:13 GMT
Server: Kestrel
Transfer-Encoding: chunked

{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.5","title":"Not Found","status":404,"traceId":"00-62fab94517c8318a8e926075f1a2879e-5f1175ca16486bca-00"}
```

O último bloco é o segundo `DELETE` da consulta 3, que já tinha sido removida pelo primeiro. Depois da sequência, `curl -s http://localhost:5084/api/consultas` devolve as consultas 1, 2 e 4, com a consulta 1 em Cardiologia.

As alterações duram só enquanto a API está no ar. Pare com `Ctrl+C`, suba de novo, e a agenda volta às 3 consultas iniciais.

## Se der errado

### Erro 1: `Failed to bind to address http://127.0.0.1:5084: address already in use`

A porta 5084 já está ocupada, normalmente pela API do tópico 01 ou por uma execução anterior deste tópico.

Correção no macOS e no Linux:

```bash
lsof -i :5084
kill <PID>
```

Correção no Windows (PowerShell):

```powershell
netstat -ano | findstr :5084
taskkill /PID <PID> /F
```

Troque `<PID>` pelo número da coluna PID e rode o Passo 2 de novo.

### Erro 2: `curl: Failed to open requisicoes/nova-consulta.json`

A mensagem vem seguida de `curl: option --data: error encountered when reading a file`. O `curl` não achou o arquivo do `--data`. O terminal não está na pasta `topico-06-rest`. Rode `cd topico-06-rest` a partir da raiz do repositório e repita o comando.

### Erro 3: `415 Unsupported Media Type` no POST ou no PUT

O pedido saiu sem o cabeçalho `Content-Type: application/json`, e a API não reconhece o corpo como JSON. Confira se o `-H "Content-Type: application/json"` está no comando.

### Erro 4: no PowerShell, o `curl -i` pede um parâmetro `Uri` ou mostra uma tabela com `StatusCode`

No Windows PowerShell 5.1, `curl` é outro nome do `Invoke-WebRequest`. Digite `curl.exe`, com a extensão.

## Experimente mudar

1. Envie o `POST` de `nova-consulta.json` duas vezes. Qual id cada uma recebe, e o que o `Location` de cada resposta traz?
2. Remova a consulta 4 com `DELETE` e crie outra com `POST`. O id 4 volta a ser usado? Confira no `ConsultaRepositorioEmMemoria.cs` o motivo.
3. Apague o `[Required]` da especialidade no `Consulta.cs`, reinicie a API e envie um `POST` sem especialidade. Qual status volta agora?
