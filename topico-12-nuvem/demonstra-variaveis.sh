#!/usr/bin/env bash
# Demonstra a configuração por variável de ambiente: a mesma imagem, dois comportamentos.
# Uso: ./demonstra-variaveis.sh   (a partir de topico-12-nuvem, com Docker em execução)
set -eu
API="http://localhost:5084"

# mostra: imprime as variáveis de configuração vistas de dentro do container e o cabeçalho CORS devolvido.
mostra() {
  docker compose exec -T api printenv Cors__Origem ConnectionStrings__Clinica
  echo "--- Origin enviada: $1"
  curl -s -D - -o /dev/null -H "Origin: $1" "$API/health" | tr -d '\r' | grep -i '^access-control-allow-origin' || echo "(sem Access-Control-Allow-Origin)"
}

echo "=== 1. Valores padrão (sem .env) ==="
docker compose up -d --wait >/dev/null
mostra "http://localhost:5173"

echo
echo "=== 2. Mesma imagem, outras variáveis ==="
CORS_ORIGEM="https://clinica.exemplo.org" CONEXAO_CLINICA="Data Source=/dados/outro.db" docker compose up -d --wait >/dev/null
mostra "https://clinica.exemplo.org"
mostra "http://localhost:5173"
echo "--- Arquivos de banco no volume:"
docker compose exec -T api ls /dados | grep -v -e '-shm' -e '-wal'

echo
echo "=== 3. Configuração resolvida pelo compose, sem subir nada ==="
CORS_ORIGEM="https://clinica.exemplo.org" docker compose config | grep -E 'Cors__Origem|ConnectionStrings__Clinica|ASPNETCORE_ENVIRONMENT'
