# Key Technical Decisions

This document explains the architectural choices made in this solution and the reasoning behind them.

## Table of Contents

- [Custom Entities vs ASP.NET Identity](#custom-entities-vs-aspnet-identity)
- [Separate API and UI Projects](#separate-api-and-ui-projects)
- [Repository Pattern with Service Layer](#repository-pattern-with-service-layer)
- [No Authentication/Authorization](#no-authenticationauthorization)
- [No End-to-End UI Tests](#no-end-to-end-ui-tests)
- [Testing Strategy](#testing-strategy)
- [Technology Stack Choices](#technology-stack-choices)

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

## No Authentication/Authorization

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
