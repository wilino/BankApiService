# Usa una imagen base de .NET SDK 7.0 para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Establecer el directorio de trabajo en el contenedor
WORKDIR /src

# Copiar el archivo .csproj y restaurar dependencias
COPY ["ServicioCuentas.API/ServicioCuentas.API.csproj", "ServicioCuentas.API/"]

# Restaurar dependencias
RUN dotnet restore "ServicioCuentas.API/ServicioCuentas.API.csproj"

# Copiar el resto de la aplicación y compilarla
COPY . .
WORKDIR "/src/ServicioCuentas.API"
RUN dotnet build "ServicioCuentas.API.csproj" -c Release -o /app/build

# Publicar la aplicación
RUN dotnet publish "ServicioCuentas.API.csproj" -c Release -o /app/publish

# Usa una imagen base más ligera para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ServicioCuentas.API.dll"]
