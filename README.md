# LogiTrack

Order Management System for LogiTrack, a logistics software platform.

## Tech Stack
- ASP.NET Core Web API
- Entity Framework Core
- SQLite

## Database
SQLite database is created via EF Core migrations.
Database files are excluded from source control.

## Features in Part 1
- Inventory item model
- Order model
- Basic order item add/remove logic
- EF Core database integration
- Initial data seeding
- Swagger enabled

## Project Structure
- `Models/` → domain classes
- `Data/` → EF Core DbContext
- `Migrations/` → database migrations

## Run the project
```bash
dotnet restore
dotnet ef database update
dotnet 