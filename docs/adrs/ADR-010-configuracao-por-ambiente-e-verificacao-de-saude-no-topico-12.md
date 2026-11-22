# ADR-010: Configuração por ambiente e verificação de saúde no tópico 12

**Data:** 22/11/2026
**Status:** Aceita
**Decisores:** José Romualdo

## Contexto

A aula 12 trata da implantação em nuvem de forma genérica. A Uninove não tem parceria com provedor, e nada é publicado em conta real. O que vale em qualquer provedor é a imagem do tópico 11, a configuração por variável de ambiente, o segredo fora do código e uma verificação de saúde que a plataforma consulta.

## Decisão

O `topico-12-nuvem` copia a API do tópico 11, acrescenta a rota `GET /health` por `AddHealthChecks` e `MapHealthChecks`, e entrega só a API, um `compose.yaml` configurado por variáveis, um teste de configuração em xUnit, um teste de saúde no container e um checklist de implantação.

## Motivações

- **Só arquivos de configuração e checklist:** o tópico não cria conta nem recurso em provedor. O aluno executa tudo no computador, e o checklist vale para qualquer provedor.
- **`/health` com `MapHealthChecks`:** é o mecanismo padrão do ASP.NET Core, responde 200 com o texto `Healthy` e não passa pelo `[Authorize]` dos controllers. Orquestradores de container e balanceadores usam uma rota assim para decidir se reiniciam ou retiram a instância.
- **Healthcheck do compose com `bash` e `/dev/tcp`:** a imagem do ASP.NET não traz `curl`, e instalar um pacote só para a verificação aumentaria a imagem e a superfície de ataque. O `bash` já existe na imagem.
- **Teste que lê os arquivos versionados:** o csproj de teste copia `appsettings*.json`, `compose.yaml` e `.env.exemplo` para a saída, e o teste procura cinco padrões de credencial. Um teste do detector com textos de exemplo impede que o detector deixe de acusar. A verificação não substitui revisão nem ferramenta de varredura de histórico, e a aula declara esse limite.
- **Variáveis com padrão para o ambiente local:** `${CORS_ORIGEM:-http://localhost:5173}` mantém o `docker compose up` sem arquivo `.env`, e um `.env` real, fora do Git, troca os valores.
- **Sem o front:** o tópico trata do que a API exige de qualquer provedor. O front do tópico 11 já está em container.

## Riscos conhecidos

- **O detector é heurístico:** um segredo sem nome reconhecível, como uma senha dentro de uma frase, passa. Mitigação: o checklist pede revisão do histórico e do pipeline, e a aula cita o limite.
- **`.dockerignore` com `bin/` só vale na raiz do contexto:** uma pasta `obj` local fez o build falhar com `NETSDK1064`. Mitigação: o `.dockerignore` do tópico usa `**/bin/` e `**/obj/`.
- **Cookie `Secure` fora de `localhost`:** continua valendo o risco da ADR-009. Em um provedor, o tráfego público precisa ser HTTPS.
- **As contas de demonstração com senha pública:** existem na primeira subida. Mitigação: o checklist exige removê-las ou trocá-las antes de abrir a API ao público.

## Consequências

Positivas: o aluno vê a mesma imagem mudar de comportamento só pelas variáveis, e a verificação de saúde fica coberta por teste, por script e pelo `docker compose ps`.

Negativas: a API ganha uma sétima cópia; o tópico não mostra nenhum serviço de provedor em execução.

## ADRs relacionadas

ADR-007 (sessão por cookie), ADR-009 (containers com Compose e SQLite em volume).
