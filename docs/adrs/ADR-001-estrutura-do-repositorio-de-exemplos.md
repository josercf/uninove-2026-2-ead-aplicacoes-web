# ADR-001: Estrutura do repositório de exemplos

**Data:** 2026-09-17

**Status:** Aceita

**Decisores:** José Romualdo (professor, disciplina Aplicações Web)

## Contexto

Este repositório vai crescer ao longo do semestre até reunir os tópicos da
disciplina Aplicações Web, começando pelo Tópico 02 (cliente-servidor). Três
escolhas precisam valer para todos os tópicos seguintes, e não fazia sentido
decidir de novo a cada tópico novo: o formato do arquivo de solução do .NET,
o que entra e o que fica de fora do scaffold gerado pelos templates, e como
tratar pacote com vulnerabilidade de segurança conhecida.

O SDK 10 do .NET passou a gerar, por padrão, uma solução no formato `.slnx`
em vez do tradicional `.sln`. O template `dotnet new webapi` traz, por
padrão, um controller e um modelo de exemplo (`WeatherForecastController`,
`WeatherForecast`) sem relação com o case da Clínica Vida+, e o `npm create
vite` gera um `README.md` em inglês descrevendo o template, também sem
relação com o case. E o próprio `dotnet new webapi` trouxe, como dependência
transitiva de `Microsoft.AspNetCore.OpenApi 10.0.7`, o pacote
`Microsoft.OpenApi` na versão `2.0.0`, que tem vulnerabilidade de severidade
alta registrada em GHSA-v5pm-xwqc-g5wc (CVE-2026-49451): um documento OpenAPI
malicioso, com referência circular de esquema, pode derrubar o processo que
faz o parsing por estouro de pilha. O repositório é público e vai ser clonado
e compilado por uma turma inteira de Análise e Desenvolvimento de Sistemas; a
disciplina tem um tópico próprio de segurança na web (209588), e deixar um
aviso de vulnerabilidade alta aparecer sem tratamento a cada `dotnet
restore`, `build` ou `test` ensinaria o hábito contrário ao que esse tópico
pretende formar.

## Decisão

Usamos o formato `.sln` (não o `.slnx` padrão do SDK 10), removemos do
commit o scaffold sem relação com o case da Clínica Vida+, e fixamos
`Microsoft.OpenApi` numa versão corrigida como `PackageReference` explícito
nos dois projetos da API, com o NuGet Audit configurado para transformar
qualquer aviso de vulnerabilidade futuro em erro de build.

## Motivações

- **`.sln` em vez de `.slnx`:** o `.sln` ainda é o formato mais reconhecido
  por versões mais antigas do Visual Studio, do VS Code e de extensões de
  terceiros que alunos podem ter instaladas nas suas máquinas. O `.slnx` é
  um formato novo (XML), e alunos de ADS em ambiente EAD, que já relatam
  dificuldade com a ferramenta, não deveriam precisar descobrir por que a
  IDE deles não abre o arquivo de solução esperado. Essa é a mesma decisão
  registrada no ADR-001 do repositório irmão,
  `uninove-2026-2-ead-modelagem-oo`, para manter os dois repositórios com a
  mesma cara.
- **Remoção do scaffold do template:** `WeatherForecastController.cs` e
  `WeatherForecast.cs` são o exemplo genérico que o `dotnet new webapi`
  cria para toda API nova, sem nenhuma relação com consultas, pacientes ou
  especialidades da Clínica Vida+. Deixá-los no repositório confundiria o
  aluno sobre qual código é o exemplo da aula e qual é sobra do gerador. O
  mesmo vale para o `README.md` em inglês que o `npm create vite` gera
  dentro de `web/`: o repositório inteiro é em português, e um segundo
  README com instruções genéricas do template, em outro idioma, colide com
  o README do tópico que já explica como rodar o front.
- **Pacote fixado, em vez de removido:** o advisory GHSA-v5pm-xwqc-g5wc lista
  `2.7.5` como a primeira versão corrigida da linha 2.x, e a linha 3.x tem
  `3.5.4` como primeira versão corrigida; a mais recente disponível na
  linha 2.x no momento desta decisão é `2.12.2`. Como
  `Microsoft.AspNetCore.OpenApi 10.0.7` (o pacote que o template do SDK 10
  referencia diretamente) foi construído contra a API da linha 2.x do
  `Microsoft.OpenApi`, promover o pacote a `PackageReference` explícito na
  versão `2.12.2`, sem trocar de major version, resolve o aviso sem risco de
  quebrar a integração entre os dois pacotes. Remover o `Microsoft.OpenApi`
  por completo exigiria também remover `AddOpenApi()`/`MapOpenApi()` de
  `Program.cs` e a `PackageReference` de `Microsoft.AspNetCore.OpenApi`, o
  que adiantaria, sem necessidade, uma decisão de escopo (onde entra o
  Swagger/OpenAPI) que pertence exclusivamente ao tópico de APIs (209585).
- **NuGet Audit como portão real, em vez de depender só de um `grep` na CI:**
  a primeira versão deste repositório barrava pacote vulnerável na CI com um
  passo que rodava `dotnet list package --vulnerable` e procurava a string
  literal em inglês `"has the following vulnerable packages"` na saída. Esse
  mecanismo tem uma falha: se o texto de saída do `dotnet` mudar numa versão
  futura do SDK, ou se a run rodar sob um locale que não seja inglês, o
  `grep` deixa de casar e o passo passa em silêncio, exatamente o cenário que
  deveria impedir. O NuGet Audit (`NuGetAudit`, `NuGetAuditMode` e
  `NuGetAuditLevel`, configurados em
  `topico-02-cliente-servidor/api/Directory.Build.props`) é o mecanismo
  nativo do próprio SDK: ele gera os avisos `NU1901` a `NU1904` durante o
  `restore`, e `WarningsAsErrors` transforma esses avisos em erro de build,
  o que interrompe o `restore` de forma determinística, sem depender de
  nenhuma string de saída. O passo de `grep` na CI foi mantido como
  verificação adicional, mas deixou de ser o portão real.

## Riscos conhecidos e mitigações

- **Risco:** o `.sln` manual pode ficar desatualizado se um novo projeto for
  criado sem passar por `dotnet sln add`.
  **Mitigação:** o `README.md` de cada novo tópico deve documentar o comando
  `dotnet sln add`, e a CI (`dotnet build` na solução) falha se algum
  projeto não estiver referenciado corretamente.
- **Risco:** um pacote transitivo novo, de qualquer dependência futura, pode
  trazer de volta uma vulnerabilidade conhecida sem que ninguém perceba, em
  algum dos próximos tópicos.
  **Mitigação:** o NuGet Audit, configurado em
  `topico-02-cliente-servidor/api/Directory.Build.props`, roda a cada
  `dotnet restore` (local ou na CI) e falha o build com erro `NU1901`-`NU1904`
  assim que um pacote (direto ou transitivo) tiver vulnerabilidade conhecida
  de severidade `low` ou maior. Esse comportamento foi testado de propósito:
  forçamos `Microsoft.OpenApi` de volta para `2.0.0` e confirmamos que o
  `dotnet restore` falha com `error NU1903: Warning As Error`, sem chegar a
  compilar nada; depois restauramos `2.12.2` e confirmamos o build limpo de
  novo. O passo de CI que roda `dotnet list package --vulnerable` continua
  presente como verificação adicional e informativa, mas o portão real
  agora é o próprio restore.
- **Risco:** fixar `Microsoft.OpenApi` numa versão explícita separa esse
  pacote da versão que `Microsoft.AspNetCore.OpenApi` passar a exigir em
  atualizações futuras do SDK, podendo gerar um conflito de versão mais
  adiante.
  **Mitigação:** o NuGet Audit descrito acima roda a cada `dotnet restore`,
  local ou na CI, então qualquer conflito ou vulnerabilidade reintroduzida
  por uma atualização de dependência fica visível já no próximo envio de
  código.

## Consequências

Positivas: a solução abre sem aviso em qualquer versão recente de IDE que os
alunos já tenham instalada; o código do tópico só contém o que pertence ao
exemplo da Clínica Vida+, sem ruído de gerador de template; `dotnet restore`,
`build` e `test` rodam sem nenhum aviso de vulnerabilidade conhecida; e a CI
passa a barrar, pelo mecanismo nativo do NuGet Audit, qualquer pacote
vulnerável que entrar num tópico futuro, sem depender de alguém lembrar de
checar manualmente.

Negativas: o `.sln` precisa ser atualizado manualmente (via `dotnet sln
add`) toda vez que um projeto novo for criado; `Microsoft.OpenApi` passou a
ser uma dependência fixada à mão, que alguém precisa lembrar de revisar
quando o SDK do .NET for atualizado; e o job `web` da CI, por enquanto, só
compila o front (`npm run build`), sem nenhum teste automatizado de
interface. Essa última lacuna é uma consequência aceita para o Tópico 02: o
exemplo é deliberadamente simples (uma tabela que busca uma lista), e o
esforço de montar testes de UI (por exemplo, com Testing Library) fica para
quando um tópico futuro justificar esse investimento por ter lógica de
interface mais rica que só a exibição de uma lista.

## ADRs relacionadas

Nenhuma até o momento. Este é o primeiro ADR deste repositório. Ele espelha,
para a disciplina Aplicações Web, a decisão de `.sln` registrada no
ADR-001 do repositório irmão `uninove-2026-2-ead-modelagem-oo`.
