# EventHub – Event Booking & Management System
**Capgemini Week-12 Assessment | ASP.NET Core 9 + Razor Pages**

---

## Tech Stack

| Layer       | Technology                                        |
|-------------|---------------------------------------------------|
| Backend API | ASP.NET Core 9 Web API                            |
| ORM         | Entity Framework Core 9 (SQL Server / LocalDB)    |
| Auth        | JWT Bearer (configured in `appsettings.json`)     |
| Mapping     | AutoMapper 13                                     |
| Validation  | Data Annotations + custom `[FutureDate]`          |
| Frontend    | Razor Pages + Bootstrap 5.3 + Vanilla JS          |
| API Docs    | Swagger / OpenAPI                                 |

---

## Prerequisites — Install These First

| Tool | Download |
|------|----------|
| .NET 9 SDK | https://dotnet.microsoft.com/download/dotnet/9.0 |
| VS Code | https://code.visualstudio.com |
| SQL Server Express (with LocalDB) | https://www.microsoft.com/en-us/sql-server/sql-server-downloads |

**VS Code Extensions** (VS Code will prompt you automatically):
- C# Dev Kit — `ms-dotnettools.csdevkit`
- C# — `ms-dotnettools.csharp`

---

## Project Structure

```
EventBookingSystem/
├── .vscode/
│   ├── launch.json               ← F5 runs both projects
│   ├── tasks.json                ← build / migrate / update tasks
│   └── extensions.json           ← auto-prompts extension install
│
├── EventBooking.API/             ← ASP.NET Core 9 Web API
│   ├── Controllers/              ← Events, Bookings, Token
│   ├── Data/                     ← AppDbContext (EF Core)
│   ├── DTOs/                     ← EventDto, BookingDto, CreateXxxDto
│   ├── Entities/                 ← Event, Booking
│   ├── Mappings/                 ← AutoMapper profile
│   ├── Validation/               ← [FutureDate] attribute
│   └── appsettings.json          ← DB connection + JWT secret
│
├── EventBooking.Web/             ← Razor Pages frontend
│   ├── Pages/
│   │   ├── Index.cshtml          ← Home / hero banner
│   │   ├── Events/Index.cshtml   ← Event cards + booking modal
│   │   ├── Bookings/MyBookings   ← Bookings table + cancel
│   │   └── Auth/Token            ← Get JWT token (dev helper)
│   └── wwwroot/
│       ├── css/site.css          ← Bootstrap overrides
│       └── js/                   ← events.js, bookings.js, token.js
│
├── EventBookingSystem.code-workspace  ← Open THIS in VS Code
├── seed.sql                           ← 6 sample events
└── README.md
```

---

## How to Run in VS Code

### 1. Open the workspace
Double-click **`EventBookingSystem.code-workspace`**
(or in VS Code: `File → Open Workspace from File`)

You will see both `EventBooking.API` and `EventBooking.Web` in the Explorer panel.
Click **Install All** if VS Code prompts for extensions.

---

### 2. Open a Terminal and restore packages
```bash
cd EventBooking.API
dotnet restore
cd ../EventBooking.Web
dotnet restore
```

---

### 3. Trust the HTTPS dev certificate (first time only)
```bash
dotnet dev-certs https --trust
```

---

### 4. Install EF tools (if not already installed)
```bash
dotnet tool install --global dotnet-ef
```

---

### 5. Create the database
```bash
cd EventBooking.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```
This creates `EventBookingDb` in your LocalDB instance automatically.

---

### 6. Add sample data (optional but recommended)
Open **Azure Data Studio** or **SSMS**, connect to:
```
Server:         (localdb)\mssqllocaldb
Authentication: Windows Authentication
```
Open and run `seed.sql` — adds 6 sample events.

---

### 7. Run both projects
Press **F5** in VS Code and select **"🚀 Run API + Web"**.

| Project | URL |
|---------|-----|
| API     | https://localhost:7100 |
| Swagger | https://localhost:7100/swagger |
| Web App | https://localhost:7200 ← opens automatically |

---

### 8. Demo the full flow
1. Go to `https://localhost:7200`
2. Click **Get Token** → enter a User ID and Name → **Generate & Save Token**
3. Click **Events** → browse cards → **Book Now** → pick seats → **Confirm Booking**
4. Click **My Bookings** → view your bookings → **Cancel** one

---

## API Reference

| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| GET | `/api/events` | Public | List all events |
| GET | `/api/events/{id}` | Public | Get one event |
| POST | `/api/events` | JWT | Create event |
| PUT | `/api/events/{id}` | JWT | Update event |
| DELETE | `/api/events/{id}` | JWT | Delete event |
| GET | `/api/bookings` | JWT | My bookings |
| POST | `/api/bookings` | JWT | Book tickets |
| DELETE | `/api/bookings/{id}` | JWT | Cancel booking |
| POST | `/api/token` | Public | Get test JWT |

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| `dotnet ef` not found | `dotnet tool install --global dotnet-ef` |
| LocalDB not found | Reinstall SQL Server Express, tick LocalDB option |
| Browser shows cert error | `dotnet dev-certs https --trust` then restart browser |
| Booking fails (401) | Go to **Get Token** page and generate a fresh token |
| Events not loading | Make sure the API is running on port 7100 |

---

## Assessment Requirements Checklist

| Requirement | File |
|---|---|
| EF Core – Event & Booking entities | `Entities/Event.cs`, `Entities/Booking.cs` |
| EF Core CRUD | `Controllers/EventsController.cs`, `BookingsController.cs` |
| DTOs | `DTOs/Dtos.cs` |
| AutoMapper | `Mappings/MappingProfile.cs` |
| JWT Authentication | `Program.cs` + `[Authorize]` attributes |
| Attribute Routing | `[Route]`, `[HttpGet]`, `[HttpPost]`, `[HttpDelete]` |
| Server-side Validation | `[Required]`, `[Range]`, `[FutureDate]` in DTOs |
| Bootstrap UI (cards + forms) | `Pages/Events/Index.cshtml` |
| Client-side JS Validation | `wwwroot/js/events.js` – `validateSeats()` |
| fetch() API Integration | `events.js`, `bookings.js`, `token.js` |
| Booking Confirmation | Dynamic success alert after POST |

---

*Capgemini Week-12 Assessment*
