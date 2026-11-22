# Checklist de implantação da Clínica Vida+

Este checklist vale para qualquer provedor de nuvem. Ele não depende de serviço específico, e nada neste tópico é publicado em conta real. Cada item traz como conferir o resultado.

## 1. Imagem

| Item | Como conferir | Resultado esperado |
|---|---|---|
| A imagem é construída pelo `Dockerfile` em dois estágios, a mesma do tópico 11 | `docker compose build` | Termina sem erro |
| O processo roda como usuário sem privilégios | `docker compose run --rm --entrypoint id api` | `uid=1654(app)` |
| A imagem não traz o banco nem arquivos `.env` | `.dockerignore` lista `*.db` e a pasta não tem `.env` | Nenhum arquivo de dados dentro da imagem |
| A imagem tem uma etiqueta de versão, não só `latest` | `docker tag` com a versão ou o hash do commit | O provedor puxa sempre a mesma imagem para a mesma versão |

## 2. Configuração por variável de ambiente

| Item | Como conferir | Resultado esperado |
|---|---|---|
| Nenhum valor que muda por ambiente está no código nem no `appsettings.json` versionado | `dotnet test`, classe `SegredoVersionadoTests` | Todos os testes passam |
| `Cors__Origem` recebe o endereço público do front | `./demonstra-variaveis.sh` | O cabeçalho `Access-Control-Allow-Origin` traz o endereço configurado |
| `ConnectionStrings__Clinica` aponta para o banco do ambiente | `docker compose exec api printenv ConnectionStrings__Clinica` | O valor do ambiente, sem relação com o da máquina do desenvolvedor |
| `ASPNETCORE_ENVIRONMENT` é `Production` | `docker compose config` | A linha `ASPNETCORE_ENVIRONMENT: Production` |

## 3. Segredo fora do código

| Item | Como conferir | Resultado esperado |
|---|---|---|
| Senha, chave e token ficam no cofre de segredos do provedor ou em variável protegida do pipeline | Revisar o pipeline e o painel do provedor | Nenhum segredo em arquivo versionado |
| O arquivo `.env` está no `.gitignore` | `git check-ignore topico-12-nuvem/.env` | Imprime o caminho do `.env` |
| O histórico do Git não contém segredo antigo | Buscar no histórico por `Password=`, `secret`, `token` | Nenhum resultado; se houver, o segredo é trocado, não apenas apagado |
| A senha de demonstração `Senha@123` não existe em produção | Conferir as contas do ambiente | As contas de demonstração são removidas ou trocadas antes de abrir ao público |

## 4. Verificação de saúde

| Item | Como conferir | Resultado esperado |
|---|---|---|
| `GET /health` responde 200 sem login | `curl -i http://localhost:5084/health` | `HTTP/1.1 200 OK` e corpo `Healthy` |
| A rota de negócio continua protegida | `curl -i http://localhost:5084/api/consultas` | `HTTP/1.1 401 Unauthorized` |
| O orquestrador usa `/health` para decidir se reinicia o container | `docker compose ps` | `(healthy)` ao lado do serviço |
| O teste de saúde passa no container local antes de qualquer envio | `./teste-de-saude.sh` | Cinco linhas `ok` e saída de código 0 |

## 5. Dados e persistência

| Item | Como conferir | Resultado esperado |
|---|---|---|
| O banco fica fora do container | `docker volume ls` | O volume `clinica-vida-nuvem_dados` existe |
| Existe cópia de segurança do banco e já foi restaurada ao menos uma vez | Restaurar a cópia em um ambiente de teste | A lista de consultas volta com os mesmos registros |
| O banco do exemplo é um arquivo SQLite, então a API roda em uma instância só | Revisar o número de réplicas do serviço | Uma réplica; para várias, troca por banco gerenciado |

## 6. Rede e acesso

| Item | Como conferir | Resultado esperado |
|---|---|---|
| O tráfego público chega por HTTPS | Abrir o endereço público | O cookie `clinica.sessao` é aceito, porque é `Secure` |
| Só a porta da API (8080 no container) está exposta | Revisar `ports` e as regras de entrada do provedor | Nenhuma outra porta aberta |
| O CORS lista só a origem do front, sem `*` | `./demonstra-variaveis.sh` | Origem diferente da configurada não recebe o cabeçalho |

## 7. Custo e saída

| Item | Como conferir | Resultado esperado |
|---|---|---|
| Há um limite de gasto ou alerta de orçamento na conta | Painel de cobrança do provedor | Alerta ativo antes do primeiro recurso criado |
| Cada recurso criado tem dono e data para remoção | Etiquetas (tags) nos recursos | Nenhum recurso sem etiqueta |
| A saída está descrita: como exportar o banco e a imagem para outro provedor | Executar a exportação uma vez | Imagem e dados reconstruídos fora do provedor original |
