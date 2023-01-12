# syntax=docker/dockerfile:experimental

FROM nginx:alpine

COPY nginx.conf /etc/nginx/nginx.conf
COPY ./certbot/www/ /var/www/certbot/
COPY ./certbot/conf/ /var/www/ssl/