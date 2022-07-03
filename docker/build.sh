#!/bin/bash

export EXTERNAL_HOST_NAME=$1
mkdir -p ./certs
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout ./certs/server.key -out ./certs/server.crt -config ca.cnf -subj /CN=$1

docker-compose -f docker-compose.yml -f docker-compose.dev.yml build --no-cache
docker rmi $(docker images --filter "dangling=true" -q --no-trunc) --force
