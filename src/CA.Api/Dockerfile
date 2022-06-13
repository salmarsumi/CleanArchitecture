#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["src/CA.Api/CA.Api.csproj", "src/CA.Api/"]
COPY ["src/CA.Common/CA.Common.csproj", "src/CA.Common/"]
COPY ["src/CA.Api.Infrastructure/CA.Api.Infrastructure.csproj", "src/CA.Api.Infrastructure/"]
COPY ["src/CA.Common.EF/CA.Common.EF.csproj", "src/CA.Common.EF/"]
COPY ["src/CA.Api.Application/CA.Api.Application.csproj", "src/CA.Api.Application/"]
COPY ["src/CA.MediatR/CA.MediatR.csproj", "src/CA.MediatR/"]
COPY ["src/CA.Api.Domain/CA.Api.Domain.csproj", "src/CA.Api.Domain/"]
RUN dotnet restore "src/CA.Api/CA.Api.csproj"
COPY . .
WORKDIR "/src/src/CA.Api"
RUN dotnet build "CA.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CA.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN groupadd -r wfs && useradd -r -g wfs wfs
USER wfs
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CA.Api.dll"]