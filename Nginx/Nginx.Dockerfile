# syntax=docker/dockerfile:experimental

FROM nginx:alpine

COPY nginx.conf /etc/nginx/nginx.conf
COPY cert.crt /etc/ssl/certs/cert.crt
COPY private.key /etc/ssl/private/private.key

