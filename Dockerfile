# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /app

COPY . .
WORKDIR /app/src/StarWarsApi.API
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/src/StarWarsApi.API/out ./
ENTRYPOINT ["dotnet", "StarWarsApi.API.dll"]
