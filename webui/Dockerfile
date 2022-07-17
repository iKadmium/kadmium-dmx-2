# syntax=docker/dockerfile:1
FROM node:latest AS build-env
WORKDIR /app/

COPY ./package.json ./

RUN npm ci

# Copy everything else and build
COPY ./* ./
RUN npm run build

# Build runtime image
FROM nginx:latest
COPY --from=build-env /app/build/ /usr/share/nginx/html