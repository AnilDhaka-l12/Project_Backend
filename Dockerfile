# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY projectBackend.csproj ./
RUN dotnet restore

# Copy everything else
COPY . ./

# Build and publish with detailed logging
RUN dotnet publish -c Release -o /app/publish --verbosity detailed

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl and other useful tools for debugging
RUN apt-get update && apt-get install -y \
    curl \
    procps \
    net-tools \
    dnsutils \
    && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=build /app/publish .

# Set environment for detailed logging
ENV ASPNETCORE_ENVIRONMENT=Docker
ENV ASPNETCORE_DETAILEDERRORS=true
ENV LOGGING__CONSOLE__FORMATTERNAME=simple
ENV LOGGING__LOGLEVEL__DEFAULT=Debug
ENV LOGGING__LOGLEVEL__Microsoft=Information
ENV LOGGING__LOGLEVEL__Microsoft.Hosting.Lifetime=Information
ENV LOGGING__LOGLEVEL__ProjectBackend=Debug

# Expose ports
EXPOSE 80
EXPOSE 443

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "projectBackend.dll"]