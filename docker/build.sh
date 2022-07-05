#!/bin/bash

docker-compose -f docker-compose.yaml -f docker-compose.dev.yaml build --no-cache
docker rmi $(docker images --filter "dangling=true" -q --no-trunc) --force
