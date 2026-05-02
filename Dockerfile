# Multi-stage build for .NET 8.0 ASP.NET Core application
# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /src

# Copy solution and project files
COPY ["TaskManager.sln", "."]
COPY ["TaskManager.API/TaskManager.API.csproj", "TaskManager.API/"]
COPY ["TaskManager.Application/TaskManager.Application.csproj", "TaskManager.Application/"]
COPY ["TaskManager.Core/TaskManager.Core.csproj", "TaskManager.Core/"]
COPY ["TaskManager.Infrastructure/TaskManager.Infrastructure.csproj", "TaskManager.Infrastructure/"]
COPY ["TaskManager.Tests/TaskManager.Tests.csproj", "TaskManager.Tests/"]

# Restore dependencies
RUN dotnet restore "TaskManager.sln"

# Copy application code
COPY . .

# Build the application
RUN dotnet build "TaskManager.sln" -c Release --no-restore

# Run unit tests
RUN dotnet test "TaskManager.Tests/TaskManager.Tests.csproj" -c Release --no-build --verbosity normal

# Publish the application
RUN dotnet publish "TaskManager.API/TaskManager.API.csproj" -c Release --no-build -o /app/publish

# Stage 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Create non-root user for security
RUN useradd -m -u 1001 appuser && chown -R appuser:appuser /app

# Copy published application from build stage
COPY --from=build --chown=appuser:appuser /app/publish .

# Switch to non-root user
USER appuser

# Expose port (HTTP on 8080 for container conventions, ASPNETCORE_URLS will configure)
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/swagger || exit 1

# Entry point
ENTRYPOINT ["dotnet", "TaskManager.API.dll"]
