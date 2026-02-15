# Running Locally

Complete guide to setting up, running, and testing the User Management System on your local machine.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Running the Application](#running-the-application)
- [Testing](#testing)
- [API Documentation](#api-documentation)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

Before running this application, ensure you have the following installed:

### Required

- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** or later
- **[SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)**
  - Included with Visual Studio 2022
  - Can be installed separately with SQL Server Express
- **Git** - For cloning the repository

### Optional

- **Visual Studio 2022** - For GUI development experience
- **VS Code** - Lightweight alternative with C# extension
- **JetBrains Rider** - Another popular .NET IDE

### Verifying Prerequisites

Check that everything is installed correctly:

```bash
# Check .NET SDK version (should be 8.0 or higher)
dotnet --version

# Check if SQL Server LocalDB is installed
sqllocaldb info

# If LocalDB isn't installed, you can install it from:
# https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb
```

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/UserManagementSystem.git
cd UserManagementSystem
```

### 2. Restore NuGet Packages

```bash
# From the solution root directory
dotnet restore
```

This will download all required NuGet packages for all projects in the solution.

### 3. Database Setup

The application uses EF Core migrations to create and seed the database.

#### Option A: Automatic Setup (Recommended)

The database will be created automatically on first run. Just start the API project and the migrations will apply automatically:

```bash
cd src/UserManagement.Api
dotnet run
```

#### Option B: Manual Setup

If you prefer to create the database before running:

```bash
# Navigate to the API project
cd src/UserManagement.Api

# Apply migrations and create database
dotnet ef database update

# Return to solution root
cd ../..
```

#### Verify Database Creation

After setup, verify the database was created:

```bash
# List LocalDB instances
sqllocaldb info

# If "mssqllocaldb" is stopped, start it
sqllocaldb start mssqllocaldb
```

### 4. Configuration (Optional)

The default configuration uses LocalDB. If you need to change database settings:

**Edit:** `src/UserManagement.Api/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=UserManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

For the Web UI to communicate with the API:

**Edit:** `src/UserManagement.Web/appsettings.json`

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7113"
  }
}
```

---

## Running the Application

You have several options for running the application:

### Option 1: Run Both Projects Simultaneously (Recommended)

This allows you to use the full application (API + Web UI).

#### Using Visual Studio 2022:

1. Open `UserManagementSystem.sln` in Visual Studio
2. Right-click the solution in Solution Explorer
3. Select **"Configure Startup Projects"**
4. Choose **"Multiple startup projects"**
5. Set both `UserManagement.Api` and `UserManagement.Web` to **"Start"**
6. Click OK
7. Press **F5** or click **"Start"**

Both projects will launch in separate browser windows.

#### Using Command Line:

Open two terminal windows:

**Terminal 1 - Start the API:**
```bash
cd src/UserManagement.Api
dotnet run
```

Wait until you see output like:
```
Now listening on: https://localhost:7113
```

**Terminal 2 - Start the Web UI:**
```bash
cd src/UserManagement.Web
dotnet run
```

Wait until you see output like:
```
Now listening on: https://localhost:7190
```

### Option 2: Run API Only

If you only want to test the API:

```bash
cd src/UserManagement.Api
dotnet run
```

Access the Swagger documentation at: **`https://localhost:7113/swagger`**

### Option 3: Run Web UI Only

If the API is already running:

```bash
cd src/UserManagement.Web
dotnet run
```

Browse to: **`https://localhost:7190`**

**Note:** The Web UI requires the API to be running. It will show errors if it cannot connect to the API.

### Accessing the Application

Once running:

- **API Swagger Documentation:** `https://localhost:7113/swagger`
- **Web UI:** `https://localhost:7190`

**Port numbers may vary** - check the console output for actual URLs.

---

## Testing

The solution includes comprehensive unit and integration tests.

### Running All Tests

From the solution root directory:

```bash
# Run all tests in the solution
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with code coverage (if coverage tools are installed)
dotnet test /p:CollectCoverage=true
```

### Running Specific Test Projects

**Run Unit Tests Only:**
```bash
cd tests/UserManagement.UnitTests
dotnet test
```

**Run Integration Tests Only:**
```bash
cd tests/UserManagement.IntegrationTests
dotnet test
```

### Running Tests in Visual Studio

1. Open **Test Explorer** (Test → Test Explorer)
2. Click **"Run All Tests"** in the toolbar
3. View results in the Test Explorer window
4. Right-click individual tests to run them separately
5. Double-click a test to view its code

### Understanding Test Output

**Successful test run:**
```
Passed!  - Failed:     0, Passed:    45, Skipped:     0, Total:    45
```

**Failed test run:**
```
Failed!  - Failed:     2, Passed:    43, Skipped:     0, Total:    45
```

Failed tests will show detailed error messages explaining what went wrong.

### Test Coverage

The test projects include:

**Unit Tests (`UserManagement.UnitTests`):**
- Service layer business logic
- Repository implementations (with mocked DbContext)
- DTO mapping logic
- Validation rules
- Edge cases and error handling

**Integration Tests (`UserManagement.IntegrationTests`):**
- Complete API workflows
- HTTP request/response validation
- Database operations with in-memory database
- Status code verification
- End-to-end CRUD operations

---

## API Documentation

### Swagger UI

Once the API is running, access interactive documentation:

**URL:** `https://localhost:7113/swagger`

The Swagger UI allows you to:
- View all available endpoints
- See request/response schemas
- Try out API calls directly in the browser
- View example requests and responses

### Available Endpoints

#### User Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create new user |
| PUT | `/api/users/{id}` | Update existing user |
| DELETE | `/api/users/{id}` | Delete user |

#### Reporting

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users/count` | Get total user count |
| GET | `/api/users/count-by-group` | Get user count per group |

### Example API Requests

**Get All Users:**
```bash
curl -X GET "https://localhost:7113/api/users" -H "accept: application/json"
```

**Get User by ID:**
```bash
curl -X GET "https://localhost:7113/api/users/1" -H "accept: application/json"
```

**Create User:**
```bash
curl -X POST "https://localhost:7113/api/users" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "groupIds": [1, 2]
  }'
```

**Update User:**
```bash
curl -X PUT "https://localhost:7113/api/users/1" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Smith",
    "email": "john.smith@example.com",
    "groupIds": [1]
  }'
```

**Delete User:**
```bash
curl -X DELETE "https://localhost:7113/api/users/1"
```

**Get Total User Count:**
```bash
curl -X GET "https://localhost:7113/api/users/count"
```

**Get Users Per Group:**
```bash
curl -X GET "https://localhost:7113/api/users/count-by-group"
```

### Response Formats

**Successful Response (200 OK):**
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "groups": [
    {
      "id": 1,
      "name": "Admin"
    }
  ],
  "createdAt": "2024-01-15T10:30:00Z"
}
```

**Error Response (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": ["The Email field is required."]
  }
}
```

---

## Troubleshooting

### Database Issues

#### Problem: "Cannot open database" or "Login failed"

**Solution:**
```bash
# Check if LocalDB is running
sqllocaldb info

# Start LocalDB if stopped
sqllocaldb start mssqllocaldb

# If issues persist, delete and recreate the database
cd src/UserManagement.Api
dotnet ef database drop --force
dotnet ef database update
```

#### Problem: "No migrations found"

**Solution:**
```bash
cd src/UserManagement.Api

# Check if migrations exist
dotnet ef migrations list

# If no migrations, create initial migration
dotnet ef migrations add InitialCreate

# Apply migrations
dotnet ef database update
```

#### Problem: "Connection string not found"

**Solution:** Verify `appsettings.json` exists in the API project with correct connection string.

---

### Port Conflicts

#### Problem: "Address already in use" or port conflict errors

**Solution:** Edit port numbers in `Properties/launchSettings.json`:

**For API (`src/UserManagement.Api/Properties/launchSettings.json`):**
```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:7113;http://localhost:5168"
    }
  }
}
```

**For Web (`src/UserManagement.Web/Properties/launchSettings.json`):**
```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:7190;http://localhost:5022"
    }
  }
}
```

After changing ports, update the API URL in Web's `appsettings.json`.

---

### Web UI Cannot Connect to API

#### Problem: Web UI shows "Cannot connect to API" errors

**Solution:**

1. Verify the API is running:
   ```bash
   curl https://localhost:7113/api/users
   ```

2. Check the API URL in `src/UserManagement.Web/appsettings.json`:
   ```json
   {
     "ApiSettings": {
       "BaseUrl": "https://localhost:7113"
     }
   }
   ```

3. Ensure firewall isn't blocking local connections

4. Try using `http://localhost:7113` instead of `https://localhost:7113`

---

### Build Errors

#### Problem: "Could not find project or directory"

**Solution:**
```bash
# Ensure you're in the solution root
cd UserManagementSystem

# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

#### Problem: Package restore failures

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore
```

---

### Test Failures

#### Problem: Integration tests fail with database errors

**Solution:**

Integration tests use an in-memory database. If tests fail:

1. Check that test project references:
   - `Microsoft.EntityFrameworkCore.InMemory` or
   - `Microsoft.EntityFrameworkCore.Sqlite` (for in-memory mode)

2. Verify test database initialization in test fixtures

3. Run tests individually to isolate issues:
   ```bash
   dotnet test --filter "FullyQualifiedName~UsersControllerTests"
   ```

---

### SSL Certificate Issues

#### Problem: "The SSL connection could not be established" or certificate errors

**Solution:**

**Windows:**
```bash
# Trust the .NET development certificate
dotnet dev-certs https --trust
```

**macOS:**
```bash
dotnet dev-certs https --trust
```

**Linux:**
```bash
# Export certificate
dotnet dev-certs https --export-path ~/.aspnet/https/aspnetapp.pfx --password password

# Trust certificate (method varies by distro)
```

Or temporarily use HTTP instead of HTTPS by modifying `launchSettings.json`.

---

### LocalDB Installation Issues

#### Problem: LocalDB not installed or not found

**Solution:**

**Install LocalDB:**
1. Download [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
2. Choose "Download Media" → "LocalDB"
3. Run installer

**Or install with Visual Studio:**
- Visual Studio Installer → Modify → Individual Components
- Check "SQL Server Express LocalDB"

**Verify installation:**
```bash
sqllocaldb info
```

---

### Still Having Issues?

If you're still experiencing problems:

1. **Check Console Output:** Look for detailed error messages
2. **Check Logs:** Application logs may contain additional details
3. **Clean Solution:** Delete `bin` and `obj` folders, then rebuild
4. **Restart IDE:** Sometimes Visual Studio needs a restart
5. **Check .NET Version:** Ensure you have .NET 8 SDK installed

---

## Additional Resources

### Useful Commands

```bash
# Build entire solution
dotnet build

# Clean build artifacts
dotnet clean

# Run specific project
dotnet run --project src/UserManagement.Api

# Watch for changes and auto-reload (development)
dotnet watch --project src/UserManagement.Api

# Create new migration
cd src/UserManagement.Api
dotnet ef migrations add MigrationName

# View applied migrations
dotnet ef migrations list

# Revert last migration
dotnet ef migrations remove

# Generate SQL script from migrations
dotnet ef migrations script
```

### Development Tips

**Hot Reload:**
```bash
# Run with hot reload enabled (restarts on code changes)
dotnet watch run --project src/UserManagement.Api
```

**Multiple Terminal Windows:**
- Use tools like Windows Terminal, iTerm2, or tmux
- Easier to monitor both API and Web UI simultaneously

**Viewing Database:**
- Use SQL Server Management Studio (SSMS)
- Visual Studio's SQL Server Object Explorer
- Azure Data Studio (cross-platform)

**Connection String for SSMS:**
```
Server=(localdb)\mssqllocaldb;Database=UserManagementDb;Integrated Security=true;
```

---

## Next Steps

Once you have the application running:

1. **Explore the Swagger UI** - Try out the API endpoints
2. **Use the Web Interface** - Create, edit, and delete users
3. **Run the Tests** - Verify everything works as expected
4. **Check the Database** - View the created tables and relationships
5. **Review the Code** - Understand the architecture and implementation

For more information:
- [Solution Structure](SOLUTION_STRUCTURE.md) - Understand project organization
- [Key Technical Decisions](KEY_DECISIONS.md) - Learn about architectural choices