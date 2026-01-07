# =========================
# Base runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar csproj (cache de restore)
COPY api-gateway-microsservice/api-gateway-microsservice.csproj api-gateway-microsservice/
 
RUN dotnet restore api-gateway-microsservice/api-gateway-microsservice.csproj

# Copiar todo o código
COPY . .

# Build
WORKDIR /src/api-gateway-microsservice
RUN dotnet build api-gateway-microsservice.csproj -c $BUILD_CONFIGURATION -o /app/build

# Publish
RUN dotnet publish api-gateway-microsservice.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# Migrator (opcional)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS migrator
WORKDIR /src

COPY . .
WORKDIR /src/api-gateway-microsservice

RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "ef", "database", "update", "--no-build"]

# =========================
# Runtime final
# =========================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "api-gateway-microsservice.dll"]
