FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

EXPOSE 8080
EXPOSE 8081
ARG BUILD_CONFIGURATION

WORKDIR /src

COPY ["InventoryManagement.API/InventoryManagement.API.csproj", "InventoryManagement.API/"]
COPY ["InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj", "InventoryManagement.Infrastructure/"]
COPY ["InventoryManagement.Application/InventoryManagement.Application.csproj", "InventoryManagement.Application/"]

RUN dotnet restore "InventoryManagement.API/InventoryManagement.API.csproj"

COPY . .

RUN dotnet publish "InventoryManagement.API/InventoryManagement.API.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "InventoryManagement.API.dll"]
