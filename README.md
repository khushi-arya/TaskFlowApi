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
