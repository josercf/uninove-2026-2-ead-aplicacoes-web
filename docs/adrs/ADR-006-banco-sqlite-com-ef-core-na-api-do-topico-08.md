# ADR-006: Banco SQLite com EF Core na API do tópico 08

**Data:** 11/11/2026
**Status:** Aceita
**Decisores:** José Romualdo

## Contexto

A aula 06 anunciou que um tópico futuro troca o `ConsultaRepositorioEmMemoria` por banco de dados atrás da mesma interface (ADR-004). A aula 06 cita o `topico-06-rest` com 12 testes, e a aula 07 cita o `topico-07-fetch` com 16, de modo que nenhum dos dois pode mudar. O tópico 08 precisa de um modelo com mais de uma entidade, chave primária, chave estrangeira e restrição, e cada aluno precisa rodá-lo sem instalar um servidor de banco.

## Decisão

O `topico-08-persistencia` copia a API do `topico-07-fetch` (com CORS) e troca o repositório em memória por `ConsultaRepositorioEf`, que usa EF Core com SQLite. A interface `IConsultaRepositorio`, o controller e o contrato JSON permanecem iguais. O modelo tem três tabelas: Pacientes, Especialidades e Consultas.

## Motivações

- **SQLite:** o banco é um arquivo, sem servidor e sem credenciais. Os testes usam SQLite em memória, o mesmo mecanismo de restrições do arquivo.
- **Três tabelas:** o paciente e a especialidade que se repetiam como texto em cada consulta passam a ser linhas referenciadas por chave estrangeira. "Cardiologia" aparece uma vez na tabela de especialidades. O JSON continua com `paciente` e `especialidade` em texto, porque o front do tópico 07 depende desses nomes.
- **Sem a entidade Médico:** o case do semestre não tem médico em nenhum tópico anterior, e acrescentar a entidade exigiria um campo novo no contrato JSON.
- **Restrições no banco:** nome único em Pacientes e em Especialidades, e par (paciente, data e hora) único em Consultas. A chave estrangeira usa `ON DELETE RESTRICT`.
- **Migração em vez de `EnsureCreated`:** a migração `Inicial` registra o esquema e os dados iniciais, e o `Program.cs` a aplica ao subir.
- **Cópia da API:** mesma razão da ADR-005. Os tópicos 06 e 07 continuam idênticos ao que as aulas citam.

## Riscos conhecidos

- **Corrida na criação de paciente:** duas requisições com o mesmo nome novo podem passar pelo SELECT juntas, e o índice único recusa a segunda. Mitigação: o repositório traduz a violação de unicidade em `ConflitoDeAgendaException`, e a API responde 409. A aula declara o limite.
- **Três cópias da API:** os tópicos 06, 07 e 08 divergem se alguém corrigir uma só. Mitigação: os dois primeiros estão fechados.
- **Porta 5084 compartilhada:** o README avisa para encerrar a outra API antes.
- **SQLite não representa um banco de produção:** não há controle de acesso nem concorrência de escrita comparável a um servidor. Mitigação: a aula declara o limite, e a troca de provedor afeta o `Program.cs` e a migração, não o controller.

## Consequências

Positivas: os dados sobrevivem ao reinício da API; o modelo tem chave, relacionamento e restrições que os testes exercitam; o controller não muda.

Negativas: o repositório passa de uma lista com `lock` para um serviço com escopo por requisição, e a tradução de texto em linhas de apoio acrescenta uma consulta por campo a cada `POST` e `PUT`.

## ADRs relacionadas

ADR-001 (estrutura do repositório), ADR-002 (numeração das pastas), ADR-004 (repositório em memória atrás de interface), ADR-005 (API do tópico 07 como cópia com CORS).
