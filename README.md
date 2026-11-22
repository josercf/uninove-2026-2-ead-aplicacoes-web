# Aplicações Web, EAD Uninove

Este repositório reúne os exemplos executáveis da disciplina Aplicações Web, do curso de Análise e Desenvolvimento de Sistemas da Uninove, oferta EAD de 2026.2.

Todo o código aqui usa um único case ao longo do semestre: a Clínica Vida+, um sistema de agendamento de consultas médicas. Cada tópico da disciplina adiciona uma peça nova a esse sistema, começando pela comunicação entre uma API ASP.NET Core e um front React.

Os trechos de código citados nas aulas apontam para os arquivos deste repositório. Se algum trecho da aula estiver diferente do código aqui, isso é um erro do material e deve ser reportado ao professor.

## Como começar

1. Clonar o repositório:

   ```bash
   git clone https://github.com/josercf/uninove-2026-2-ead-aplicacoes-web.git
   ```

   Resultado esperado: uma pasta nova chamada `uninove-2026-2-ead-aplicacoes-web` aparece no diretório onde você rodou o comando.

2. Entrar na pasta do repositório:

   ```bash
   cd uninove-2026-2-ead-aplicacoes-web
   ```

   Resultado esperado: o comando `ls` (ou `dir` no Windows) mostra o arquivo `README.md` e as pastas `topico-01-cliente-servidor`, `topico-03-html-css`, `topico-04-javascript`, `topico-05-react`, `topico-06-rest`, `topico-07-fetch`, `topico-08-persistencia`, `topico-09-autenticacao`, `topico-10-testes`, `topico-11-containers` e `topico-12-nuvem`.

Depois disso, siga a seção "Como rodar tudo de uma vez" logo abaixo, ou entre direto na pasta do tópico que você quer estudar.

## Tópicos

| Tópico | Assunto | Pasta |
|---|---|---|
| 01 | Cliente-servidor | [`topico-01-cliente-servidor`](./topico-01-cliente-servidor) |
| 03 | HTML semântico e CSS responsivo | [`topico-03-html-css`](./topico-03-html-css) |
| 04 | Introdução ao JavaScript | [`topico-04-javascript`](./topico-04-javascript) |
| 05 | Frameworks front-end modernos (React) | [`topico-05-react`](./topico-05-react) |
| 06 | Arquiteturas web MVC, REST e microsserviços (API REST) | [`topico-06-rest`](./topico-06-rest) |
| 07 | APIs e integração de dados (`fetch` e CORS) | [`topico-07-fetch`](./topico-07-fetch) |
| 08 | Persistência e modelagem de dados para web (EF Core e SQLite) | [`topico-08-persistencia`](./topico-08-persistencia) |
| 09 | Autenticação, sessão e segurança essencial na web (cookie, hash de senha e XSS) | [`topico-09-autenticacao`](./topico-09-autenticacao) |
| 10 | Testes e depuração de aplicações web (Vitest, Testing Library, xUnit e cobertura) | [`topico-10-testes`](./topico-10-testes) |
| 11 | Aplicações web em containers (Dockerfile, build em estágios, Compose e SQLite em volume) | [`topico-11-containers`](./topico-11-containers) |
| 12 | Implantação em nuvem (configuração por variável de ambiente, `/health` e checklist de implantação) | [`topico-12-nuvem`](./topico-12-nuvem) |

A aula 02 usa a mesma pasta do tópico 01 e por isso não tem pasta própria. Este é o último tópico da disciplina.

## Como rodar tudo de uma vez

Estes comandos rodam a partir da pasta raiz do repositório (a pasta onde está este `README.md`).

1. Restaurar e testar a parte de API de todos os tópicos:

   ```bash
   dotnet restore topico-01-cliente-servidor/ClinicaVida.sln
   dotnet build topico-01-cliente-servidor/ClinicaVida.sln
   dotnet test topico-01-cliente-servidor/ClinicaVida.sln
   ```

   Resultado esperado: o build termina com `Build succeeded` e os testes aparecem como `Passed`.

2. Instalar e compilar a parte de front de todos os tópicos:

   ```bash
   cd topico-01-cliente-servidor/web && npm install && npm run build && cd ../..
   ```

   Resultado esperado: a última linha mostra `built in` seguido de um tempo, sem nenhuma linha de erro.

3. Rodar os testes da página estática do tópico 03:

   ```bash
   cd topico-03-html-css && npm test && cd ..
   ```

   Resultado esperado: `ℹ pass 7` e `ℹ fail 0` no fim da saída.

4. Instalar e testar o filtro em JavaScript do tópico 04:

   ```bash
   cd topico-04-javascript && npm install && npm test && cd ..
   ```

   Resultado esperado: `ℹ pass 6` e `ℹ fail 0` no fim da saída.

5. Instalar, testar e compilar a agenda em React do tópico 05:

   ```bash
   cd topico-05-react && npm ci && npm test && npm run build && cd ..
   ```

   Resultado esperado: `Tests  8 passed (8)` na saída dos testes e `built in` seguido de um tempo na última linha.

6. Testar a API REST do tópico 06:

   ```bash
   dotnet test topico-06-rest/ClinicaVida.sln
   ```

   Resultado esperado: `Passed!  - Failed:     0, Passed:    12` na última linha.

7. Testar a API e o front do tópico 07:

   ```bash
   dotnet test topico-07-fetch/ClinicaVida.sln
   cd topico-07-fetch && npm ci && npm test && cd ..
   ```

   Resultado esperado: `Passed!  - Failed:     0, Passed:    16` na última linha dos testes da API, `ℹ pass 7` do `node --test` e `Tests  10 passed (10)` do Vitest.

8. Testar a API com banco de dados do tópico 08:

   ```bash
   dotnet test topico-08-persistencia/ClinicaVida.sln
   ```

   Resultado esperado: `Passed!  - Failed:     0, Passed:    29` na última linha.

9. Testar a API com autenticação e o exemplo de XSS do tópico 09:

   ```bash
   dotnet test topico-09-autenticacao/ClinicaVida.sln
   cd topico-09-autenticacao && npm install && npm test && cd ..
   ```

   Resultado esperado: `Passed!  - Failed:     0, Passed:    48` na última linha dos testes da API e `ℹ pass 5` do `node --test`.

10. Testar a API, os componentes React e a cobertura do tópico 10:

   ```bash
   dotnet test topico-10-testes/ClinicaVida.sln
   cd topico-10-testes && npm ci && npm test && npm run cobertura && cd ..
   ```

   Resultado esperado: `Passed!  - Failed:     0, Passed:    60` na última linha dos testes da API, `ℹ pass 7` do `node --test`, `Tests  17 passed | 1 expected fail (18)` do Vitest e `Lines        : 100% ( 35/35 )` no resumo de cobertura.

11. Subir a API, o front e o banco em containers e rodar o teste de fumaça do tópico 11:

   ```bash
   cd topico-11-containers && docker compose up -d --build && ./teste-de-fumaca.sh; docker compose down -v; cd ..
   ```

   Resultado esperado: oito linhas `ok` e `Teste de fumaça: todas as verificações passaram.`, seguidas das linhas `Removed` do `docker compose down -v`. Exige Docker com Compose v2 e as portas 5084 e 5173 livres.

12. Testar a configuração sem segredo e a rota `/health`, e conferir a API no container local, no tópico 12:

   ```bash
   dotnet test topico-12-nuvem/ClinicaVida.sln
   cd topico-12-nuvem && docker compose up -d --build --wait && ./teste-de-saude.sh; docker compose down -v; cd ..
   ```

   Resultado esperado: `Passed!  - Failed:     0, Passed:    21` nos testes, cinco linhas `ok` e `Teste de saúde: todas as verificações passaram.` no container, seguidas das linhas `Removed`. Exige Docker com Compose v2 e a porta 5084 livre. Nada é publicado em conta de provedor.

Para rodar um exemplo específico com o passo a passo completo, entre na pasta do tópico indicada na tabela acima e siga o `README.md` de dentro dela. Cada tópico tem seu próprio passo a passo, com a saída esperada e uma seção de ajuda caso algo dê errado.

## Pré-requisitos gerais

- Docker com Compose v2, só para os tópicos 11 e 12.
- .NET SDK 10.0 ou superior instalado. Confirme com `dotnet --version`.
- Node.js 18 ou superior e npm instalados, e Node.js 22.12 ou superior para o tópico 05. Confirme com `node --version` e `npm --version`.
- Git instalado, para clonar o repositório.

## Estrutura do repositório

- `topico-NN-nome-do-assunto/`: um exemplo executável por tópico da disciplina, com API, front (quando o tópico tiver um) e README próprio.
- `docs/adrs/`: as decisões arquiteturais registradas ao longo do semestre, uma por arquivo, numeradas em ordem.
- `.github/workflows/ci.yml`: a integração contínua, que compila e testa a API, compila o front, roda os testes da página do tópico 03, os das funções JavaScript do tópico 04 os dos componentes React do tópico 05 e os da API REST do tópico 06, os da API com CORS e os do front com `fetch` do tópico 07, e compila os tópicos 05 e 07, os da API com banco de dados do tópico 08, os da API com autenticação e do exemplo de XSS do tópico 09, os da API, dos componentes React e da cobertura do tópico 10, o teste de fumaça do tópico 11 sobre os containers, e os testes de configuração e de saúde do tópico 12, a cada envio de código.

## Licença e uso didático

Este repositório está sob a licença MIT (veja o arquivo [`LICENSE`](./LICENSE)). O código foi escrito para fins didáticos, como material de apoio da disciplina, e não passou por revisão de segurança nem foi pensado para uso em produção.
