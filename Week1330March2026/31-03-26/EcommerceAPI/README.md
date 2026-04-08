# 🛒 EcommerceAPI — .NET 9 Web API

A production-ready E-Commerce Product API built with **ASP.NET Core 9**, **Entity Framework Core**, and **JWT Authentication**.

---

## 🏗️ Project Structure

```
EcommerceAPI.sln
├── EcommerceAPI/                     # Main API project
│   ├── Controllers/
│   │   ├── AuthController.cs         # Login → JWT token
│   │   └── ProductsController.cs     # Full CRUD for products
│   ├── Data/
│   │   └── AppDbContext.cs           # EF Core DbContext
│   ├── Models/
│   │   └── Product.cs                # Product entity
│   ├── Program.cs                    # App entry point + DI
│   ├── appsettings.json              # Config (JWT, DB connection)
│   └── EcommerceAPI.csproj
└── EcommerceAPI.Tests/               # xUnit test project
    ├── ProductTests.cs               # Product CRUD tests
    ├── AuthControllerTests.cs        # Auth controller tests
    └── EcommerceAPI.Tests.csproj
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB, Express, or full) — or switch to InMemory for dev

### 1. Restore packages
```bash
dotnet restore
```

### 2. Update your connection string
Edit `EcommerceAPI/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=EcommerceDB;Trusted_Connection=True;"
}
```

### 3. Run EF Core migrations
```bash
dotnet ef migrations add Init --project EcommerceAPI
dotnet ef database update --project EcommerceAPI
```

### 4. Run the API
```bash
dotnet run --project EcommerceAPI
```

### 5. Open Swagger UI
```
https://localhost:5001/swagger
```

---

## 🔐 Authentication

1. Call `POST /api/auth/login?username=admin`
2. Copy the `token` from the response
3. In Swagger, click **Authorize** → enter `Bearer <your-token>`
4. Now you can call admin-protected endpoints

---

## 🛣️ API Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | /api/auth/login | — | Get JWT token |
| GET | /api/v1/products | Public | List all products |
| GET | /api/v1/products/{id} | Public | Get by ID |
| GET | /api/v1/products/category/{cat} | Public | Filter by category |
| POST | /api/v1/products | 🔒 Admin | Create product |
| PUT | /api/v1/products/{id} | 🔒 Admin | Update product |
| DELETE | /api/v1/products/{id} | 🔒 Admin | Delete product |

---

## 🧪 Running Tests

```bash
dotnet test EcommerceAPI.Tests
```

Tests use **in-memory database** — no SQL Server needed for testing.

---

## ⚙️ Configuration

`appsettings.json` JWT settings:
```json
"Jwt": {
  "Key": "THIS_IS_A_SECURE_KEY_1234567890123456",
  "Issuer": "EcommerceAPI",
  "Audience": "EcommerceUsers"
}
```
> ⚠️ **Change the JWT Key** before deploying to production!

---

## 📦 NuGet Packages

| Package | Purpose |
|---------|---------|
| Microsoft.AspNetCore.Authentication.JwtBearer | JWT auth middleware |
| Microsoft.EntityFrameworkCore.SqlServer | SQL Server provider |
| Microsoft.EntityFrameworkCore.InMemory | In-memory DB for tests |
| Microsoft.EntityFrameworkCore.Tools | EF Core CLI tools |
| Swashbuckle.AspNetCore | Swagger / OpenAPI |
| xunit | Test framework |
| Moq | Mocking library |

---

## 🏷️ Key Design Decisions

- **Route Versioning**: `api/v1/products` ensures forward compatibility
- **Route Constraints**: `{id:int}` prevents conflicts between ID and category routes
- **Async/Await**: All DB operations use async methods for scalability
- **Model Validation**: DataAnnotations + ModelState validation
- **Structured Logging**: ILogger injected in all controllers
