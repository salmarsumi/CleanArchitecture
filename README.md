# ASP.NET Core 6.0 Multi-Service Application
This repository aims to demonstrate the use of ASP.NET Core 6.0 to build a multi-service  application.

## Architecture

![system overview](./assets/container.png)

## Docker Installation

1. While in the repository change directory to docker
```
cd docker
```

2. Generate SSL certificate.
```
bash create-cert.sh $(uname -n)
```
