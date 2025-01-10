# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install openssl to generate self-signed certificates
RUN apt-get update && apt-get install -y openssl

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlazorApp.csproj", "."]
RUN dotnet restore "./BlazorApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./BlazorApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlazorApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Generate a self-signed certificate at runtime
RUN mkdir /https
COPY ["generate-cert.sh", "/app/generate-cert.sh"]
RUN chmod +x /app/generate-cert.sh

# Set the entrypoint to run the certificate generation script and then start the app
ENTRYPOINT ["/bin/bash", "-c", "/app/generate-cert.sh && dotnet BlazorApp.dll"]
