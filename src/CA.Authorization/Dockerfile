#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["src/CA.Authorization/CA.Authorization.csproj", "src/CA.Authorization/"]
COPY ["src/CA.Common/CA.Common.csproj", "src/CA.Common/"]
RUN dotnet restore "src/CA.Authorization/CA.Authorization.csproj"
COPY . .
WORKDIR "/src/src/CA.Authorization"
RUN dotnet build "CA.Authorization.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CA.Authorization.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN groupadd -r wfs && useradd -r -g wfs wfs
USER wfs
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CA.Authorization.dll"]