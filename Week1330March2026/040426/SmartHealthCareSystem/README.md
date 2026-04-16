# Smart Health Care System

A comprehensive web-based hospital management system built with ASP.NET Core.

## Features

- 🏥 **User Management**: Support for Admin, Doctor, and Patient roles
- 👨‍⚕️ **Doctor & Department Management**: Organize doctors by departments
- 📅 **Appointment Booking**: Patients can book appointments with doctors
- 💊 **Prescription Management**: Doctors can create prescriptions for appointments
- 💳 **Billing System**: Generate bills for appointments with payment tracking
- 🔒 **Secure Authentication**: JWT-based authentication with role-based authorization

## Technology Stack

- **Frontend**: ASP.NET Core MVC with Razor Views, Bootstrap, modern UI with hospital theme
- **Backend**: ASP.NET Core Web API
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Architecture**: Repository Pattern, AutoMapper, DTOs

## Project Structure

```
SmartHealthCareSystem/
├── SmartHealthCareSystem.API/          # Web API Backend
│   ├── Controllers/                    # API Controllers
│   ├── Data/                          # Database Context
│   ├── DTOs/                          # Data Transfer Objects
│   ├── Middleware/                    # Exception Handling
│   ├── Models/                        # Entity Models
│   ├── Profiles/                      # AutoMapper Profiles
│   ├── Repositories/                  # Repository Pattern
│   └── Services/                      # Business Logic
└── SmartHealthCareSystem.MVC/          # MVC Frontend
    ├── Controllers/                   # MVC Controllers
    ├── DTOs/                         # Data Transfer Objects
    ├── Services/                     # API Service Layer
    ├── Views/                        # Razor Views
    └── wwwroot/                      # Static Files
```

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio Code or Visual Studio

### Installation

1. Clone the repository
2. Navigate to the project directory
3. Restore packages:
   ```bash
   dotnet restore
   ```

4. Update database connection string in `SmartHealthCareSystem.API/appsettings.json`

5. Run database migrations:
   ```bash
   dotnet ef database update --project SmartHealthCareSystem.API
   ```

6. Run the applications:
   - API: `dotnet run --project SmartHealthCareSystem.API`
   - MVC: `dotnet run --project SmartHealthCareSystem.MVC`

### Default URLs

- MVC Frontend: https://localhost:7189
- API Backend: https://localhost:5001

## Usage

1. Register as a new user or login
2. Browse doctors by department
3. Book appointments
4. View medical records and prescriptions
5. Manage billing (Admin/Doctor roles)

## API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login

### Resources
- `GET /api/users` - Get all users (Admin)
- `GET /api/departments` - Get all departments
- `GET /api/doctors` - Get all doctors
- `GET /api/appointments` - Get appointments
- `POST /api/appointments` - Book appointment
- `GET /api/prescriptions` - Get prescriptions
- `GET /api/bills` - Get bills

## Security

- JWT token-based authentication
- Role-based authorization (Admin, Doctor, Patient)
- Password hashing with BCrypt
- CORS enabled for frontend-backend communication

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.