# July Team - KITSpace

## Overview
This is an ASP.NET Core 8.0 MVC web application for KITSpace, a tech community platform. The application includes features for courses, products/store, contact forms, user authentication, and more.

## Project Structure
- `July_Team/` - Main ASP.NET Core project
  - `Controllers/` - MVC and API controllers
  - `Models/` - Entity models and view models
  - `Views/` - Razor views
  - `wwwroot/` - Static files (CSS, JS, images)
  - `Resourses/` - Localization resources (English/Arabic)
  - `Migrations/` - Entity Framework migrations

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
- Migrated from SQL Server to PostgreSQL for Replit compatibility
- Configured to run on port 5000
- Added automatic database migration on startup
