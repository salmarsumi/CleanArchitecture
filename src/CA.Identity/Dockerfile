#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["src/CA.Identity/CA.Identity.csproj", "src/CA.Identity/"]
COPY ["src/CA.Common/CA.Common.csproj", "src/CA.Common/"]
RUN dotnet restore "src/CA.Identity/CA.Identity.csproj"
COPY . .
WORKDIR "/src/src/CA.Identity"
RUN dotnet build "CA.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CA.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN addgroup -S wfs && adduser -S wfs -G wfs \
	&& mkdir /home/wfs/.aspnet/DataProtection-Keys -p \
	&& chown -R wfs /home/wfs \
	&& chown wfs /app
USER wfs
COPY --chown=wfs:wfs --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CA.Identity.dll"]