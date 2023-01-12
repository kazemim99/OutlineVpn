# syntax=docker/dockerfile:experimental

FROM alpine:3.8 AS generate
WORKDIR /certificates
RUN apk update && \
	apk add --no-cache openssl && \
	rm -rf /var/cache/apk/*
RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout iranv2ray.key -out iranv2ray.crt -subj "/C=GB"


FROM nginx:alpine

COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 443
COPY --from=generate /certificates/iranv2ray.key /certificates/iranv2ray.crt /etc/nginx/ssl/
