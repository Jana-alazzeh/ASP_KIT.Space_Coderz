# July Team - KITSpace

## Overview
This is an ASP.NET Core 8.0 MVC web application for KITSpace, a tech community platform. The application includes features for courses, products/store, contact forms, user authentication, and more.

## Project Structure
- `July_Team/` - Main ASP.NET Core project
  - `Controllers/` - MVC and API controllers (thin controllers delegating to services)
  - `Models/` - Entity models and view models
  - `Views/` - Razor views
  - `Services/` - Business logic layer (ProductService, OrderService, UserRoleService)
  - `Repositories/` - Data access layer with generic repository pattern
  - `Exceptions/` - Custom exception classes for structured error handling
  - `Helpers/` - Utility classes including LoggingHelper
  - `wwwroot/` - Static files (CSS, JS, images)
  - `Resourses/` - Localization resources (English/Arabic)
  - `Migrations/` - Entity Framework migrations

## Architecture

### Clean Architecture Pattern
The project follows a layered architecture for separation of concerns:

1. **Controllers** - Handle HTTP requests, validate input, delegate to services
2. **Services** - Business logic layer (IProductService, IOrderService, IUserRoleService)
3. **Repositories** - Data access layer (generic IRepository<T> pattern)
4. **Models** - Domain entities and view models

### Dependency Injection
All services are registered in Program.cs:
- `IRepository<T>` -> `Repository<T>` (Scoped)
- `IProductService` -> `ProductService` (Scoped)
- `IOrderService` -> `OrderService` (Scoped)
- `IUserRoleService` -> `UserRoleService` (Scoped)

### Error Handling
Custom exceptions in `Exceptions/AppException.cs`:
- `AppException` - Base exception with error codes
- `NotFoundException` - Resource not found (404)
- `ValidationException` - Input validation errors (400)
- `BusinessRuleException` - Business rule violations (422)

### Logging
Console logging via `Helpers/LoggingHelper.cs`:
- LogInfo, LogWarning, LogError, LogDebug, LogSuccess
- Colored output with timestamps for development debugging

### Enhanced Role Management
`UserRoleService` provides:
- Detailed role information with permissions and control levels
- Role definitions: Admin (100), Trainer (70), Member (30)
- Badge colors and descriptions for UI display

## Tech Stack
- ASP.NET Core 8.0 MVC
- Entity Framework Core with PostgreSQL (Npgsql)
- ASP.NET Core Identity for authentication
- Swagger for API documentation
- Localization support (English/Arabic)

## Running the Application
The application runs on port 5000 with the workflow command:
```
cd July_Team && dotnet run --urls "http://0.0.0.0:5000"
```

## Database
Uses Replit's built-in PostgreSQL database. The DATABASE_URL environment variable is automatically configured and the application migrates the database on startup.

## API Documentation
Swagger UI is available at `/swagger` in development mode.

## Default Admin User
- Email: admin@kitspace.com
- Password: AdminP@ss123

## Recent Changes
- Added Service Layer (IProductService, IOrderService, IUserRoleService)
- Added Repository Pattern (IRepository<T>, Repository<T>)
- Added Custom Exceptions for structured error handling
- Added LoggingHelper for console debugging
- Enhanced UserController with role details and permissions display
- Migrated from SQL Server to PostgreSQL for Replit compatibility
- Configured to run on port 5000
- Added automatic database migration on startup
