# Key Technical Decisions

This document explains the architectural choices made in this solution and the reasoning behind them.

## Table of Contents

- [Custom Entities vs ASP.NET Identity](#custom-entities-vs-aspnet-identity)
- [Separate API and UI Projects](#separate-api-and-ui-projects)
- [Repository Pattern with Service Layer](#repository-pattern-with-service-layer)
- [Project Types and Dependencies](#project-types-and-dependencies)
- [No Authentication/Authorization](#no-authenticationauthorization)
- [No End-to-End UI Tests](#no-end-to-end-ui-tests)
- [Testing Strategy](#testing-strategy)
- [Technology Stack Choices](#technology-stack-choices)
  - [.NET 8 vs .NET 10 (LTS)](#net-8-vs-net-10-lts)

---

## Custom Entities vs ASP.NET Identity

**Decision:** Built custom User, Group, and Permission entities from scratch using EF Core Code First instead of using ASP.NET Identity.

### Rationale

**Matches Assessment Requirements**
- Task 1 explicitly asks to "create a SQL database using a Code First approach" with specific entities: Users, Groups, and Permissions
- The requirement specifies a many-to-many relationship structure that differs from Identity's built-in model
- The assessment appears designed to evaluate database design skills, which would be hidden by using a pre-built framework

**Domain Model Differences**
- The requirement uses "Groups" as the organizational structure
- ASP.NET Identity is built around "Roles" which have different semantics and implementation
- Identity's schema includes many additional tables (Claims, Tokens, Logins) that aren't part of this requirement

**Complexity Considerations**
- For this specific requirement (3 entities + 2 join tables), building custom entities is actually simpler than installing and configuring Identity
- Custom implementation gives full control over the schema and relationships
- Easier to understand and maintain for this specific use case

**What Was Not Considered**
- This decision doesn't imply Identity is unsuitable for production applications
- For real-world user management with authentication needs, Identity would be the better choice
- The custom approach was chosen specifically to address the assessment criteria

---

## Separate API and UI Projects

**Decision:** Created two distinct projects - UserManagement.Api (Web API) and UserManagement.Web (MVC UI).

### Rationale

**Assessment Structure**
- Task 2 asks for a "RESTful web service"
- Task 3 asks for a "web interface that consumes the API"
- These are presented as distinct tasks, suggesting they should be separate components

**Architectural Benefits**
- Clear separation of concerns between backend logic and presentation
- The API can potentially serve multiple clients (not just the web UI)
- Each component can be developed, tested, and deployed independently
- Mirrors modern application architecture patterns

**Testability**
- API can be tested independently using integration tests
- UI can be tested without requiring the full API to be running (using mocked API responses if needed)
- Clearer boundaries make it easier to isolate issues

**Real-World Alignment**
- This structure reflects how many production applications are built
- Demonstrates understanding of layered architecture
- Shows ability to work with distributed systems

---

## Repository Pattern with Service Layer

**Decision:** Implemented a repository layer for data access and a service layer for business logic.

### Rationale

**Separation of Concerns**
- Controllers handle HTTP concerns (routing, status codes)
- Services contain business logic and orchestration
- Repositories handle data access
- Each component has a single, well-defined responsibility

**Testability**
- Repositories can be mocked in service tests
- Services can be mocked in controller tests
- Business logic can be tested without database dependencies
- Easier to write focused unit tests

**Maintainability**
- Business rules are centralized in services, not scattered across controllers
- Data access logic is isolated in repositories
- Changes to data access don't affect business logic
- Easier to understand and modify

**Industry Practice**
- Repository pattern is widely recognized and understood
- Makes the codebase approachable for other developers
- Demonstrates knowledge of common design patterns

**Trade-offs Acknowledged**
- Adds some additional abstraction layers
- For very simple CRUD operations, might seem like overhead
- The benefit increases as business logic complexity grows

---

## Project Types and Dependencies

**Decision:** Used specific project types for each component - Class Libraries for Core and Data, ASP.NET Core projects for API and Web, and xUnit Test Projects for tests.

### Why Different Project Types?

#### Class Libraries (UserManagement.Core & UserManagement.Data)

**Project Type:** Class Library (.NET 8)

**What They Are:**
- Class Libraries are compiled into DLL files, not executables
- They contain reusable code that other projects can reference
- Cannot be run directly - they're dependencies for other projects

**Why Class Library for UserManagement.Core:**
- Contains domain entities (User, Group, Permission)
- Contains DTOs (Data Transfer Objects)
- Contains interfaces (IUserRepository, IUserService)
- Pure C# classes with no web/API-specific functionality
- Needs to be referenced by multiple projects (Data, API, Web, Tests)
- Lightweight - no extra dependencies needed

**Why Class Library for UserManagement.Data:**
- Contains EF Core DbContext and database logic
- Contains repository implementations
- Contains entity configurations
- Focused solely on data access, not HTTP or UI concerns
- Can be referenced by API and test projects
- Allows swapping out data access implementations without changing API

**Benefits:**
- **Reusability:** Can be referenced by multiple projects
- **Separation of Concerns:** No accidental dependencies on web/API frameworks
- **Testability:** Can be tested independently
- **Flexibility:** Could be used in console apps, background services, etc.

#### ASP.NET Core Web API (UserManagement.Api)

**Project Type:** ASP.NET Core Web API

**What It Is:**
- A web application specifically designed for building RESTful APIs
- Includes built-in HTTP server (Kestrel)
- Can be run as an executable
- Configured for JSON serialization, routing, middleware

**Why Not Class Library:**
- Needs to listen for HTTP requests (requires Kestrel web server)
- Needs routing infrastructure (`[Route]` attributes)
- Needs middleware pipeline (authentication, CORS, exception handling)
- Must be executable to run as a service
- Comes with Program.cs to configure and start the application

**What It Includes:**
- Controllers for handling HTTP requests
- Service registration (Dependency Injection)
- Swagger/OpenAPI configuration
- Middleware pipeline configuration
- Application hosting and lifecycle management

#### ASP.NET Core MVC (UserManagement.Web)

**Project Type:** ASP.NET Core Web App (Model-View-Controller)

**What It Is:**
- A web application for serving HTML pages
- Includes MVC framework (Models, Views, Controllers)
- Includes Razor view engine for rendering HTML
- Can be run as an executable

**Why Not Class Library:**
- Needs to serve HTTP requests and responses
- Needs Razor view rendering engine
- Needs static file serving (CSS, JavaScript, images)
- Must be executable to run as a web server
- Includes wwwroot folder for static assets

**What It Includes:**
- MVC Controllers (return views, not JSON)
- Razor Views (.cshtml files)
- View Models
- Static file serving
- Tag Helpers and HTML Helpers

#### xUnit Test Projects (UserManagement.UnitTests & IntegrationTests)

**Project Type:** xUnit Test Project

**What They Are:**
- Special project type specifically for running tests
- Executable projects that run via test runners
- Automatically configured with xUnit testing framework

**Why Not Class Library:**
- Need to be discovered and executed by test runners
- Must reference the xUnit framework
- Require special MSBuild properties for test discovery
- Need test runner integration in Visual Studio/VS Code

**What They Include:**
- xUnit test framework
- Test discovery attributes ([Fact], [Theory])
- Test runner integration
- Can reference other projects to test them

---

### NuGet Package Dependencies Explained

Each project needs specific NuGet packages based on its responsibilities. Here's why each package is needed:

#### UserManagement.Core
**Packages:** None required initially

**Why:**
- Pure domain models - just C# classes
- No external framework dependencies needed
- Keeps the domain layer clean and framework-agnostic
- May add packages later for validation (FluentValidation) if needed

#### UserManagement.Data
**Required Packages:**

**Microsoft.EntityFrameworkCore (8.0.x)**
- Core EF Core functionality
- Provides DbContext base class
- LINQ query translation
- Change tracking
- Entity mapping

**Microsoft.EntityFrameworkCore.SqlServer (8.0.x)**
- SQL Server database provider
- Translates LINQ to SQL Server T-SQL
- Handles SQL Server-specific data types
- Manages connections to SQL Server

**Microsoft.EntityFrameworkCore.Tools (8.0.x)**
- Provides `dotnet ef` command line tools
- Needed for creating and applying migrations
- Enables: `dotnet ef migrations add`, `dotnet ef database update`
- Development-time only dependency

**Microsoft.EntityFrameworkCore.Design (8.0.x)**
- Design-time components for EF Core
- Required for migration generation
- Scaffolding support
- Works with EF Core Tools

**Why All Four:**
- **EntityFrameworkCore:** Base ORM functionality
- **SqlServer:** Connects to SQL Server specifically
- **Tools:** Command-line migration tools
- **Design:** Design-time migration generation

Without these, you cannot:
- Define DbContext
- Connect to SQL Server
- Generate migrations
- Update the database

#### UserManagement.Api
**Required Packages:**

**Swashbuckle.AspNetCore (Already included)**
- Generates Swagger/OpenAPI documentation
- Provides Swagger UI for testing endpoints
- Auto-generates API documentation from controllers
- Industry standard for API documentation

**Microsoft.EntityFrameworkCore.Design (8.0.x)**
- Needed to run `dotnet ef` commands from the API project
- The API project is where we run migrations from
- Without this, `dotnet ef database update` won't work

**Why from API project:**
- API is the startup project
- Has connection string in appsettings.json
- Easier to run migrations from here
- Alternative would be running from Data project, but API is more convenient

#### UserManagement.Web
**Required Packages:**

None beyond what's included by default in MVC template

**Why:**
- Uses HttpClient (built into .NET)
- Uses standard MVC framework (included in template)
- No ORM needed (calls API instead of database)
- No testing frameworks needed here

#### UserManagement.UnitTests
**Required Packages:**

**xUnit (Already included)**
- Testing framework
- Provides [Fact], [Theory] attributes
- Test runner integration

**Moq (Latest)**
- Mocking framework for unit tests
- Creates fake implementations of interfaces
- Example: Mock IUserRepository to test UserService without database
- Industry standard for .NET mocking

**FluentAssertions (Latest)**
- Makes test assertions more readable
- Better error messages than default Assert
- Example: `result.Should().NotBeNull()` vs `Assert.NotNull(result)`
- Not required, but improves test code quality

**Microsoft.EntityFrameworkCore.InMemory (8.0.x)**
- In-memory database provider for testing
- Allows testing repository logic without SQL Server
- Fast test execution
- Each test gets fresh database

**Why These:**
- **xUnit:** Run tests
- **Moq:** Mock dependencies (repositories, services)
- **FluentAssertions:** Better test readability
- **InMemory:** Test database operations without SQL Server

#### UserManagement.IntegrationTests
**Required Packages:**

**xUnit (Already included)**
- Same as UnitTests

**Microsoft.AspNetCore.Mvc.Testing (8.0.x)**
- Provides WebApplicationFactory<T>
- Hosts the entire API in-memory for testing
- Makes real HTTP requests without starting actual server
- Standard approach for testing ASP.NET Core APIs

**FluentAssertions (Latest)**
- Same benefits as in unit tests

**Microsoft.EntityFrameworkCore.InMemory (8.0.x)**
- Same as UnitTests
- Integration tests use in-memory database
- Tests complete API flow including database operations

**Why These:**
- **xUnit:** Run tests
- **Mvc.Testing:** Host API in-memory, make HTTP requests
- **FluentAssertions:** Readable assertions
- **InMemory:** Database without SQL Server dependency

---

### Why Version 8.0.x for EF Core?

**The Rule:** EF Core version must match .NET version

- **.NET 8** → **EF Core 8.x**
- **.NET 10** → **EF Core 10.x**

**Why:**
- EF Core 10 uses .NET 10 APIs not available in .NET 8
- Installing EF Core 10 in a .NET 8 project causes compilation errors
- Framework and ORM versions must align

**Specific Version (8.0.24):**
- Latest stable patch release of EF Core 8
- Includes bug fixes and security updates
- Still compatible with .NET 8
- More stable than 8.0.0

---

### Project Reference Dependencies

**Why These References:**

```
UserManagement.Core ← UserManagement.Data
                    ← UserManagement.Api
                    ← UserManagement.Web (for DTOs)
                    ← UserManagement.UnitTests
                    
UserManagement.Data ← UserManagement.Api
                    ← UserManagement.UnitTests

UserManagement.Api  ← UserManagement.IntegrationTests
```

**UserManagement.Data references Core:**
- Needs entity definitions (User, Group, Permission)
- Needs repository interfaces to implement

**UserManagement.Api references Core + Data:**
- Needs entities and DTOs (Core)
- Needs repositories and DbContext (Data)
- Implements business logic using both

**UserManagement.Web references Core:**
- Needs DTOs to communicate with API
- Does NOT reference Data (never talks to database directly)
- Only knows about data shapes (DTOs), not database

**UserManagement.UnitTests references Core + Data + Api:**
- Tests services (Api)
- Tests repositories (Data)
- Uses entities (Core)

**UserManagement.IntegrationTests references Api:**
- Tests API endpoints
- API already references everything else
- Gets access to everything through API

---

### Why Not Just One Big Project?

**Problems with Single Project:**
-  No separation of concerns
-  Can't enforce architectural boundaries
-  Harder to test in isolation
-  Accidentally mixing UI code with business logic
-  Can't reuse components easily
-  Difficult to understand responsibilities

**Benefits of Multiple Projects:**
-  Clear boundaries between layers
-  Core has zero dependencies (pure domain)
-  Each layer can be tested independently
-  Compiler enforces architecture (Data can't reference API)
-  Can reuse Core/Data in other applications
-  Easier to understand and maintain

---

**Decision:** Did not implement authentication or authorization mechanisms (no login, no JWT tokens, no role-based access control).

### Rationale

**Assessment Scope**
- Authentication and authorization are not mentioned in the requirements
- The three tasks focus on: database design, API development, and UI development
- No requirement asks to "secure" endpoints or "restrict access"

**Time and Focus**
- Allows more time to focus on what is being evaluated: clean code, database design, and maintainability
- Authentication adds significant complexity (token management, middleware, security considerations)
- Better to excel at required features than add unrequested ones

**Ease of Testing**
- Integration tests can directly call endpoints without authentication setup
- Simplifies the "clone and run" requirement
- Reduces setup steps for reviewers

**Easy to Add Later**
- The architecture supports adding authentication as a future enhancement
- Service and repository layers are already in place
- Would integrate cleanly with ASP.NET Core's authentication middleware

**Not a Production Decision**
- This choice is specific to the assessment context
- A production system would absolutely need authentication and authorization
- The decision prioritizes demonstrating core development skills over security features not requested in scope

---

## No End-to-End UI Tests

**Decision:** Implemented unit tests and API integration tests, but did not include end-to-end UI tests with tools like Playwright or Selenium.

### Rationale

**Assessment Requirements**
- Requirements specify "Unit Tests and Integration Tests where appropriate"
- End-to-end tests are not explicitly mentioned
- Visual design is explicitly stated as "not assessed"

**Complexity vs. Value**
- E2E testing tools require additional setup (browser drivers, test infrastructure)
- E2E tests are more fragile and harder to maintain
- The UI is described as "simple" with focus on "functionality and structure"
- Given the scope, comprehensive unit + integration tests provide better coverage of critical functionality

**"Clone and Run" Requirement**
- Assessment asks for projects that can be "cloned from GitHub, built and run locally"
- E2E tests would require additional software installation (Chrome/Firefox drivers, Playwright)
- Adds friction to the evaluation process

**Testing Pyramid**
- Unit tests verify business logic in isolation (fast, focused)
- Integration tests verify the API layer with real database operations (still relatively fast)
- E2E tests would test through the browser (slow, brittle)
- For this scope, the first two layers provide sufficient confidence

**Where Effort Went Instead**
- Comprehensive integration tests using WebApplicationFactory
- Tests that verify complete workflows (Create → Read → Update → Delete)
- Focus on testing the API contract, which is what the UI depends on

**Acknowledgment**
- E2E tests have value in production applications
- They catch integration issues between frontend and backend
- For this assessment, the trade-off favored depth over breadth

---

## Testing Strategy

**Decision:** Used WebApplicationFactory for integration tests with an in-memory database, and Moq for unit test mocking.

### Rationale

**WebApplicationFactory Benefits**
- Standard approach for testing ASP.NET Core applications
- Hosts the entire API in-memory for testing
- Tests the complete stack: routing, middleware, controllers, services, database
- Fast execution compared to running actual servers
- No external dependencies required

**In-Memory Database Choice**
- Tests run quickly without needing SQL Server
- Each test gets a fresh database state
- Prevents test interference
- No cleanup required between tests
- Simplifies CI/CD pipeline (no database server needed)

**Testing Framework Selection**
- **xUnit:** Modern, widely adopted in .NET Core ecosystem, supports parallel test execution
- **FluentAssertions:** Makes test assertions more readable and provides better error messages
- **Moq:** Industry standard for mocking in .NET, easy to use and well-documented

**Coverage Strategy**
- Unit tests focus on business logic in services
- Integration tests focus on API endpoints and complete workflows
- Tests verify both happy paths and error scenarios
- Validates HTTP status codes, response payloads, and database state

---

## Technology Stack Choices

### ASP.NET Core (.NET 8)

**Rationale:**
- Modern, cross-platform framework
- Better performance than .NET Framework
- Long-term support (LTS) release
- Latest features and best practices
- Strong community and ecosystem

**Why Not .NET Framework:**
- .NET Framework is legacy and Windows-only
- Microsoft recommends .NET Core for new projects
- Better alignment with modern development practices

#### .NET 8 vs .NET 10 (LTS)

**Decision:** Used .NET 8 instead of the newer .NET 10 (LTS).

**Rationale:**

**Maturity and Stability**
- .NET 10 was released in November 2025, making it relatively new (3 months old as of February 2026)
- .NET 8 has been in production use for over a year, with more proven stability
- While both are LTS releases, .NET 8 has had more time for bug fixes and refinements
- Assessment projects benefit from using battle-tested platforms

**Industry Adoption**
- Most production applications are currently on .NET 8 (LTS)
- .NET 10 adoption is still ramping up in the industry
- Interviewers and reviewers are more likely to have hands-on experience with .NET 8
- Job postings and company tech stacks are predominantly using .NET 8

**Ecosystem Maturity**
- .NET 8 has mature tooling, extensive documentation, and proven libraries
- Larger community knowledge base (Stack Overflow, blogs, tutorials)
- Third-party packages have had more time to stabilize and be tested on .NET 8
- NuGet package ecosystem is more mature for .NET 8

**Risk Mitigation**
- Newer frameworks can have undiscovered issues or edge cases
- Using .NET 8 reduces the risk of encountering framework-related bugs during development
- More stable platform means less time troubleshooting framework issues, more time focusing on assessment requirements

**Assessment Context**
- The assessment evaluates code quality, architecture, and database design
- .NET 10's new features (C# 14, AVX10.2, enhanced NativeAOT) aren't necessary for this project
- Using a stable, well-documented platform demonstrates production-ready thinking
- Shows prioritization of reliability over having the absolute latest version

**Version Compatibility**
- EF Core 8.x pairs with .NET 8 and is very stable
- EF Core 10.x requires .NET 10 and is newer with less production history
- Staying on .NET 8 ensures all dependencies are proven and compatible

**Support Timeline**
- .NET 8: Supported until November 2026 (sufficient for this assessment and portfolio use)
- .NET 10: Supported until November 2028 (longer support, but 3 months old)
- Both are LTS releases; the difference is maturity, not support duration

**Alternative Considered**
- .NET 10 demonstrates staying current with latest releases
- Could show awareness of newest features
- However, for this assessment, choosing the more established LTS version demonstrates sound judgment about when to adopt new technology
- The architecture, code quality, and design patterns matter more than using the absolute latest framework version

### Entity Framework Core (Code First)

**Rationale:**
- Meets the explicit "Code First" requirement
- Strong typing and IntelliSense support
- Excellent migration tooling
- LINQ support for readable queries
- Industry standard ORM for .NET

**Approach:**
- Entities defined as C# classes
- Relationships configured via Fluent API
- Migrations for version control of schema
- Seed data included in initial migration

### SQL Server (LocalDB)

**Rationale:**
- Meets the requirement for SQL database
- LocalDB simplifies local development (no separate SQL Server installation)
- Easy to run locally for reviewers
- Straightforward migration path to full SQL Server for production

**Why Not Other Databases:**
- PostgreSQL or MySQL would work but require separate installation
- SQLite is limited for demonstrating complex relationships
- SQL Server aligns with typical .NET development environments

### Swagger/Swashbuckle

**Rationale:**
- Provides interactive API documentation
- Allows testing endpoints directly in browser
- Auto-generated from code (no manual documentation maintenance)
- Industry standard for REST API documentation
- Minimal setup required (single NuGet package)

---

## Architecture Principles Applied

### SOLID Principles

- **Single Responsibility:** Each class has one reason to change
- **Open/Closed:** Open for extension, closed for modification
- **Liskov Substitution:** Interfaces used for abstraction
- **Interface Segregation:** Specific interfaces for specific needs
- **Dependency Inversion:** Depend on abstractions, not concretions

### Clean Architecture Concepts

- **Layered Architecture:** Core → Data → API → Web
- **Dependency Flow:** Outer layers depend on inner layers
- **DTOs:** API boundaries use data transfer objects
- **Separation of Concerns:** Each layer has distinct responsibilities

### Design Patterns

- **Repository Pattern:** Abstracts data access
- **Dependency Injection:** Loose coupling between components
- **Factory Pattern:** WebApplicationFactory for test setup

---

## Trade-offs and Alternatives Considered

### Repository Pattern
- **Alternative:** Direct DbContext usage in services
- **Trade-off:** Repository adds abstraction but improves testability
- **Decision:** Benefits outweigh the additional layer for this application size

### DTOs vs. Direct Entity Exposure
- **Alternative:** Return entities directly from API
- **Trade-off:** DTOs add mapping code but provide API stability
- **Decision:** DTOs are best practice for public APIs

### Monolithic vs. Microservices
- **Alternative:** Separate API into multiple microservices
- **Trade-off:** Microservices add complexity without benefit at this scale
- **Decision:** Monolithic architecture is appropriate for assessment scope

### MVC vs. Razor Pages (for UI)
- **Alternative:** Either works for Task 3
- **Trade-off:** MVC is more familiar, Razor Pages is more modern
- **Decision:** Either choice is valid; picked based on preference

---

## Summary

The technical decisions in this solution prioritize:

1. **Meeting Requirements** - Every choice directly addresses assessment criteria
2. **Clarity** - Code is readable and maintainable
3. **Best Practices** - Follows established patterns and conventions
4. **Simplicity** - Avoids over-engineering while demonstrating competence
5. **Testability** - Architecture supports comprehensive testing
6. **Reviewability** - Easy for assessors to clone, run, and evaluate

Each decision was made to balance these priorities while staying within the scope of the assessment tasks.
