#!/bin/bash
docker-compose -f docker-compose.yml -f docker-compose.dev.yml -p ca up -d --no-build