# Tópico 09: autenticação, sessão e segurança essencial na web

Este exemplo protege a API do tópico 08. A rota `/api/consultas` passa a exigir login, a senha de cada conta é gravada com hash e sal, e a sessão vive num cookie com `HttpOnly`, `Secure` e `SameSite`. A pasta `xss/` repete o alerta de `innerHTML` da aula 04 e mostra a correção com `textContent`.

A API parte do `topico-08-persistencia`, copiado sem alterar os tópicos anteriores. A escolha está em `docs/adrs/ADR-007-sessao-por-cookie-e-hash-de-senha-na-api-do-topico-09.md`. A interface `IConsultaRepositorio`, o contrato JSON e as consultas do tópico 08 permanecem.

## O que mudou em relação ao tópico 08

| Peça | Arquivo | O que faz |
|---|---|---|
| Tabela de contas | `api/ClinicaVida.Api/Entidades/Usuario.cs` e migração `Usuarios` | Guarda e-mail único, `SenhaHash` e o paciente dono da conta |
| Contas de demonstração | `Seguranca/UsuariosIniciais.cs` | Cria Ana e Bruno com a mesma senha, cada um com hash e sal próprios |
| Login e logout | `Controllers/AutenticacaoController.cs` | `POST /api/auth/login`, `POST /api/auth/logout` e `GET /api/auth/eu` |
| Cookie de sessão | `Program.cs` (`AddCookie`) | `HttpOnly`, `Secure`, `SameSite=Lax`, expiração em 30 minutos |
| Rota protegida | `[Authorize]` no `ConsultasController` | 401 sem sessão, 200 com sessão |
| Autorização por recurso | `Controllers/MinhaAgendaController.cs` | 200 ao dono da consulta e 403 a outro usuário logado |
| XSS | `xss/renderizar.js` e `xss/testes/xss.test.js` | `innerHTML` com a falha e `textContent` com a correção, medidos em jsdom |

Contas de demonstração, criadas na primeira subida:

| E-mail | Senha | Paciente |
|---|---|---|
| `ana@vidamais.example` | `Senha@123` | Ana Souza (consulta 1) |
| `bruno@vidamais.example` | `Senha@123` | Bruno Lima (consulta 2) |

A senha é pública e só vale para estudo. Um sistema real não cria conta com senha fixa.

## Pré-requisitos

1. .NET SDK 10.0 ou superior.

   ```bash
   dotnet --version
   ```

   Resultado esperado: um número que começa por `10.`.

2. Node.js 22.22.2 ou superior, para o exemplo de XSS. O jsdom 30.1.1 declara no campo `engines` as versões `^22.22.2 || ^24.15.0 || >=26.0.0`.

   ```bash
   node --version
   ```

   Resultado esperado: `v22.22.2` ou maior.

3. Os comandos `curl` e `sqlite3`, para os passos 4 a 6.

   ```bash
   curl --version && sqlite3 --version
   ```

   Resultado esperado: uma linha de versão de cada um.

## Como executar

### Passo 1: Testar a API

A partir da pasta raiz do repositório:

```bash
dotnet test topico-09-autenticacao/ClinicaVida.sln
```

Resultado esperado: `Passed!  - Failed:     0, Passed:    48`. Os grupos, com `--filter "FullyQualifiedName~Nome"`: `ConsultasRestTests` (12), `ModeloRelacionalTests` (8), `PersistenciaTests` (5), `CorsTests` (5), `AutenticacaoTests` (8), `SenhaArmazenadaTests` (6) e `AutorizacaoPorRecursoTests` (4).

### Passo 2: Testar o XSS

```bash
cd topico-09-autenticacao && npm install && npm test && cd ..
```

Resultado esperado: `ℹ pass 5` e `ℹ fail 0` no fim da saída.

### Passo 3: Subir a API

```bash
cd topico-09-autenticacao/api/ClinicaVida.Api
dotnet run --launch-profile http
```

Resultado esperado: o log mostra `Applying migration '...Usuarios'` e `Now listening on: http://localhost:5084`. O perfil `https` sobe também em `https://localhost:7084` e precisa do certificado de desenvolvimento (`dotnet dev-certs https --trust`). Se a API de outro tópico estiver aberta, encerre-a antes com `Ctrl+C`.

### Passo 4: Ver o 401 e o login

Em outro terminal, a partir da pasta raiz do repositório:

```bash
curl -i http://localhost:5084/api/consultas
```

Resultado esperado: `HTTP/1.1 401 Unauthorized`, sem corpo.

```bash
curl -i -c cookies.txt -H "Content-Type: application/json" -d @topico-09-autenticacao/requisicoes/login-ana.json http://localhost:5084/api/auth/login
```

Resultado esperado: `HTTP/1.1 200 OK` e uma linha como `Set-Cookie: clinica.sessao=CfDJ8...; path=/; secure; samesite=lax; httponly`.

### Passo 5: Usar a sessão e ver o limite da autenticação

```bash
curl -i -b cookies.txt http://localhost:5084/api/consultas/2
curl -i -b cookies.txt http://localhost:5084/api/minha-agenda/2
curl -i -b cookies.txt http://localhost:5084/api/minha-agenda/1
```

Resultado esperado, na ordem: `200 OK` com a consulta do Bruno (a sessão da Ana basta para `/api/consultas`), `403 Forbidden` (a consulta 2 não é da Ana) e `200 OK` com a consulta da Ana.

### Passo 6: Medir o hash no banco

Com a API encerrada, a partir de `topico-09-autenticacao/api/ClinicaVida.Api`:

```bash
sqlite3 -header -column clinica.db "SELECT Id, Email, length(SenhaHash) AS tamanho, SenhaHash FROM Usuarios;"
sqlite3 clinica.db "SELECT count(DISTINCT SenhaHash) AS hashes_distintos, count(*) AS contas FROM Usuarios;"
```

Resultado esperado: duas linhas com `tamanho` 84 e hashes diferentes, e `2|2` na segunda consulta. As duas contas têm a mesma senha, e os hashes diferem por causa do sal.

## Se der errado

### Erro 1: o `curl` devolve 401 depois do login

O arquivo `cookies.txt` ficou numa pasta diferente da que o segundo comando lê, ou o login falhou. Repita o login com `-i` e confira o `200 OK` e a linha `Set-Cookie`.

### Erro 2: o navegador não guarda o cookie em `http://localhost`

O cookie tem o atributo `Secure`, e alguns navegadores (Safari) só o guardam por HTTPS. Suba a API com `dotnet run --launch-profile https` e use `https://localhost:7084`.

### Erro 3: `address already in use`

A API de outro tópico usa a porta 5084. Encerre-a com `Ctrl+C` e rode de novo.

### Erro 4: o teste de XSS não encontra o jsdom

Faltou instalar as dependências. Rode `npm install` dentro de `topico-09-autenticacao`.

## Experimente mudar

### Mudança 1: tirar o `HttpOnly` do cookie

No `Program.cs`, troque `opcoes.Cookie.HttpOnly = true` por `false` e rode `dotnet test`.

Pergunta: qual teste passa a falhar, e o que um script injetado na página passaria a ler em `document.cookie`?

### Mudança 2: trocar `textContent` por `innerHTML`

Em `xss/renderizar.js`, faça `desenharComTextContent` usar `innerHTML` e rode `npm test`.

Pergunta: quais testes falham, e quantas vezes o código do atacante roda?
