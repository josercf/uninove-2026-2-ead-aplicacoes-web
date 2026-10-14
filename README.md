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

   Resultado esperado: o comando `ls` (ou `dir` no Windows) mostra o arquivo `README.md` e as pastas `topico-01-cliente-servidor`, `topico-03-html-css`, `topico-04-javascript`, `topico-05-react` e `topico-06-rest`.

Depois disso, siga a seção "Como rodar tudo de uma vez" logo abaixo, ou entre direto na pasta do tópico que você quer estudar.

## Tópicos

| Tópico | Assunto | Pasta |
|---|---|---|
| 01 | Cliente-servidor | [`topico-01-cliente-servidor`](./topico-01-cliente-servidor) |
| 03 | HTML semântico e CSS responsivo | [`topico-03-html-css`](./topico-03-html-css) |
| 04 | Introdução ao JavaScript | [`topico-04-javascript`](./topico-04-javascript) |
| 05 | Frameworks front-end modernos (React) | [`topico-05-react`](./topico-05-react) |
| 06 | Arquiteturas web MVC, REST e microsserviços (API REST) | [`topico-06-rest`](./topico-06-rest) |

A aula 02 usa a mesma pasta do tópico 01 e por isso não tem pasta própria. Os demais tópicos da disciplina serão adicionados a esta tabela conforme as próximas aulas forem publicadas.

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

Para rodar um exemplo específico com o passo a passo completo, entre na pasta do tópico indicada na tabela acima e siga o `README.md` de dentro dela. Cada tópico tem seu próprio passo a passo, com a saída esperada e uma seção de ajuda caso algo dê errado.

## Pré-requisitos gerais

- .NET SDK 10.0 ou superior instalado. Confirme com `dotnet --version`.
- Node.js 18 ou superior e npm instalados, e Node.js 22.12 ou superior para o tópico 05. Confirme com `node --version` e `npm --version`.
- Git instalado, para clonar o repositório.

## Estrutura do repositório

- `topico-NN-nome-do-assunto/`: um exemplo executável por tópico da disciplina, com API, front (quando o tópico tiver um) e README próprio.
- `docs/adrs/`: as decisões arquiteturais registradas ao longo do semestre, uma por arquivo, numeradas em ordem.
- `.github/workflows/ci.yml`: a integração contínua, que compila e testa a API, compila o front, roda os testes da página do tópico 03, os das funções JavaScript do tópico 04 os dos componentes React do tópico 05 e os da API REST do tópico 06, e compila o tópico 05, a cada envio de código.

## Licença e uso didático

Este repositório está sob a licença MIT (veja o arquivo [`LICENSE`](./LICENSE)). O código foi escrito para fins didáticos, como material de apoio da disciplina, e não passou por revisão de segurança nem foi pensado para uso em produção.
