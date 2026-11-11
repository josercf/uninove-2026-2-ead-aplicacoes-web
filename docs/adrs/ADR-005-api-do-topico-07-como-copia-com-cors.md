# ADR-005: API do tópico 07 como cópia do tópico 06, com CORS

**Data:** 11/11/2026
**Status:** Aceita
**Decisores:** José Romualdo

## Contexto

O front do tópico 07 roda em `http://localhost:5173` e busca dados em `http://localhost:5084`. São origens diferentes, e o navegador só entrega a resposta ao JavaScript se a API enviar `Access-Control-Allow-Origin`. A API do tópico 06 não envia. A aula 06, já publicada, cita o `topico-06-rest` com saídas e contagem de testes (12 testes).

## Decisão

A API vai para `topico-07-fetch/api`, como cópia do `topico-06-rest/api` com CORS liberado só para `http://localhost:5173`. O `topico-06-rest` não muda.

## Motivações

- A saída citada na aula 06 (`Passed:    12`, as respostas de `curl`) continua verdadeira.
- A aula 07 mostra primeiro a requisição bloqueada e depois o cabeçalho que libera, o que exige a API sem CORS no tópico 06 e com CORS no tópico 07.
- Cada pasta de tópico roda sozinha, sem depender de outra (ADR-001).

## Riscos conhecidos

- Duas cópias do mesmo código divergem com o tempo. Mitigação: o tópico 06 está fechado e a aula 08 parte da cópia do tópico 07.
- Os dois projetos usam a porta 5084. Mitigação: o README avisa para encerrar um antes de subir o outro.

## Consequências

- Positivas: o tópico 06 permanece idêntico ao citado na aula; o teste de CORS (`CorsTests`) fica no mesmo projeto que o `Program.cs` alterado.
- Negativas: duplicação de cerca de vinte arquivos; a política usa `AllowAnyHeader` e `AllowAnyMethod`, aceitável só para a origem de desenvolvimento.

## ADRs relacionadas

ADR-001 (estrutura do repositório), ADR-002 (numeração das pastas), ADR-004 (repositório em memória atrás de interface).
