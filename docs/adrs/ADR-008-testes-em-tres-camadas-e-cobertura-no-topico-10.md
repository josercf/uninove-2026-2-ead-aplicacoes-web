# ADR-008: Testes em três camadas e cobertura no tópico 10

**Data:** 22/11/2026
**Status:** Aceita
**Decisores:** José Romualdo

## Contexto

A aula 10 trata de testes e depuração de aplicações web. A pirâmide de testes precisa de contagens reais por camada, o front precisa de um teste de componente com Testing Library, e a aula precisa mostrar com medida que cobertura alta não garante ausência de defeito. Os tópicos 07 a 09 deixaram dois fatos que a decisão enfrenta. A API do tópico 09 responde 401 sem login, e o front do tópico 07 não faz login. Além disso, os 48 testes herdados da API usam todos o servidor em memória ou o banco, e nenhum é teste de unidade.

## Decisão

O `topico-10-testes` copia a API do tópico 09 e o front do tópico 07, acrescenta uma regra de domínio sem dependências (`RegraDeAgenda`, horário de atendimento) para ter testes de unidade na API, e mede a cobertura com Vitest (v8) no front e coverlet na API. O 401 é tratado no front com uma mensagem própria e testado com API simulada; o login com a API real é testado no lado da API (`ApiFactory.ClienteAutenticado`).

## Motivações

- **401 tratado no front, sem tela de login:** a tela de login exigiria formulário, estado de sessão e `credentials: 'include'`, e desviaria a aula do tema. O teste de componente com `fetch` simulado já cobre o comportamento que a aula mede, e o par 401/403 na tela é o que o aluno vê ao ligar o front na API real.
- **Regra de horário de atendimento:** é uma regra de negócio pura, que cria a camada de unidade na API (8 casos de borda, sem servidor). Os valores de horário dos testes herdados já estavam entre 08:00 e 18:00 de dias úteis, e os 48 testes continuam verdes.
- **`userEvent` além de `fireEvent`:** o tópico 07 dispara eventos soltos com `fireEvent`; o `userEvent` imita a sequência de uma pessoa. A aula contrasta os dois.
- **`test.fails` para o defeito:** o repositório precisa continuar verde no CI. O teste que reprova a função com defeito está marcado como falha esperada, e o resultado `1 expected fail` documenta o defeito.
- **Cobertura medida sem migrações:** as migrações são código gerado pelo EF Core, e entram no `line-rate` global (98,5%). A aula usa o número sem elas.

## Riscos conhecidos

- **A cobertura mede execução e não verificação:** o experimento de `proximaConsultaComDefeito` mostra 100% de linhas e ramos com defeito vivo. Mitigação: a aula usa esse caso como limite do subtema 4.
- **Teste instável herdado do tópico 09:** `Valor_do_cookie_nao_contem_o_email_nem_o_nome_do_paciente` procurava a sequência "ana" sem diferenciar maiúsculas no valor do cookie (Base64 de 575 caracteres). Em 3000 logins medidos, 48 cookies (1,6%) continham a sequência por acaso, e o teste reprovava sem defeito algum. Mitigação: o teste do tópico 10 procura o e-mail inteiro e o sobrenome. A mesma correção não foi aplicada na pasta do tópico 09.
- **Sem teste de ponta a ponta no navegador:** a pirâmide do exemplo tem 0 testes dessa camada. Mitigação: a aula declara a lacuna.
- **jsdom não é navegador:** os testes de componente rodam em DOM simulado. Mitigação: a depuração no navegador, por ponto de parada, cobre o comportamento real.
- **Quinta cópia da API na porta 5084:** o README avisa.

## Consequências

Positivas: a pirâmide tem contagens reais por camada (unidade, componente e integração), o defeito de cobertura é reproduzível por comando, e a aula mostra um teste instável de verdade.

Negativas: a API ganha uma regra de negócio que os tópicos anteriores não têm; o front do exemplo não faz login; a pasta duplica 3 conjuntos de código dos tópicos anteriores.

## ADRs relacionadas

ADR-003 (Vitest para componentes React), ADR-005 (API como cópia com CORS), ADR-006 (SQLite com EF Core), ADR-007 (sessão por cookie e hash de senha).
