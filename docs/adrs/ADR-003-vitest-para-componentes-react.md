# ADR-003: Vitest com Testing Library para os componentes React

**Data:** 2026-10-13

**Status:** Aceita

**Decisores:** José Romualdo (professor, disciplina Aplicações Web)

## Contexto

O tópico 05 (`topico-05-react`) reescreve em React o filtro da agenda do
tópico 04. No tópico 04, as funções puras do `agenda.js` são testadas com o
executor nativo do Node (`node --test`), e o `main.js`, que mexe na página,
não tem teste automático: a conferência é feita no navegador.

No tópico 05, o comportamento que importa está nos componentes: escolher
Cardiologia no campo precisa deixar só as consultas dessa especialidade na
tabela e mostrar `2 consulta(s) encontrada(s).`. Testar isso exige três
coisas que o `node --test` sozinho não oferece: converter JSX em JavaScript,
simular uma página (DOM) dentro do Node e disparar eventos no campo como uma
pessoa faria.

O tópico 01 (`topico-01-cliente-servidor/web`) já usa React com Vite, mas não
tem testes de front.

## Decisão

Os testes de componente React do repositório usam Vitest, com
`@testing-library/react` para desenhar e consultar os componentes e jsdom
como página simulada, todos com versão exata no `package.json` e instalados
com `npm ci` na CI.

## Motivações

- **Mesma configuração do Vite:** o Vitest lê o `vite.config.js` do tópico.
  O plugin `@vitejs/plugin-react`, que já converte o JSX para o
  `npm run dev`, converte também o JSX dos testes. Não há um segundo
  conversor (Babel ou outro) para configurar e explicar ao aluno.
- **Testing Library consulta a tela como a pessoa a vê:** o teste encontra
  o campo pelo rótulo (`getByLabelText('Especialidade')`) e o total pelo
  texto (`getByText('2 consulta(s) encontrada(s).')`). O teste não depende
  do nome do estado nem da estrutura interna do componente, e continua
  válido se o componente for reorganizado.
- **jsdom em vez de navegador real:** os oito testes do tópico rodaram em
  cerca de 1,4 segundo em 13/10/2026, sem baixar navegador. Um executor com
  navegador real (Playwright, Cypress) exigiria o download do Chromium em
  cada máquina de aluno e na CI.
- **Versões exatas:** a turma inteira instala o mesmo conjunto. Com `^`, o
  `npm install` de um aluno em semanas diferentes traria versões
  diferentes do Vitest e do jsdom, e uma saída de teste diferente da citada
  na aula. As versões fixadas em 13/10/2026 foram: `react` e `react-dom`
  19.3.0, `vite` 8.3.0 (a mesma do tópico 01), `@vitejs/plugin-react`
  6.1.1, `vitest` 5.0.3, `jsdom` 30.1.1, `@testing-library/react` 16.3.3 e
  `@testing-library/dom` 10.4.2. Em cada pacote, a escolha foi a versão
  publicada há pelo menos uma semana, e não a mais recente do dia.
- **`fireEvent` em vez de `@testing-library/user-event`:** o teste só
  precisa trocar a opção de um `select`. O `fireEvent.change` resolve com
  uma dependência a menos.

## Riscos conhecidos e mitigações

- **Risco:** o jsdom não é um navegador. Layout, CSS e diferenças de
  renderização entre navegadores não aparecem nos testes.
  **Mitigação:** a aula 05 confere a tela no Chrome headless pelo
  Puppeteer, e a captura da agenda filtrada por Cardiologia saiu idêntica,
  pixel a pixel, à do tópico 04.
- **Risco:** o Vite 8 e o jsdom 30 exigem Node 22.12 ou superior, acima do
  Node 18 que os tópicos 03 e 04 pedem.
  **Mitigação:** o README do tópico 05 traz a versão mínima no primeiro
  pré-requisito e o sintoma de Node antigo na seção "Se der errado". A CI
  usa o Node 22.
- **Risco:** a cópia do `agenda.js` do tópico 04 dentro do tópico 05
  divergir com o tempo.
  **Mitigação:** o teste `reaproveitamento.test.js` compara os dois
  arquivos byte a byte e os dados das duas pastas a cada execução.
- **Risco:** versões fixadas acumulam vulnerabilidades conhecidas ao longo
  do semestre.
  **Mitigação:** `npm ci` imprime o resultado do `npm audit`
  (`found 0 vulnerabilities` em 13/10/2026). Uma atualização de versão é
  feita no `package.json` e no `package-lock.json` no mesmo commit, com os
  testes rodando na CI.

## Consequências

Positivas: os componentes React passam a ter teste automático na CI, com a
mesma configuração que o aluno usa para rodar a página. Os testes leem como
o comportamento pedido ("Cardiologia mostra só as 2 consultas dessa
especialidade").

Negativas: o tópico 05 instala 92 pacotes, contra 1 dependência direta do
tópico 04, e exige Node 22.12 ou superior. O repositório passa a ter dois
executores de teste JavaScript, o `node --test` nos tópicos 03 e 04 e o
Vitest no tópico 05.

## ADRs relacionadas

- ADR-001 deste repositório (estrutura do repositório de exemplos), que
  remove do commit o scaffold dos templates.
- ADR-002 deste repositório (numeração das pastas pelo número da aula),
  que dá o nome `topico-05-react` à pasta.
