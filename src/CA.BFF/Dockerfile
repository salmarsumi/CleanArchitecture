#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
RUN curl -fsSL https://deb.nodesource.com/setup_14.x | bash - && apt-get install -y nodejs
COPY ["src/CA.BFF/CA.BFF.csproj", "src/CA.BFF/"]
COPY ["src/CA.Common/CA.Common.csproj", "src/CA.Common/"]
RUN dotnet restore "src/CA.BFF/CA.BFF.csproj"
COPY . .
WORKDIR "/src/src/CA.BFF"
RUN dotnet build "CA.BFF.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CA.BFF.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY docker/certs/server.crt /usr/local/share/ca-certificates
RUN cat /usr/local/share/ca-certificates/server.crt >> /etc/ssl/certs/ca-certificates.crt \
	&& apk --no-cache add ca-certificates \
    && rm -rf /var/cache/apk/* \
	&& update-ca-certificates \
	&& addgroup -S wfs && adduser -S wfs -G wfs \
	&& mkdir /home/wfs/.aspnet/DataProtection-Keys -p \
	&& chown -R wfs /home/wfs
USER wfs
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CA.BFF.dll"]