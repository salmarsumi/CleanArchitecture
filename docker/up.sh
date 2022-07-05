#!/bin/bash
export EXTERNAL_HOST_NAME=$1
docker-compose -f docker-compose.yaml -f docker-compose.dev.yaml -p ca up -d --no-build