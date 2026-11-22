# syntax=docker/dockerfile:1
# Estágio 1, "construir": imagem do Node, que roda o Vite e gera os arquivos estáticos.
FROM node:22-alpine AS construir
WORKDIR /origem
# Copia só os arquivos de dependências, para o npm ci ter camada própria no cache.
COPY package.json package-lock.json ./
# Instala exatamente as versões do package-lock.json.
RUN npm ci
# Copia o restante do front e gera a pasta dist.
COPY . .
RUN npm run build

# Estágio 2, "final": o nginx entrega os arquivos de dist; Node e node_modules ficam para trás.
FROM nginx:alpine AS final
# Substitui a página padrão do nginx pelo resultado do build.
COPY --from=construir /origem/dist /usr/share/nginx/html
EXPOSE 80
