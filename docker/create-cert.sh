#!/bin/bash

export EXTERNAL_HOST_NAME_OR_IP=$1 
mkdir -p ./certs
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout ./certs/server.key -out ./certs/server.crt -config ca.cnf -subj /CN=$1

#sudo cp ./certs/erp.crt /usr/local/share/ca-certificates/erp.crt
#sudo update-ca-certificates