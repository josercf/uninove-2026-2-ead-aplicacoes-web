# ADR-002: Numeração das pastas pelo número da aula

**Data:** 2026-10-13

**Status:** Aceita

**Decisores:** José Romualdo (professor, disciplina Aplicações Web)

## Contexto

Os dois repositórios de exemplos da turma numeravam as pastas de formas
diferentes. No repositório `uninove-2026-2-ead-modelagem-oo`, a pasta
`topico-02-classes-e-objetos` corresponde ao arquivo de aula `aula_02`. Neste
repositório, a pasta `topico-02-cliente-servidor` correspondia ao arquivo
`aula_01`, porque o número seguia a posição do tópico no AVA (o segundo
tópico, depois da apresentação da disciplina). O aluno que cursa as duas
disciplinas encontraria duas convenções para o mesmo tipo de pasta.

Nenhuma aula de Aplicações Web além da aula 00 (apresentação) foi publicada
até esta data, de modo que a renomeação ainda não invalida nenhum link
distribuído.

## Decisão

As pastas `topico-NN-*` usam o número do arquivo de aula `aula_NN`, nos dois
repositórios.

## Motivações

- **Uma convenção para as duas disciplinas:** o aluno e o professor localizam
  a pasta de uma aula pelo mesmo número do arquivo `aula_NN`, sem consultar
  uma tabela de correspondência.
- **Custo mínimo agora:** a renomeação é uma operação de `git mv` mais a
  substituição do nome em README, CI e ADR-001, feita antes da publicação das
  aulas 01 em diante.
- **Referências estáveis nas aulas:** as aulas citam `topico-NN-*` com o mesmo
  número do próprio arquivo, o que simplifica a revisão e a verificação
  automática.

## Riscos conhecidos e mitigações

- **Risco:** links antigos para `topico-02-cliente-servidor` em material já
  distribuído deixam de funcionar.
  **Mitigação:** nenhuma aula de Aplicações Web além da aula 00 foi publicada,
  e a aula 00 é corrigida na mesma entrega.
- **Risco:** o nome antigo permanecer em algum arquivo do repositório.
  **Mitigação:** busca textual por `topico-02` nos arquivos versionados após a
  renomeação, e execução da CI no mesmo commit.

## Consequências

Positivas: a pasta de cada aula é previsível a partir do número do arquivo, e
os dois repositórios seguem a mesma regra.

Negativas: o ADR-001 deste repositório e o histórico do Git ainda mencionam o
nome antigo em commits anteriores; o texto do ADR-001 foi atualizado para o
nome novo, e o histórico permanece como está.

## ADRs relacionadas

- ADR-001 deste repositório (estrutura do repositório de exemplos), cujos
  caminhos foram atualizados para `topico-01-cliente-servidor`.
