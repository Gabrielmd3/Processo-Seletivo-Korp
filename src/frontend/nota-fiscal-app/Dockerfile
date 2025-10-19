# Estágio 1: Build do Angular
FROM node:20 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Estágio 2: Servir com Nginx
FROM nginx:alpine

# AQUI ESTÁ A CORREÇÃO: O caminho correto com a pasta 'browser'
COPY --from=build /app/dist/nota-fiscal-app/browser/ /usr/share/nginx/html

RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d