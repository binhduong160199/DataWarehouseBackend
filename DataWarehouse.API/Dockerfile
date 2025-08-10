# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DataWarehouse.Models/DataWarehouse.Models.csproj ./DataWarehouse.Models/
COPY DataWarehouse.API/DataWarehouse.API.csproj ./DataWarehouse.API/

RUN dotnet restore ./DataWarehouse.API/DataWarehouse.API.csproj

# Copy the rest of the source
COPY . .

# Publish self-contained app
WORKDIR /src/DataWarehouse.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# App files
COPY --from=build /app/publish .

# Logging (optional)
COPY ./DataWarehouse.API/log4net.config ./log4net.config

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "DataWarehouse.API.dll"]