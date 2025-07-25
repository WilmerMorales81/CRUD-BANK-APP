###########  Build stage  #################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el .csproj y restaurar dependencias
COPY ["CRUD-BANK-APP.csproj", "./"]
RUN dotnet restore

# Copiar todo el código y publicar en Release
COPY . .
RUN dotnet publish -c Release -o /app/out --no-restore

###########  Runtime stage  ###############
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiar binarios publicados
COPY --from=build /app/out .

# Expone el puerto 5000 dentro del contenedor
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Comando de arranque
ENTRYPOINT ["dotnet", "CRUD-BANK-APP.dll"]
