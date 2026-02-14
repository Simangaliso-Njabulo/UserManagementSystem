# Solution Structure

This document provides a detailed overview of the project organization and the responsibilities of each component.

## Project Layout

```
UserManagementSystem/
├── src/
│   ├── UserManagement.Api/              # ASP.NET Core Web API
│   │   ├── Controllers/                 # API endpoints
│   │   ├── Services/                    # Business logic layer
│   │   ├── Program.cs                   # Application entry point
│   │   └── appsettings.json            # Configuration
│   │
│   ├── UserManagement.Web/              # ASP.NET Core MVC UI
│   │   ├── Controllers/                 # MVC controllers
│   │   ├── Views/                       # Razor views
│   │   ├── Services/                    # API client service
│   │   ├── Models/                      # View models
│   │   └── Program.cs
│   │
│   ├── UserManagement.Core/             # Domain layer
│   │   ├── Entities/                    # Domain entities
│   │   │   ├── User.cs
│   │   │   ├── Group.cs
│   │   │   ├── Permission.cs
│   │   │   ├── UserGroup.cs
│   │   │   └── GroupPermission.cs
│   │   ├── DTOs/                        # Data transfer objects
│   │   │   ├── UserDto.cs
│   │   │   ├── CreateUserDto.cs
│   │   │   ├── UpdateUserDto.cs
│   │   │   └── UserCountByGroupDto.cs
│   │   └── Interfaces/                  # Service and repository interfaces
│   │       ├── IUserService.cs
│   │       └── IUserRepository.cs
│   │
│   └── UserManagement.Data/             # Data access layer
│       ├── Context/                     
│       │   └── ApplicationDbContext.cs  # EF Core DbContext
│       ├── Repositories/                # Repository implementations
│       │   └── UserRepository.cs
│       ├── Migrations/                  # EF Core migrations
│       └── Configurations/              # Entity configurations
│           ├── UserConfiguration.cs
│           ├── GroupConfiguration.cs
│           └── PermissionConfiguration.cs
│
├── tests/
│   ├── UserManagement.UnitTests/        # Unit tests
│   │   ├── Services/                    # Service layer tests
│   │   └── Repositories/                # Repository tests
│   │
│   └── UserManagement.IntegrationTests/ # Integration tests
│       ├── Controllers/                 # API endpoint tests
│       ├── TestFixtures/                # Test setup and helpers
│       └── appsettings.Test.json       # Test configuration
│
├── docs/                                # Documentation
│   ├── SOLUTION_STRUCTURE.md
│   ├── KEY_DECISIONS.md
│   └── RUNNING_LOCALLY.md
│
└── README.md
```

## Project Responsibilities

### UserManagement.Core

**Purpose:** Contains the domain model and contracts.

**Responsibilities:**
- Defines core domain entities (User, Group, Permission)
- Declares Data Transfer Objects (DTOs) for API communication
- Defines service and repository interfaces
- Contains no implementation details or external dependencies

**Key Components:**
- **Entities:** Plain C# classes representing the database schema
  - `User` - User information with navigation properties to groups
  - `Group` - Group definitions
  - `Permission` - System permissions
  - `UserGroup` - Join table for User-Group many-to-many relationship
  - `GroupPermission` - Join table for Group-Permission many-to-many relationship

- **DTOs:** Objects for transferring data across layers
  - `UserDto` - Full user information including groups
  - `CreateUserDto` - User creation payload
  - `UpdateUserDto` - User update payload
  - `UserCountByGroupDto` - Reporting DTO

- **Interfaces:** Contracts for services and repositories
  - Enables dependency injection
  - Supports testing through mocking

**Dependencies:** None (pure domain layer)

---

### UserManagement.Data

**Purpose:** Handles all data access logic and database operations.

**Responsibilities:**
- Configures Entity Framework Core DbContext
- Implements repository pattern
- Contains database migrations
- Manages entity configurations and relationships
- Seeds initial data

**Key Components:**
- **ApplicationDbContext:** EF Core context
  - Configures entity relationships
  - Applies entity configurations
  - Seeds default data (Groups, Permissions, sample Users)

- **Repositories:** Implement data access operations
  - `UserRepository` - CRUD operations and queries for users
  - Abstracts database operations from business logic
  - Returns domain entities

- **Configurations:** Fluent API entity configurations
  - Defines primary keys, indexes, and constraints
  - Configures many-to-many relationships
  - Sets up navigation properties

- **Migrations:** EF Core migration files
  - Version control for database schema
  - Includes seed data initialization

**Dependencies:**
- UserManagement.Core (references domain entities and interfaces)
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools

---

### UserManagement.Api

**Purpose:** RESTful Web API providing user management endpoints.

**Responsibilities:**
- Exposes HTTP endpoints for user operations
- Implements business logic through service layer
- Handles HTTP request/response
- Performs input validation
- Provides API documentation via Swagger

**Key Components:**
- **Controllers:** API endpoints
  - `UsersController` - CRUD operations and reporting endpoints
  - Returns appropriate HTTP status codes
  - Maps between DTOs and domain entities

- **Services:** Business logic layer
  - `UserService` - Orchestrates operations between controller and repository
  - Handles validation and business rules
  - Maps entities to DTOs

- **Configuration:**
  - Dependency injection setup
  - Database connection string
  - Swagger/OpenAPI configuration

**Endpoints:**
- `GET /api/users` - Retrieve all users
- `GET /api/users/{id}` - Retrieve specific user
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update existing user
- `DELETE /api/users/{id}` - Delete user
- `GET /api/users/count` - Get total user count
- `GET /api/users/count-by-group` - Get users per group

**Dependencies:**
- UserManagement.Core (domain entities, DTOs, interfaces)
- UserManagement.Data (data access implementation)
- Swashbuckle.AspNetCore (Swagger)

---

### UserManagement.Web

**Purpose:** Web-based user interface for user management.

**Responsibilities:**
- Provides user interface for CRUD operations
- Consumes the Web API via HTTP calls
- Handles user input and displays results
- Provides basic form validation

**Key Components:**
- **Controllers:** MVC controllers
  - `UsersController` - Handles UI requests
  - Calls API service for data operations
  - Returns views with appropriate models

- **Views:** Razor views
  - `Index.cshtml` - User listing
  - `Create.cshtml` - Add new user form
  - `Edit.cshtml` - Edit user form
  - `Delete.cshtml` - Delete confirmation

- **Services:** API client
  - `ApiService` - Handles HTTP communication with the API
  - Uses HttpClient for API calls
  - Serializes/deserializes JSON

- **Models:** View models (if needed)
  - May contain UI-specific properties not in DTOs

**Dependencies:**
- UserManagement.Core (references DTOs for communication)
- No direct reference to UserManagement.Data (communicates via API only)

---

### UserManagement.UnitTests

**Purpose:** Tests individual components in isolation.

**Responsibilities:**
- Tests business logic in services
- Tests repository implementations
- Uses mocking to isolate dependencies
- Verifies edge cases and error handling

**Key Components:**
- **Service Tests:** Test business logic
  - Mock repository dependencies
  - Verify correct behavior for various scenarios
  - Test validation logic

- **Repository Tests (Optional):** Test data access logic
  - Can use in-memory database for isolated tests

**Testing Approach:**
- Arrange-Act-Assert pattern
- Mock external dependencies using Moq
- Use FluentAssertions for readable test assertions

**Dependencies:**
- xUnit (test framework)
- Moq (mocking framework)
- FluentAssertions (assertion library)

---

### UserManagement.IntegrationTests

**Purpose:** Tests the complete API stack from HTTP request to database.

**Responsibilities:**
- Tests entire request/response pipeline
- Verifies controller, service, and repository integration
- Tests database interactions with real EF Core operations
- Validates HTTP responses and status codes

**Key Components:**
- **Controller Tests:** End-to-end API tests
  - Use WebApplicationFactory to host the API
  - Make actual HTTP requests to endpoints
  - Verify complete workflows (Create → Read → Update → Delete)

- **Test Fixtures:** Shared test setup
  - Custom WebApplicationFactory configuration
  - In-memory database setup
  - Test data seeding

**Testing Approach:**
- Uses in-memory SQLite database for test isolation
- Each test runs against fresh database state
- Tests real HTTP requests without mocking
- Validates status codes, headers, and response bodies

**Dependencies:**
- xUnit
- Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory)
- Microsoft.EntityFrameworkCore.InMemory (or Sqlite)
- FluentAssertions

---

## Dependency Flow

```
┌─────────────────────┐
│  UserManagement.Web │  (UI Layer - Consumes API)
└──────────┬──────────┘
           │ HTTP
           ▼
┌─────────────────────┐
│  UserManagement.Api │  (API Layer)
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ UserManagement.Core │  (Domain Layer)
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ UserManagement.Data │  (Data Layer)
└──────────┬──────────┘
           │
           ▼
     [SQL Database]
```

**Separation Benefits:**
- **Testability:** Each layer can be tested independently
- **Maintainability:** Changes in one layer don't cascade to others
- **Scalability:** API can serve multiple clients (web, mobile, etc.)
- **Clear Responsibilities:** Each project has a single, well-defined purpose

---

## Architecture Principles

### Separation of Concerns

Each project has a distinct responsibility:
- **Core** = Domain and contracts
- **Data** = Database operations
- **Api** = HTTP endpoints and business logic
- **Web** = User interface

### Dependency Inversion

- High-level modules (Api) depend on abstractions (Interfaces in Core)
- Low-level modules (Data) implement those abstractions
- Enables loose coupling and easier testing

### Repository Pattern

- Abstracts data access behind IUserRepository interface
- Controllers and services don't directly use DbContext
- Simplifies unit testing (can mock repositories)
- Centralizes data access logic

### Service Layer

- Business logic lives in services, not controllers
- Controllers are thin, handling only HTTP concerns
- Services are reusable across multiple controllers
- Makes business rules easier to test

### DTOs for API Boundaries

- API doesn't expose domain entities directly
- DTOs provide a stable contract for API consumers
- Allows internal domain model to evolve without breaking API
- Prevents over-posting vulnerabilities

---

## Database Schema

### Entity Relationships

```
┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│    User     │────────▶│  UserGroup   │◀────────│    Group    │
│             │    M:N  │              │   M:N   │             │
│ - Id        │         │ - UserId     │         │ - Id        │
│ - FirstName │         │ - GroupId    │         │ - Name      │
│ - LastName  │         └──────────────┘         │ - Desc      │
│ - Email     │                                   └──────┬──────┘
│ - CreatedAt │                                          │
└─────────────┘                                          │ M:N
                                                         │
                                              ┌──────────▼─────────┐
                                              │ GroupPermission    │
                                              │                    │
                                              │ - GroupId          │
                                              │ - PermissionId     │
                                              └──────────┬─────────┘
                                                         │
                                                         │
                                              ┌──────────▼─────────┐
                                              │   Permission       │
                                              │                    │
                                              │ - Id               │
                                              │ - Name             │
                                              │ - Description      │
                                              └────────────────────┘
```

### Seed Data

The initial migration includes:

**Groups:**
- Admin
- Level 1
- Level 2

**Permissions:**
- Read
- Write
- Delete
- Manage Users

**Sample Users:**
- Pre-assigned to various groups for demonstration purposes
