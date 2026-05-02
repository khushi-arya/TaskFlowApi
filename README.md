# TaskManager - Production-Ready .NET Web API

A modern .NET 8 Web API application demonstrating clean architecture, SOLID principles, and DevOps best practices.

## 📋 Project Overview

This is a **Task Manager API** that provides CRUD operations for managing tasks. The project showcases:

- ✅ Clean Architecture (Layered approach)
- ✅ Dependency Injection
- ✅ Entity Framework Core with SQLite
- ✅ FluentValidation
- ✅ Comprehensive unit tests with xUnit and Moq
- ✅ API documentation with Swagger
- ✅ CI/CD pipelines (Azure DevOps, GitHub Actions)
- ✅ Docker containerization
- ✅ SonarQube code quality scanning

## 🏗️ Project Structure

```
TaskManager/
├── TaskManager.API/              # Web API (controllers, middleware)
│   ├── Controllers/
│   ├── Program.cs               # Configuration & DI setup
│   └── Properties/
├── TaskManager.Core/             # Domain models & interfaces
│   ├── Entities/                # Business entities
│   ├── DTOs/                    # Data Transfer Objects
│   └── Interfaces/              # Repository & Service contracts
├── TaskManager.Application/      # Business Logic
│   ├── Services/                # Service implementations
│   └── Validators/              # FluentValidation rules
├── TaskManager.Infrastructure/   # Data Access
│   ├── Data/                    # DbContext
│   └── Repositories/            # Repository implementations
├── TaskManager.Tests/            # Unit & Integration tests
├── azure-pipelines.yml           # Azure DevOps CI/CD
├── .github/workflows/            # GitHub Actions CI/CD
├── Dockerfile                    # Container configuration
└── README.md                     # This file
```

## 🚀 Quick Start

### Prerequisites

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Git** - [Download](https://git-scm.com)
- **Visual Studio 2022** or **VS Code** (optional)

### Option 1: Run with `dotnet CLI`

```bash
# Clone the repository
git clone https://github.com/your-org/TaskManager.git
cd TaskManager

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
dotnet run --project TaskManager.API

# API will be available at http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### Option 2: Run with Visual Studio

1. Open `TaskManager.sln` in Visual Studio 2022
2. Right-click the solution → **Set Startup Projects** → Select `TaskManager.API`
3. Press `F5` to run with debugging

### Option 3: Run with VS Code

1. Install the **C# Dev Kit** extension
2. Open the project folder in VS Code
3. Terminal → **Run Build Task** or execute:
   ```bash
   dotnet run --project TaskManager.API
   ```

## 📚 API Endpoints

### Tasks

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/tasks` | Get all tasks |
| `GET` | `/api/tasks/{id}` | Get task by ID |
| `POST` | `/api/tasks` | Create new task |
| `PUT` | `/api/tasks/{id}` | Update task |
| `DELETE` | `/api/tasks/{id}` | Delete task |
| `POST` | `/api/tasks/{id}/complete` | Mark task as complete |

### Sample Request

```bash
curl -X POST http://localhost:5000/api/tasks \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Build API",
    "description": "Create REST API endpoints",
    "priority": 5,
    "dueDate": "2025-12-31T23:59:59Z"
  }'
```

## 🧪 Running Tests

### Run All Tests

```bash
dotnet test
```

### Run Tests with Coverage

```bash
dotnet test /p:CollectCoverage=true
```

### Run Specific Test Project

```bash
dotnet test TaskManager.Tests
```

### Test Projects Included

- **TaskService Tests** - Service layer logic
- **TaskController Tests** - API endpoint validation (with mocking)
- **Validation Tests** - Input validation rules

## 🐳 Docker

### Build Docker Image

```bash
docker build -t taskmanager:latest .
```

### Run Container

```bash
docker run -p 5000:5000 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  taskmanager:latest
```

## 🔐 Security Features

- **Input Validation** - FluentValidation rules
- **Secure Headers** - CORS policies configured
- **Error Handling** - Graceful exception handling
- **Logging** - Structured logging throughout

## 📊 Code Quality

### SonarQube Analysis

```bash
# Install SonarQube CLI
dotnet tool install --global dotnet-sonarscanner

# Run analysis
dotnet sonarscanner begin /k:TaskManager /d:sonar.host.url=http://localhost:9000 /d:sonar.login=YOUR_TOKEN
dotnet build
dotnet sonarscanner end /d:sonar.login=YOUR_TOKEN
```

## 🔄 CI/CD Pipelines

### Azure DevOps

- Location: `azure-pipelines.yml`
- Triggers on: Push to `main` and `develop` branches
- Stages: Build → Test → Publish → Deploy

### GitHub Actions

- Location: `.github/workflows/`
- Runs: Build, Test, SonarQube scan, Deploy to Azure

## 📦 Dependencies

### NuGet Packages

- **Microsoft.EntityFrameworkCore** (8.0.0) - ORM
- **Microsoft.EntityFrameworkCore.Sqlite** (8.0.0) - SQLite provider
- **FluentValidation** (11.8.0) - Input validation
- **xUnit** (2.6.4) - Testing framework
- **Moq** (4.20.70) - Mocking library
- **FluentAssertions** (6.12.0) - Assertion library

## 🔧 Development Commands

```bash
# Create migration (if using migrations)
dotnet ef migrations add InitialCreate --project TaskManager.Infrastructure

# Update database
dotnet ef database update --project TaskManager.Infrastructure

# Watch mode (rebuild on file change)
dotnet watch --project TaskManager.API run

# Publish for production
dotnet publish -c Release -o ./publish
```

## 📝 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=taskmanager.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

## 🚢 Deployment

### Deploy to Azure App Service

```bash
# Using Azure CLI
az webapp deployment source config-zip \
  --resource-group myResourceGroup \
  --name myAppService \
  --src ./publish.zip
```

### Deploy using Azure DevOps Pipeline

- Configure Azure subscription in pipeline
- Pipeline automatically deploys on successful test pass

## 🐛 Troubleshooting

### Port Already in Use

```bash
# Change port in launchSettings.json
# Or run on different port:
dotnet run --project TaskManager.API -- --urls="http://localhost:5002"
```

### Database Issues

```bash
# Remove SQLite database and recreate
rm taskmanager.db
dotnet run --project TaskManager.API
```

### Tests Failing

```bash
# Clean and rebuild
dotnet clean
dotnet build
dotnet test --verbosity normal
```

## 🤝 Contributing

1. Create a feature branch: `git checkout -b feature/amazing-feature`
2. Commit changes: `git commit -m 'Add amazing feature'`
3. Push to branch: `git push origin feature/amazing-feature`
4. Open a Pull Request

## 📄 License

MIT License - See LICENSE file for details

## 📞 Support

For issues or questions:
- Create an Issue in GitHub
- Contact: [your-email@example.com](mailto:your-email@example.com)

---

**Last Updated:** May 2026  
**Version:** 1.0.0  
**Status:** Production Ready ✅
