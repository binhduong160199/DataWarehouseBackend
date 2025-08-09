# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY DataWarehouse.Models/DataWarehouse.Models.csproj ./DataWarehouse.Models/
COPY DataWarehouse.API/DataWarehouse.API.csproj ./DataWarehouse.API/
RUN dotnet restore ./DataWarehouse.API/DataWarehouse.API.csproj

# Copy everything and build
COPY . .
WORKDIR /src/DataWarehouse.API
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Logging (optional)
COPY ./DataWarehouse.API/log4net.config ./log4net.config

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "DataWarehouse.API.dll"]