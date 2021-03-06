# ASP.NET Core 6.0 Multi-Service Application
This repository aims to demonstrate the use of ASP.NET Core 6.0 to build a multi-service  application.


## Architecture
An architectur documentation is provided under the architecture directory.

### Context View
![system overview](./assets/context.png)

### Container View
![system overview](./assets/container.png)


## Docker Deployment
The next steps can be followed to run the application. The following was tested on an Ubuntu 20.04.

1. While in the repository change directory to docker
```
cd docker
```

2. Generate SSL certificate.
```
bash create-cert.sh $(uname -n)
```

3. Build the docker images.
```
bash build.sh
```

4. Run the containers using docker compose.
```
bash up.sh $(uname -n)
```

5. Ports published.
   * 8080 The BFF web application used to deliver the SPA.
   * 8090 The IdentityServer application.
   * 3000 The Grafana portal.

6. Get the hostname.
```
uname -n
```

7. Access the application through the host name.
```
https://<hostname>:8080
```

### Deployment View
![deployment overview](./assets/deployment.png)
