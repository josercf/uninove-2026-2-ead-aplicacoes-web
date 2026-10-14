# ADR-004: Repositório em memória atrás de interface na API do tópico 06

**Data:** 2026-10-14

**Status:** Aceita

**Decisores:** José Romualdo (professor, disciplina Aplicações Web)

## Contexto

No tópico 01, a agenda da Clínica Vida+ era uma lista `static` declarada
dentro do `ConsultasController`, com duas consultas, e a API só respondia a
`GET`. O tópico 06 completa a API com `POST`, `PUT` e `DELETE`, e três tópicos
seguintes dependem dela:

- o tópico de APIs e integração de dados troca o `dados.js` do tópico 05 por
  `fetch` nesta API, e espera a mesma rota `/api/consultas` e os mesmos campos
  de JSON (`id`, `paciente`, `especialidade`, `dataHora`);
- o tópico de persistência acrescenta um banco de dados atrás do controller;
- os testes precisam de uma agenda conhecida no início de cada caso, porque
  `POST`, `PUT` e `DELETE` alteram os dados.

Com a lista `static` dentro do controller, um teste que remove a consulta 3
deixaria a agenda alterada para o teste seguinte, e trocar a lista por banco
exigiria reescrever o controller.

## Decisão

O controller do tópico 06 recebe por injeção de dependência a interface
`IConsultaRepositorio`, implementada por `ConsultaRepositorioEmMemoria` e
registrada como singleton no `Program.cs`, e cada teste de integração cria a
própria `WebApplicationFactory<Program>`.

## Motivações

- **Troca de armazenamento sem mexer no controller:** o tópico de persistência
  registra outra implementação da mesma interface numa linha do `Program.cs`.
- **Isolamento dos testes:** o repositório vive no contêiner de serviços de
  cada fábrica. Cada um dos onze testes começa com as 3 consultas iniciais,
  sem depender da ordem de execução.
- **Mesmos dados do front:** as 3 consultas iniciais são as do `dados.js` do
  tópico 05. A agenda em React mostra a mesma tela quando passar a buscar os
  dados nesta API.
- **Rota escrita por extenso:** `[Route("api/consultas")]` no lugar de
  `api/[controller]`. O cabeçalho `Location` do `POST` sai em minúsculas,
  `/api/consultas/4`, igual à rota que as aulas citam.

## Riscos conhecidos e mitigações

- **Risco:** os dados se perdem quando a API para.
  **Mitigação:** comportamento declarado no README do tópico e na aula; o
  banco de dados entra no tópico de persistência.
- **Risco:** uma fábrica por teste sobe a API onze vezes.
  **Mitigação:** a suíte inteira levou cerca de 1 segundo em 14/10/2026. Se
  o número de testes crescer a ponto de pesar, a alternativa é uma fábrica
  compartilhada com a agenda reiniciada antes de cada teste.
- **Risco:** requisições simultâneas alterando a mesma lista.
  **Mitigação:** cada operação do repositório roda dentro de um `lock`.

## Consequências

Positivas: o controller fica com uma linha por verbo; os testes não dependem
de ordem; a troca por banco tem ponto de entrada definido.

Negativas: o tópico 06 tem um arquivo de interface e uma classe a mais do que
o tópico 01, e o aluno precisa entender injeção de dependência para seguir o
caminho de uma requisição.

## ADRs relacionadas

- ADR-002, numeração das pastas pelo número da aula.
