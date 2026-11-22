#!/usr/bin/env bash
# Teste de fumaça do tópico 11: sobe nada, só pergunta aos containers que já estão no ar.
# Uso: docker compose up -d --build && ./teste-de-fumaca.sh
# Cada verificação imprime a linha de status literal que o curl recebeu e falha o script se não for a esperada.
set -u

API="${API:-http://localhost:5084}"
WEB="${WEB:-http://localhost:5173}"
JAR="$(mktemp)"
trap 'rm -f "$JAR"' EXIT
falhas=0

# espera: tenta por até 30 segundos até a URL responder qualquer status, porque o container leva alguns segundos para subir.
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

espera "$WEB/" || exit 1
espera "$API/api/consultas" || exit 1

# 1. O front responde 200 e entrega a página que contém o ponto de montagem do React.
confere "front, GET /" 200 "$(curl -s -o /dev/null -w '%{http_code}' "$WEB/")"
if curl -s "$WEB/" | grep -q 'id="agenda-do-dia"'; then
  echo "ok    front, a página traz a seção agenda-do-dia"
else
  echo "FALHA front, a página não traz a seção agenda-do-dia"
  falhas=$((falhas + 1))
fi

# 2. A API sem login responde 401, a regra do tópico 09.
confere "api sem login, GET /api/consultas" 401 "$(curl -s -o /dev/null -w '%{http_code}' "$API/api/consultas")"

# 3. O login da Ana responde 200 e entrega o cookie de sessão.
confere "api, POST /api/auth/login" 200 "$(curl -s -o /dev/null -w '%{http_code}' -c "$JAR" \
  -H 'Content-Type: application/json' -d @requisicoes/login-ana.json "$API/api/auth/login")"
if grep -q 'clinica.sessao' "$JAR"; then
  echo "ok    api, o cookie clinica.sessao foi emitido"
else
  echo "FALHA api, nenhum cookie clinica.sessao"
  falhas=$((falhas + 1))
fi

# 4. Com o cookie, a lista de consultas vem do banco: 200.
confere "api com login, GET /api/consultas" 200 "$(curl -s -o /dev/null -w '%{http_code}' -b "$JAR" "$API/api/consultas")"

# 5. A regra de horário do tópico 10 continua valendo dentro do container: sábado vira 400.
confere "api, POST de consulta no sábado" 400 "$(curl -s -o /dev/null -w '%{http_code}' -b "$JAR" \
  -H 'Content-Type: application/json' -d @requisicoes/consulta-no-sabado.json "$API/api/consultas")"

# 6. O CORS aceita a origem do front servido pelo container.
cabecalho="$(curl -s -D - -o /dev/null -H "Origin: $WEB" "$API/api/consultas" | tr -d '\r' | grep -i '^access-control-allow-origin' || true)"
if [ "$cabecalho" = "Access-Control-Allow-Origin: $WEB" ]; then
  echo "ok    api, $cabecalho"
else
  echo "FALHA api, cabeçalho CORS recebido: '$cabecalho'"
  falhas=$((falhas + 1))
fi

if [ "$falhas" -eq 0 ]; then echo "Teste de fumaça: todas as verificações passaram."; else echo "Teste de fumaça: $falhas falha(s)."; fi
exit "$falhas"
