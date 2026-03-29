# LogiTrack
[![Build LogiTrack](https://github.com/JMLOSP/LogiTrack/actions/workflows/build.yml/badge.svg?branch=develop)](https://github.com/JMLOSP/LogiTrack/actions/workflows/build.yml)
[![Build LogiTrack](https://github.com/JMLOSP/LogiTrack/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/JMLOSP/LogiTrack/actions/workflows/build.yml)

Order Management System for LogiTrack, a logistics software platform.

> ⚠️ This project was developed as part of a technical assessment (capstone exam) to demonstrate API design, security, and performance optimization skills using ASP.NET Core.

---

## 🧱 Tech Stack
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- JWT Authentication
- IMemoryCache

---

## 📦 Features

### Inventory Management
- Full CRUD operations for inventory items
- Validation for input data and resource existence

### Order Management
- Create orders with multiple inventory items
- Validation of item references before order creation
- Relationship handling between orders and inventory

### Authentication & Authorization
- JWT-based authentication
- Role-based access control (e.g., **Manager** role)
- Protected endpoints with proper HTTP status responses

### Data Persistence
- Database persistence using Entity Framework Core
- Data remains consistent across application restarts

### Caching & Performance
- IMemoryCache used for frequently accessed data
- Cache invalidation after create/update/delete operations
- Optimized read queries using `AsNoTracking()`
- DTO projection to reduce payload size and avoid circular references
- Basic response time measurement using `Stopwatch`

### Error Handling
- Proper use of HTTP status codes (400, 401, 403, 404)
- Clear and consistent error messages

### API Documentation
- Swagger enabled for testing and exploration

---

## 🗄️ Database
- SQLite database managed via EF Core migrations
- Database files are excluded from source control

---

## 📁 Project Structure
- `Models/` → domain entities
- `Data/` → DbContext and database configuration
- `Migrations/` → EF Core migrations
- `Controllers/` → API endpoints
- `DTOs/` → response and request models

---

## 🚀 Run the project
```bash
dotnet restore
dotnet ef database update
dotnet run