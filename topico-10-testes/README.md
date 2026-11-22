# Tópico 10: testes e depuração de aplicações web

Este exemplo reúne a API do tópico 09 e o front React do tópico 07 em uma só pasta e mede o que cada camada de teste cobre. Ele traz o teste de componente com Testing Library, os testes xUnit da API com login, o relatório de cobertura das duas camadas e um experimento em que a cobertura chega a 100% com um defeito vivo.

A decisão sobre o 401 do tópico 09 está em `docs/adrs/ADR-008-testes-em-tres-camadas-e-cobertura-no-topico-10.md`. Resumo: o front mostra a mensagem "Faça login para ver a agenda." quando a API responde 401, e o login com a API real é testado no lado da API. A tela de login não faz parte do exemplo.

## O que mudou em relação aos tópicos 07 e 09

| Peça | Arquivo | O que faz |
|---|---|---|
| API | `api/` | Cópia da API do tópico 09, mais a regra de horário de atendimento (`Agenda/RegraDeAgenda.cs`) ligada ao POST e ao PUT |
| Front | `src/`, `index.html`, `vite.config.js` | Cópia do front do tópico 07. `App.jsx` distingue 401 e 403 de outros erros |
| Teste de componente | `src/__tests__/sessao.test.jsx` | Vitest, Testing Library e `userEvent`: 401, 403, erro genérico e filtro pelo teclado |
| Teste de unidade da API | `api/ClinicaVida.Api.Tests/RegraDeAgendaTests.cs` | 8 casos sem servidor, sem banco e sem rede |
| Teste de integração da API | `api/ClinicaVida.Api.Tests/HorarioDeAtendimentoTests.cs` | 4 casos da regra de horário pelo HTTP, com login |
| Cobertura enganosa | `src/cobertura/` e `src/__tests__/cobertura.test.js` | Uma função correta, uma com defeito de limite, um teste fraco e um teste forte |
| Relatórios de cobertura | `npm run cobertura` e `dotnet test --collect:"XPlat Code Coverage"` | Cobertura do front (v8) e da API (coverlet) |

## Pré-requisitos

1. .NET SDK 10.0 ou superior, conferido com `dotnet --version` (resultado esperado: um número que começa por `10.` ou maior).
2. Node.js 22.12 ou superior, conferido com `node --version`.

## Como executar

### Passo 1: Testar a API

A partir da pasta raiz do repositório:

```bash
dotnet test topico-10-testes/ClinicaVida.sln
```

Resultado esperado: `Passed!  - Failed:     0, Passed:    60` na última linha.

### Passo 2: Testar o front

```bash
cd topico-10-testes && npm install && npm test && cd ..
```

Resultado esperado: `ℹ pass 7` do `node --test` e `Tests  17 passed | 1 expected fail (18)` do Vitest. O caso "expected fail" é o teste forte que reprova de propósito a função com defeito (`test.fails`).

### Passo 3: Ver a cobertura do front

```bash
cd topico-10-testes && npm run cobertura && cd ..
```

Resultado esperado: o resumo termina com `Lines        : 100% ( 35/35 )`. A linha `Branches` fica abaixo de 100% (95,65%), porque `agenda.js` tem um ramo que nenhum teste percorre.

### Passo 4: Ver a cobertura da API

```bash
dotnet test topico-10-testes/ClinicaVida.sln --collect:"XPlat Code Coverage" --results-directory topico-10-testes/TestResults
```

Resultado esperado: o comando imprime o caminho de um arquivo `coverage.cobertura.xml`. Nele, `line-rate` e `branch-rate` são frações (0,985 e 0,935 incluem as migrações geradas; sem elas, a API mede 100% das linhas e 93,5% dos ramos).

### Passo 5: Reproduzir a cobertura que não pega defeito

```bash
cd topico-10-testes
npx vitest run src/__tests__/cobertura.test.js -t "fraco" --coverage --coverage.include=src/cobertura/proximaConsultaComDefeito.js --coverage.reporter=text
```

Resultado esperado: `Lines        : 100% ( 3/3 )` e `Branches     : 100% ( 2/2 )` para `proximaConsultaComDefeito.js`, com os 2 testes fracos passando. Em seguida, o teste forte (`test.fails` em `cobertura.test.js`) confirma que a mesma função devolve a consulta 2 onde deveria devolver a 1.

### Passo 6: Depurar no navegador e no `dotnet`

Os passos numerados estão no plano da aula. Para o 401 no navegador: suba a API (`dotnet run --launch-profile http` em `api/ClinicaVida.Api`), suba o front (`npm run dev`), abra as ferramentas do navegador em Fontes, abra `src/api.js` e marque um ponto de parada na linha `if (!resposta.ok)`. Recarregue a página e confira `resposta.status` igual a 401 no painel Escopo. Para o horário no `dotnet`: marque um ponto de parada em `RegraDeAgenda.EstaNoHorarioDeAtendimento`, inicie a API pelo depurador do editor, envie `requisicoes/consulta-no-sabado.json` (depois do login com `requisicoes/login-ana.json`) e confira `instante.DayOfWeek` igual a `Saturday`.

## Se der errado

### Erro 1: `Could not find 'tests/'`

O comando rodou fora da pasta `topico-10-testes`. Entre nela e repita.

### Erro 2: `address already in use`

A API de outro tópico usa a porta 5084. Encerre-a com `Ctrl+C`.

### Erro 3: o front mostra "Faça login para ver a agenda."

É o comportamento esperado contra a API real: o exemplo não tem tela de login. Os testes usam uma API simulada e, no lado da API, o cliente com sessão.

## Experimente mudar

### Mudança 1: corrigir o defeito pelo teste

Em `src/cobertura/proximaConsultaComDefeito.js`, troque `>` por `>=`. O teste `test.fails` passa a falhar, porque o defeito deixou de existir. Isso mostra que o teste forte é o que detectava o problema, e a cobertura nunca mudou.

### Mudança 2: ampliar o horário de atendimento

Em `RegraDeAgenda.cs`, troque `FromHours(18)` por `FromHours(19)`. Rode `dotnet test`: os casos de 18:00 passam a falhar nos dois níveis, o de unidade e o de integração.
