# ADR-001: Estrutura do repositorio de exemplos

**Data:** 2026-09-17

**Status:** Aceita

**Decisores:** Jose Romualdo (professor, disciplina Aplicacoes Web)

## Contexto

Este repositorio vai crescer ao longo do semestre ate reunir os topicos da
disciplina Aplicacoes Web, comecando pelo Topico 02 (cliente-servidor). Tres
escolhas precisam valer para todos os topicos seguintes, e nao fazia sentido
decidir de novo a cada topico novo: o formato do arquivo de solucao do .NET,
o que entra e o que fica de fora do scaffold gerado pelos templates, e como
tratar pacote com vulnerabilidade de seguranca conhecida.

O SDK 10 do .NET passou a gerar, por padrao, uma solucao no formato `.slnx`
em vez do tradicional `.sln`. O template `dotnet new webapi` traz, por
padrao, um controller e um modelo de exemplo (`WeatherForecastController`,
`WeatherForecast`) sem relacao com o case da Clinica Vida+, e o `npm create
vite` gera um `README.md` em ingles descrevendo o template, tambem sem
relacao com o case. E o proprio `dotnet new webapi` trouxe, como dependencia
transitiva de `Microsoft.AspNetCore.OpenApi 10.0.7`, o pacote
`Microsoft.OpenApi` na versao `2.0.0`, que tem vulnerabilidade de severidade
alta registrada em GHSA-v5pm-xwqc-g5wc (CVE-2026-49451): um documento OpenAPI
malicioso, com referencia circular de esquema, pode derrubar o processo que
faz o parsing por estouro de pilha. O repositorio e publico e vai ser clonado
e compilado por uma turma inteira de Analise e Desenvolvimento de Sistemas; a
disciplina tem um topico proprio de seguranca na web (209588), e deixar um
aviso de vulnerabilidade alta aparecer sem tratamento a cada `dotnet
restore`, `build` ou `test` ensinaria o habito contrario ao que esse topico
pretende formar.

## Decisao

Usamos o formato `.sln` (nao o `.slnx` padrao do SDK 10), removemos do
commit o scaffold sem relacao com o case da Clinica Vida+, e fixamos
`Microsoft.OpenApi` numa versao corrigida como `PackageReference` explicito
nos dois projetos da API, em vez de remover o pacote.

## Motivacoes

- **`.sln` em vez de `.slnx`:** o `.sln` ainda e o formato mais reconhecido
  por versoes mais antigas do Visual Studio, do VS Code e de extensoes de
  terceiros que alunos podem ter instaladas nas suas maquinas. O `.slnx` e
  um formato novo (XML), e alunos de ADS em ambiente EAD, que ja relatam
  dificuldade com a ferramenta, nao deveriam precisar descobrir por que a
  IDE deles nao abre o arquivo de solucao esperado. Essa e a mesma decisao
  registrada no ADR-001 do repositorio irmao,
  `uninove-2026-2-ead-modelagem-oo`, para manter os dois repositorios com a
  mesma cara.
- **Remocao do scaffold do template:** `WeatherForecastController.cs` e
  `WeatherForecast.cs` sao o exemplo generico que o `dotnet new webapi`
  cria para toda API nova, sem nenhuma relacao com consultas, pacientes ou
  especialidades da Clinica Vida+. Deixa-los no repositorio confundiria o
  aluno sobre qual codigo e o exemplo da aula e qual e sobra do gerador. O
  mesmo vale para o `README.md` em ingles que o `npm create vite` gera
  dentro de `web/`: o repositorio inteiro e em portugues, e um segundo
  README com instrucoes genericas do template, em outro idioma, colide com
  o README do topico que ja explica como rodar o front.
- **Pacote fixado, em vez de removido:** o advisory GHSA-v5pm-xwqc-g5wc lista
  `2.7.5` como a primeira versao corrigida da linha 2.x, e a linha 3.x tem
  `3.5.4` como primeira versao corrigida; a mais recente disponivel na
  linha 2.x no momento desta decisao e `2.12.2`. Como
  `Microsoft.AspNetCore.OpenApi 10.0.7` (o pacote que o template do SDK 10
  referencia diretamente) foi construido contra a API da linha 2.x do
  `Microsoft.OpenApi`, promover o pacote a `PackageReference` explicito na
  versao `2.12.2`, sem trocar de major version, resolve o aviso sem risco de
  quebrar a integracao entre os dois pacotes. Remover o `Microsoft.OpenApi`
  por completo exigiria tambem remover `AddOpenApi()`/`MapOpenApi()` de
  `Program.cs` e a `PackageReference` de `Microsoft.AspNetCore.OpenApi`, o
  que adiantaria, sem necessidade, uma decisao de escopo (onde entra o
  Swagger/OpenAPI) que pertence ao topico de APIs (209585), nao ao topico de
  cliente-servidor.

## Riscos conhecidos e mitigacoes

- **Risco:** o `.sln` manual pode ficar desatualizado se um novo projeto for
  criado sem passar por `dotnet sln add`.
  **Mitigacao:** o `README.md` de cada novo topico deve documentar o comando
  `dotnet sln add`, e a CI (`dotnet build` na solucao) falha se algum
  projeto nao estiver referenciado corretamente.
- **Risco:** um pacote transitivo novo, de qualquer dependencia futura, pode
  trazer de volta uma vulnerabilidade conhecida sem que ninguem perceba, em
  algum dos proximos topicos.
  **Mitigacao:** a CI ganhou um passo dedicado, "Conferir pacotes
  vulneraveis", que roda `dotnet list package --vulnerable
  --include-transitive` logo apos o restore e falha o job `api` se a saida
  contiver "has the following vulnerable packages". Esse passo foi testado
  localmente antes da correcao, forcando `Microsoft.OpenApi` de volta para
  `2.0.0`, e confirmado que ele reconhece o pacote vulneravel e retorna
  codigo de saida 1.
- **Risco:** fixar `Microsoft.OpenApi` numa versao explicita separa esse
  pacote da versao que `Microsoft.AspNetCore.OpenApi` passar a exigir em
  atualizacoes futuras do SDK, podendo gerar um conflito de versao mais
  adiante.
  **Mitigacao:** o novo passo de CI descrito acima roda a cada `push` e
  `pull_request`, entao qualquer conflito ou vulnerabilidade reintroduzida
  por uma atualizacao de dependencia aparece no proximo envio de codigo, nao
  silenciosamente.

## Consequencias

Positivas: a solucao abre sem aviso em qualquer versao recente de IDE que os
alunos ja tenham instalada; o codigo do topico so contem o que pertence ao
exemplo da Clinica Vida+, sem ruido de gerador de template; `dotnet restore`,
`build` e `test` rodam sem nenhum aviso de vulnerabilidade conhecida; e a CI
passa a barrar, de forma automatica, qualquer pacote vulneravel que entrar
num topico futuro, sem depender de alguem lembrar de checar manualmente.

Negativas: o `.sln` precisa ser atualizado manualmente (via `dotnet sln
add`) toda vez que um projeto novo for criado; `Microsoft.OpenApi` passou a
ser uma dependencia fixada a mao, que alguem precisa lembrar de revisar
quando o SDK do .NET for atualizado; e o job `web` da CI, por enquanto, so
compila o front (`npm run build`), sem nenhum teste automatizado de
interface. Essa ultima lacuna e uma consequencia aceita para o Topico 02: o
exemplo e deliberadamente simples (uma tabela que busca uma lista), e o
esforco de montar testes de UI (por exemplo, com Testing Library) fica para
quando um topico futuro justificar esse investimento por ter logica de
interface mais rica que so a exibicao de uma lista.

## ADRs relacionadas

Nenhuma ate o momento. Este e o primeiro ADR deste repositorio. Ele espelha,
para a disciplina Aplicacoes Web, a decisao de `.sln` registrada no
ADR-001 do repositorio irmao `uninove-2026-2-ead-modelagem-oo`.
