# Dockerfile Guide for TaskManager API

## Overview
This Dockerfile uses a multi-stage build approach to create an optimized Docker image for the ASP.NET Core application.

## Build Stages

### Stage 1: Build Stage
- Uses `mcr.microsoft.com/dotnet/sdk:8.0` base image
- Installs all dependencies via NuGet restore
- Compiles the application
- Runs unit tests (fails build if tests fail)
- Publishes optimized Release build
- Final size: ~1-2GB (only used during build)

### Stage 2: Runtime Stage
- Uses `mcr.microsoft.com/dotnet/aspnet:8.0` base image (lightweight)
- Installs `curl` for health checks
- Creates non-root user `appuser` (security best practice)
- Copies only the published application from Stage 1
- Exposes port 8080
- Includes health check configuration
- Final image size: ~200-300MB

## Key Optimizations

1. **Multi-stage Build**: Reduces final image size by ~80%
2. **Non-root User**: Runs as `appuser` (UID 1001) instead of root
3. **Health Checks**: Automated container health monitoring
4. **Minimal Runtime Image**: Uses aspnet (not sdk)
5. **Layer Caching**: Leverages Docker layer caching for faster builds
6. **Environment Variables**: Configuration via env vars (12-factor app)

## Building

```bash
# Standard build
docker build -t taskmanager-api:latest .

# Build with specific tag
docker build -t taskmanager-api:v1.0.0 .

# Build with registry prefix
docker build -t myregistry.azurecr.io/taskmanager-api:latest .

# Build without cache
docker build --no-cache -t taskmanager-api:latest .
```

## Running

```bash
# Basic run
docker run -p 8080:8080 taskmanager-api:latest

# With environment variables
docker run -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS=http://+:8080 \
  taskmanager-api:latest

# With volume mounts
docker run -p 8080:8080 \
  -v $(pwd)/taskmanager.db:/app/taskmanager.db \
  taskmanager-api:latest

# Background (detached) mode
docker run -d -p 8080:8080 --name taskmanager-api taskmanager-api:latest
```

## Docker Compose

See `docker-compose.yml` for orchestrated local development with health checks and networking.

## Security Considerations

- ✅ Non-root user execution
- ✅ Minimal attack surface
- ✅ No secrets in image
- ✅ Health endpoint monitoring
- ✅ Read-only user context

## Size Optimization

- Runtime image: ~200MB
- Multi-stage eliminates SDK layer
- Only runtime dependencies included
- Curl is the only additional package
