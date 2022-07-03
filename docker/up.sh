#!/bin/bash
export EXTERNAL_HOST_NAME=$1
docker-compose -f docker-compose.yml -f docker-compose.dev.yml -p ca up -d --no-build