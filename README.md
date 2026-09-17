# Aplicacoes Web, EAD Uninove

Este repositorio reune os exemplos executaveis da disciplina Aplicacoes Web, do curso de Analise e Desenvolvimento de Sistemas da Uninove, oferta EAD de 2026.2.

Todo o codigo aqui usa um unico case ao longo do semestre: a Clinica Vida+, um sistema de agendamento de consultas medicas. Cada topico da disciplina adiciona uma peca nova a esse sistema, comecando pela comunicacao entre uma API ASP.NET Core e um front React.

Os trechos de codigo citados nas aulas apontam para os arquivos deste repositorio. Se algum trecho da aula estiver diferente do codigo aqui, isso e um erro do material e deve ser reportado ao professor.

## Topicos

| Topico | Assunto | Pasta |
|---|---|---|
| 02 | Cliente-servidor | [`topico-02-cliente-servidor`](./topico-02-cliente-servidor) |

Os demais topicos da disciplina serao adicionados a esta tabela conforme as proximas aulas forem publicadas.

## Como rodar tudo de uma vez

Estes comandos rodam a partir da pasta raiz do repositorio (a pasta onde esta este `README.md`).

1. Restaurar e testar a parte de API de todos os topicos:

   ```bash
   dotnet restore topico-02-cliente-servidor/ClinicaVida.sln
   dotnet build topico-02-cliente-servidor/ClinicaVida.sln
   dotnet test topico-02-cliente-servidor/ClinicaVida.sln
   ```

   Resultado esperado: o build termina com `Build succeeded` e os testes aparecem como `Passed`.

2. Instalar e compilar a parte de front de todos os topicos:

   ```bash
   cd topico-02-cliente-servidor/web && npm install && npm run build && cd ../..
   ```

   Resultado esperado: a ultima linha mostra `built in` seguido de um tempo, sem nenhuma linha de erro.

Para rodar um exemplo especifico com o passo a passo completo, entre na pasta do topico indicada na tabela acima e siga o `README.md` de dentro dela. Cada topico tem seu proprio passo a passo, com a saida esperada e uma secao de ajuda caso algo de errado.

## Pre-requisitos gerais

- .NET SDK 10.0 ou superior instalado. Confirme com `dotnet --version`.
- Node.js 18 ou superior e npm instalados. Confirme com `node --version` e `npm --version`.
- Git instalado, para clonar o repositorio.

## Estrutura do repositorio

- `topico-NN-nome-do-assunto/`: um exemplo executavel por topico da disciplina, com API, front (quando o topico tiver um) e README proprio.
- `.github/workflows/ci.yml`: a integracao continua, que compila e testa a API e compila o front a cada envio de codigo.

## Licenca e uso didatico

Este repositorio esta sob a licenca MIT (veja o arquivo [`LICENSE`](./LICENSE)). O codigo foi escrito para fins didaticos, como material de apoio da disciplina, e nao passou por revisao de seguranca nem foi pensado para uso em producao.
