#!/bin/bash

docker-compose -f docker-compose.yml -f docker-compose.dev.yml build --no-cache
docker rmi $(docker images --filter "dangling=true" -q --no-trunc) --force
