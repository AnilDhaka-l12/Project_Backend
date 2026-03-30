# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY projectBackend.csproj ./
RUN dotnet restore

# Copy everything else
COPY . ./

# Build and publish
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=build /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "projectBackend.dll"]