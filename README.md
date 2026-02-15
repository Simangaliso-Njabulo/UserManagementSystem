# User Management System

A RESTful web service and web interface for managing users, groups, and permissions, built with ASP.NET Core and Entity Framework Core.

## Overview

This project implements a user management system with the following features:

- **RESTful API** - Complete CRUD operations for user management
- **Web Interface** - MVC application for interacting with users
- **Database Design** - Code First approach with many-to-many relationships
- **Comprehensive Testing** - Unit and integration tests included

## Documentation

- **[Solution Structure](docs/SOLUTION_STRUCTURE.md)** - Project organization and component responsibilities
- **[Key Technical Decisions](docs/KEY_DECISIONS.md)** - Architecture choices and rationale
- **[Running Locally](docs/RUNNING_LOCALLY.md)** - Complete setup and testing guide

## Quick Start

```bash
# Clone the repository
git clone https://github.com/yourusername/UserManagementSystem.git
cd UserManagementSystem

# Restore dependencies
dotnet restore

# Run database migrations
cd src/UserManagement.Api
dotnet ef database update

# Run the API
dotnet run
```

For detailed setup instructions, see [Running Locally](docs/RUNNING_LOCALLY.md).

## Running Tests

The solution includes comprehensive unit and integration tests.

```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test tests/UserManagement.UnitTests

# Run only integration tests
dotnet test tests/UserManagement.IntegrationTests
```

**Test Coverage:**
- 8 unit tests for service layer business logic
- 13 integration tests for API endpoints
- Tests verify CRUD operations, error handling, and edge cases

## Technology Stack

- **Framework:** ASP.NET Core (.NET 8)
- **ORM:** Entity Framework Core (Code First)
- **Database:** SQL Server (LocalDB)
- **Testing:** xUnit, FluentAssertions, Moq
- **API Documentation:** Swagger/Swashbuckle

## Key Features

### API Endpoints

- User CRUD operations (Create, Read, Update, Delete)
- Total user count reporting
- Users per group reporting
- Full Swagger/OpenAPI documentation

### Database Model

- Users can belong to multiple Groups (many-to-many)
- Groups can have multiple Permissions (many-to-many)
- Seed data included for demonstration

### Testing

- Unit tests for business logic and services
- Integration tests using WebApplicationFactory
- In-memory database for test isolation

## Project Structure

```
UserManagementSystem/
├── src/
│   ├── UserManagement.Api/         # RESTful Web API
│   ├── UserManagement.Web/         # MVC Web Interface
│   ├── UserManagement.Core/        # Domain entities and interfaces
│   └── UserManagement.Data/        # Data access layer
├── tests/
│   ├── UserManagement.UnitTests/
│   └── UserManagement.IntegrationTests/
└── docs/                           # Documentation
```

For a detailed breakdown, see [Solution Structure](docs/SOLUTION_STRUCTURE.md).

## API Documentation

Once running, access the interactive Swagger documentation at:

**`https://localhost:7113/swagger`**

## Requirements Met

This solution addresses all assessment requirements:

- **Task 1: Database Design** - Code First approach with Users, Groups, and Permissions  
- **Task 2: RESTful API** - Complete CRUD operations and reporting endpoints  
- **Task 3: Web Interface** - MVC application consuming the API  
- **Testing** - Unit and integration tests included  
- **Deployment Ready** - Can be cloned, built, and run locally  

## Getting Help

If you encounter any issues during setup, please refer to:
- [Running Locally](docs/RUNNING_LOCALLY.md) - Includes troubleshooting section
- [Solution Structure](docs/SOLUTION_STRUCTURE.md) - Understanding the project layout
- [Key Technical Decisions](docs/KEY_DECISIONS.md) - Context for architecture choices