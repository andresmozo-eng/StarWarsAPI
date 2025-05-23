# Imagen base del SDK para compilar
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

# Establecer directorio de trabajo
WORKDIR /app

# Copiar los archivos de solución y proyectos
COPY *.sln .
COPY src/StarWarsAPI.API/*.csproj ./src/StarWarsAPI.API/
COPY src/StarWarsAPI.Application/*.csproj ./src/StarWarsAPI.Application/
COPY src/StarWarsAPI.Domain/*.csproj ./src/StarWarsAPI.Domain/
COPY src/StarWarsAPI.Infrastructure/*.csproj ./src/StarWarsAPI.Infrastructure/
COPY tests/StarWarsAPI.Tests/*.csproj ./tests/StarWarsAPI.Tests/

# Restaurar dependencias
RUN dotnet restore

# Copiar todo el código
COPY . .

# Compilar el proyecto en modo Release
RUN dotnet publish src/StarWarsAPI.API/StarWarsAPI.API.csproj -c Release -o out

# Imagen base para ejecutar (más liviana)
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime

WORKDIR /app
COPY --from=build /app/out .

# Exponer el puerto por defecto
EXPOSE 80

# Comando para ejecutar la app
ENTRYPOINT ["dotnet", "StarWarsAPI.API.dll"]
