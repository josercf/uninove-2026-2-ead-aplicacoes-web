# Tópico 11: aplicações web em containers

Este exemplo empacota a Clínica Vida+ em containers. A API é a do tópico 10 (ASP.NET Core, SQLite, sessão por cookie e regra de horário). O front é o React do tópico 07 e do tópico 10, compilado com o Vite e entregue pelo nginx. O banco é o arquivo SQLite da API, guardado em um volume do Docker.

## O que há na pasta

| Peça | Arquivo | O que faz |
|---|---|---|
| Imagem da API | `api/Dockerfile` | Build em dois estágios: SDK do .NET 10 compila, runtime do ASP.NET 10 executa, com usuário `app` sem privilégios |
| Imagem da API, estágio único | `api/Dockerfile.estagio-unico` | Mesma API em um só estágio, só para medir o tamanho |
| Imagem do front | `web/Dockerfile` | Build em dois estágios: Node 22 gera `dist`, nginx entrega os arquivos |
| Imagem do front, estágio único | `web/Dockerfile.estagio-unico` | Mesmo front em um só estágio, com `vite preview` |
| Orquestração | `compose.yaml` | API na porta 5084, front na 5173 e o volume `dados` com o banco |
| Teste de fumaça | `teste-de-fumaca.sh` | 8 verificações com `curl` sobre os containers no ar |
| Requisições | `requisicoes/` | Login da Ana e consulta no sábado, as mesmas do tópico 10 |

A API é cópia da pasta `topico-10-testes/api` sem os testes. Uma única linha de código mudou: o `Program.cs` lê a origem do CORS da configuração `Cors:Origem` (padrão `http://localhost:5173`), para o compose poder trocá-la sem recompilar a imagem. A decisão de manter SQLite em volume está em `docs/adrs/ADR-009-containers-com-compose-e-sqlite-em-volume.md`.

## Pré-requisitos

1. Docker com Compose v2, conferido com `docker compose version` (resultado esperado: uma linha `Docker Compose version v2...`).
2. `curl` e `bash`, para o teste de fumaça.
3. As portas 5084 e 5173 livres no computador.

Não é preciso instalar .NET nem Node: os dois compiladores rodam dentro do build da imagem.

## Como executar

### Passo 1: Construir e subir

A partir da pasta `topico-11-containers`:

```bash
docker compose up -d --build
```

Resultado esperado: as linhas `Container clinica-vida-api-1 Started` e `Container clinica-vida-web-1 Started`. O primeiro build baixa as imagens base e leva alguns minutos.

### Passo 2: Conferir os containers

```bash
docker compose ps
```

Resultado esperado: duas linhas com `Up`, a API com `0.0.0.0:5084->8080/tcp` e o front com `0.0.0.0:5173->80/tcp`.

### Passo 3: Rodar o teste de fumaça

```bash
./teste-de-fumaca.sh
```

Resultado esperado: oito linhas começando por `ok` e a última `Teste de fumaça: todas as verificações passaram.`, com saída de código 0. A API leva alguns segundos para aplicar as migrações na primeira subida, e o script espera até 30 segundos.

### Passo 4: Conferir que o banco sobrevive ao container

1. Entrar com a Ana e criar uma consulta na segunda-feira:

   ```bash
   curl -s -c cookies.txt -H 'Content-Type: application/json' -d @requisicoes/login-ana.json http://localhost:5084/api/auth/login -o /dev/null
   curl -s -b cookies.txt -H 'Content-Type: application/json' -d '{"paciente":"Ana Souza","especialidade":"Cardiologia","dataHora":"2026-10-12T09:00:00"}' http://localhost:5084/api/consultas
   ```

   Resultado esperado: o corpo `{"id":4,"paciente":"Ana Souza","especialidade":"Cardiologia","dataHora":"2026-10-12T09:00:00"}`.

2. Remover os containers sem remover o volume e subir de novo:

   ```bash
   docker compose down
   docker compose up -d
   ```

   Resultado esperado: `docker volume ls` ainda lista `clinica-vida_dados`.

3. Esperar cerca de 10 segundos, entrar de novo e listar:

   ```bash
   curl -s -c cookies.txt -H 'Content-Type: application/json' -d @requisicoes/login-ana.json http://localhost:5084/api/auth/login -o /dev/null
   curl -s -b cookies.txt http://localhost:5084/api/consultas
   ```

   Resultado esperado: a lista tem 4 consultas, com a de `id` 4 criada no passo 1.

### Passo 5: Medir o tamanho das imagens

```bash
docker build -f api/Dockerfile.estagio-unico -t clinica-api:estagio-unico api
docker build -f web/Dockerfile.estagio-unico -t clinica-web:estagio-unico web
docker image ls --filter 'reference=clinica*'
```

Resultado esperado, medido com Docker Desktop 29 (coluna `DISK USAGE` é o tamanho descompactado no disco e `CONTENT SIZE` é o tamanho comprimido do conteúdo):

```text
IMAGE                       ID             DISK USAGE   CONTENT SIZE
clinica-api:estagio-unico   1657421a4b96       2.05GB          606MB
clinica-vida-api:latest     6081349f798d        462MB          119MB
clinica-vida-web:latest     0c7c7f1b37da       93.5MB         26.4MB
clinica-web:estagio-unico   c0e2cd6a6a1a        330MB         92.2MB
```

Os identificadores mudam a cada build; os tamanhos variam pouco entre versões de imagem base.

### Passo 6: Ver o que o container não isola

```bash
uname -sr
docker run --rm alpine uname -sr
docker run --rm --memory=64m --cpus=0.5 alpine sh -c 'head -1 /proc/meminfo; cat /sys/fs/cgroup/memory.max; cat /sys/fs/cgroup/cpu.max; nproc'
docker run --rm --memory=64m --memory-swap=64m alpine sh -c 'head -c 200000000 /dev/zero | tr "\0" a | tail -n 1 | wc -c'; echo "exit=$?"
```

Resultado esperado, medido no macOS com Docker Desktop:

```text
Darwin 27.3.0
Linux 7.0.14-linuxkit
MemTotal:        2017836 kB
67108864
50000 100000
4
exit=137
```

O container mostra o kernel da máquina virtual do Docker Desktop (`linuxkit`), e dois containers diferentes mostram o mesmo kernel. Com `--memory=64m`, o `memory.max` é 67108864, mas `/proc/meminfo` e `nproc` continuam mostrando a máquina virtual inteira (2017836 kB e 4 CPUs). O processo que tenta alocar 200 MB sob esse limite termina com código 137 (SIGKILL pelo limite de memória).

### Passo 7: Parar e remover tudo

```bash
docker compose down -v
```

Resultado esperado: os containers, a rede e o volume `clinica-vida_dados` aparecem como `Removed`. Sem `-v`, o volume (e o banco) permanece.

## Se der errado

### Erro 1: `port is already allocated`

Outro programa usa a 5084 ou a 5173, por exemplo a API de outro tópico. Encerre-o ou troque a porta da esquerda em `compose.yaml` (por exemplo `"5085:8080"`). Se trocar a porta do front, troque também `Cors__Origem`.

### Erro 2: o teste de fumaça falha na linha do CORS

A origem do front mudou. Rode com `WEB=http://localhost:PORTA ./teste-de-fumaca.sh` e ajuste `Cors__Origem` no `compose.yaml` para o mesmo valor.

### Erro 3: `permission denied while trying to connect to the Docker daemon socket`

O usuário não tem acesso ao Docker. No Linux, entre no grupo `docker` ou use `sudo`.

### Erro 4: a API responde `unable to open database file`

O volume foi criado por uma versão anterior da imagem, com dono diferente. Rode `docker compose down -v` e suba de novo.

## Experimente mudar

### Mudança 1: remover o volume

Rode `docker compose down -v`, suba de novo e liste as consultas. A lista volta a ter 3 consultas, porque o banco novo nasce com os dados iniciais.

### Mudança 2: ver o cache de camadas

Altere um comentário em `api/ClinicaVida.Api/Controllers/ConsultasController.cs` e rode `docker compose build api`. Os passos `restore` aparecem como `CACHED`, e só a compilação e as camadas seguintes são refeitas.

### Mudança 3: limitar a memória da API

Em `compose.yaml`, acrescente `mem_limit: 64m` ao serviço `api` e suba de novo. Compare `docker stats --no-stream` antes e depois.
