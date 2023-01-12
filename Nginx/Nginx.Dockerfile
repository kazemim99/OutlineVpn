# syntax=docker/dockerfile:experimental

FROM nginx:alpine

COPY nginx.conf /etc/nginx/nginx.conf
COPY private.key cert.crt /etc/nginx/ssl/
