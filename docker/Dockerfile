# Use the official .NET 10 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80;https://+:443

# Install PostgreSQL client and curl for health checks
RUN apt-get update && apt-get install -y \
    postgresql-client \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Use the SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["src/ConfigurationManagement.Api/ConfigurationManagement.Api.csproj", "src/ConfigurationManagement.Api/"]
COPY ["src/ConfigurationManagement.Application/ConfigurationManagement.Application.csproj", "src/ConfigurationManagement.Application/"]
COPY ["src/ConfigurationManagement.Domain/ConfigurationManagement.Domain.csproj", "src/ConfigurationManagement.Domain/"]
COPY ["src/ConfigurationManagement.Infrastructure/ConfigurationManagement.Infrastructure.csproj", "src/ConfigurationManagement.Infrastructure/"]

# Restore NuGet packages for API project
RUN dotnet restore "src/ConfigurationManagement.Api/ConfigurationManagement.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/ConfigurationManagement.Api"
RUN dotnet build "ConfigurationManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConfigurationManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user for security (using a different UID)
RUN useradd -m -u 9999 -s /bin/bash appuser && chown -R appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s \
    CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "ConfigurationManagement.Api.dll"]