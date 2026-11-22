# ADR-009: Containers com Compose e SQLite em volume no tópico 11

**Data:** 22/11/2026
**Status:** Aceita
**Decisores:** José Romualdo

## Contexto

A aula 11 trata de aplicações web em containers. O exemplo precisa de uma imagem por `Dockerfile` comentado, de um build em estágios com tamanho medido, de um `docker compose` com API, front e banco e de uma demonstração do que o container não isola. A API vem do tópico 10 (e do tópico 09), que usa SQLite com migrações do EF Core. O front vem do tópico 07, e o tópico 10 acrescentou o tratamento de 401 e 403.

## Decisão

O `topico-11-containers` copia a API do tópico 10, sem os testes, e o front do tópico 10, sem os testes e sem o Vitest. O banco continua sendo o arquivo SQLite, guardado em um volume nomeado do Docker montado em `/dados`. O compose declara dois serviços (`api` e `web`) e um volume (`dados`).

## Motivações

- **Versão do tópico 10 em vez da do tópico 09:** a API do tópico 10 traz a regra de horário de atendimento e o front trata 401 e 403 com mensagem própria. O teste de fumaça usa os dois comportamentos (400 no sábado e 401 sem login).
- **SQLite em volume em vez de um servidor de banco no compose:** as migrações do EF Core do tópico 08 em diante são geradas para SQLite, e trocar de banco exigiria outro provedor, outras migrações e retestar as 60 verificações da API. O volume mostra o ponto da aula, que é separar o dado do container: `docker compose down` remove o container e o `clinica-vida_dados` fica. A aula declara que o banco, neste exemplo, é um arquivo e não um serviço.
- **Front servido por nginx na porta 5173:** a política de CORS do tópico 10 aceita a origem `http://localhost:5173`. Servir o front nessa porta mantém a origem e evita mexer na política. O navegador do aluno chama a API em `http://localhost:5084`, publicada no computador, e por isso `api.js` não muda.
- **Origem do CORS configurável:** uma única linha do `Program.cs` passou a ler `Cors:Origem`, com o padrão antigo. A imagem fica independente do endereço do front.
- **Usuário `app` e pasta `/dados` com dono `app`:** a imagem oficial do ASP.NET 10 traz o usuário `app` (UID 1654). O processo roda sem privilégios, e a pasta do volume recebe o mesmo dono na construção da imagem.
- **Imagens `sdk:10.0` e `aspnet:10.0`:** correspondem ao `TargetFramework` `net10.0`. Dentro da imagem não é preciso `DOTNET_ROLL_FORWARD`, que só os testes locais em máquina sem .NET 10 usam.
- **Dois `Dockerfile` de estágio único:** existem apenas para medir o tamanho antes do build em estágios, e o `compose.yaml` não os usa.

## Riscos conhecidos

- **Banco em arquivo não escala para várias réplicas da API:** duas instâncias escrevendo no mesmo arquivo SQLite se bloqueiam. Mitigação: o exemplo sobe uma instância, e a aula declara o limite.
- **Cookie `Secure` sobre HTTP:** a API emite `Secure` (tópico 09) e o compose serve HTTP. O `curl` e os navegadores aceitam o cookie para `localhost`; fora de `localhost`, o cookie não seria enviado. Mitigação: o README e a aula limitam o exemplo a `localhost`.
- **A medição do isolamento depende do sistema:** no macOS e no Windows, o Docker Desktop executa os containers em uma máquina virtual Linux, e `uname -r` fora do container mostra o kernel do macOS. No Linux o kernel de dentro e o de fora são o mesmo. Mitigação: a aula registra o sistema da medição.
- **Tamanhos de imagem mudam com as versões das imagens base:** os números medidos são de 22/11/2026, com `dotnet/aspnet:10.0` (10.0.15) e `nginx:alpine`.
- **Sem verificação de saúde (`healthcheck`):** a imagem do ASP.NET não traz `curl`. Mitigação: o teste de fumaça espera até 30 segundos pela resposta.

## Consequências

Positivas: um único `docker compose up` entrega o sistema inteiro, as imagens finais ficam 4,4 vezes (API, de 2,05 GB para 462 MB) e 3,5 vezes (front, de 330 MB para 93,5 MB) menores que as de estágio único, e o banco sobrevive à remoção dos containers.

Negativas: a API do tópico 10 ganha uma cópia (sexta cópia, a porta 5084 continua a mesma dos tópicos anteriores); o banco não é um serviço separado; o exemplo exige Docker instalado.

## ADRs relacionadas

ADR-006 (SQLite com EF Core), ADR-007 (sessão por cookie), ADR-008 (testes e cobertura no tópico 10).
