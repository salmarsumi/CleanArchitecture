#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["src/CA.Audit/CA.Audit.csproj", "src/CA.Audit/"]
COPY ["src/CA.MediatR/CA.MediatR.csproj", "src/CA.MediatR/"]
COPY ["src/CA.Common/CA.Common.csproj", "src/CA.Common/"]
COPY ["src/CA.Common.EF/CA.Common.EF.csproj", "src/CA.Common.EF/"]
COPY ["src/CA.Common.Authorization/CA.Common.Authorization.csproj", "src/CA.Common.Authorization/"]
COPY ["src/CA.Common.Contracts/CA.Common.Contracts.csproj", "src/CA.Common.Contracts/"]
RUN dotnet restore "src/CA.Audit/CA.Audit.csproj"
COPY . .
WORKDIR "/src/src/CA.Audit"
RUN dotnet build "CA.Audit.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CA.Audit.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN addgroup -S wfs && adduser -S wfs -G wfs
USER wfs
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CA.Audit.dll"]