#!/usr/bin/env bash
# Teste de saúde do tópico 12: pergunta ao container local se a API está viva e configurada.
# Uso: docker compose up -d --build && ./teste-de-saude.sh
set -u

API="${API:-http://localhost:5084}"
ORIGEM="${CORS_ORIGEM:-http://localhost:5173}"
falhas=0

# espera: tenta por até 30 segundos até a URL responder, porque o container leva alguns segundos para subir.
espera() {
  for _ in $(seq 1 30); do
    if curl -s -o /dev/null "$1"; then return 0; fi
    sleep 1
  done
  echo "FALHA: $1 não respondeu em 30 segundos"
  return 1
}

# confere: compara o código HTTP recebido com o esperado e imprime a linha do resultado.
confere() {
  local rotulo="$1" esperado="$2" recebido="$3"
  if [ "$esperado" = "$recebido" ]; then
    echo "ok    $rotulo: HTTP $recebido"
  else
    echo "FALHA $rotulo: esperado HTTP $esperado, recebido HTTP $recebido"
    falhas=$((falhas + 1))
  fi
}

espera "$API/health" || exit 1

# 1. /health responde 200 sem login e o corpo é Healthy.
confere "GET /health" 200 "$(curl -s -o /dev/null -w '%{http_code}' "$API/health")"
corpo="$(curl -s "$API/health")"
if [ "$corpo" = "Healthy" ]; then echo "ok    corpo de /health: $corpo"; else echo "FALHA corpo de /health: '$corpo'"; falhas=$((falhas + 1)); fi

# 2. A rota de negócio continua protegida: sem login, 401.
confere "GET /api/consultas sem login" 401 "$(curl -s -o /dev/null -w '%{http_code}' "$API/api/consultas")"

# 3. O CORS aceita a origem configurada na variável de ambiente Cors__Origem.
cabecalho="$(curl -s -D - -o /dev/null -H "Origin: $ORIGEM" "$API/api/consultas" | tr -d '\r' | grep -i '^access-control-allow-origin' || true)"
if [ "$cabecalho" = "Access-Control-Allow-Origin: $ORIGEM" ]; then echo "ok    $cabecalho"; else echo "FALHA cabeçalho CORS recebido: '$cabecalho'"; falhas=$((falhas + 1)); fi

# 4. A verificação de saúde do próprio compose também marca o container como healthy.
estado="$(docker compose ps api --format '{{.Status}}' 2>/dev/null)"
case "$estado" in
  *"(healthy)"*) echo "ok    docker compose ps: $estado" ;;
  *) echo "FALHA docker compose ps: '$estado'"; falhas=$((falhas + 1)) ;;
esac

if [ "$falhas" -eq 0 ]; then echo "Teste de saúde: todas as verificações passaram."; else echo "Teste de saúde: $falhas falha(s)."; fi
exit "$falhas"
