# SmartHealthcare.Models - Dependency Injection Explanation

## ❌ NOT Registered as DI Service

**SmartHealthcare.Models is NOT registered in the Dependency Injection (DI) container.**

Rather, it's a **shared class library** that is **referenced as a project dependency** by both the API and Web projects.

---

## 📦 How SmartHealthcare.Models is Used

### 1. Project References (Compile-Time Dependency)

**SmartHealthcare.API.csproj:**
```xml
<ItemGroup>
  <ProjectReference Include="..\SmartHealthcare.Models\SmartHealthcare.Models.csproj" />
</ItemGroup>
```

**SmartHealthcare.Web.csproj:**
```xml
<ItemGroup>
  <ProjectReference Include="..\SmartHealthcare.Models\SmartHealthcare.Models.csproj" />
</ItemGroup>
```

This is a **compile-time reference**, not a runtime DI registration.

---

## 🔗 How It Works

### Classes from SmartHealthcare.Models are Used Via `using` Statements

**Examples from Code:**

**SmartHealthcare.API/Services/AuthService.cs:**
```csharp
using SmartHealthcare.Models.DTOs;      // ← Import DTOs
using SmartHealthcare.Models.Entities;  // ← Import Entities

public class AuthService : IAuthService
{
    public async Task<UserDTO?> RegisterAsync(RegisterDTO registerDTO)
    {
        // RegisterDTO comes from SmartHealthcare.Models.DTOs
        var user = new User()  // ← User entity from SmartHealthcare.Models
        {
            FullName = registerDTO.FullName,
            Email = registerDTO.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password),
            Role = registerDTO.Role,
            CreatedAt = DateTime.UtcNow
        };
        
        return _mapper.Map<UserDTO>(created);  // ← UserDTO from SmartHealthcare.Models
    }
}
```

**SmartHealthcare.Web/Controllers/AppointmentsController.cs:**
```csharp
using SmartHealthcare.Models.DTOs;  // ← Import DTOs

public class AppointmentsController : Controller
{
    public async Task<IActionResult> Create(CreateAppointmentDTO model)
    {
        // CreateAppointmentDTO comes from SmartHealthcare.Models.DTOs
        if (!ModelState.IsValid)
            return View("Book", model);
        
        // Uses AppointmentDTO to pass data to API
        var result = System.Text.Json.JsonSerializer.Deserialize<AppointmentDTO>(responseContent);
    }
}
```

---

## 🎯 What SmartHealthcare.Models Contains

### 1. **Entities/** - Database Models
```csharp
- User.cs          // Instantiated directly in AuthService
- Patient.cs       // Created in AuthService.RegisterAsync()
- Doctor.cs        // Created in AuthService.RegisterAsync()
- Appointment.cs   // Created by API endpoints
- Prescription.cs  // Used in services
- Medicine.cs      // Catalog
- Specialization.cs
- DoctorSpecialization.cs
```

**Used by:**
- Entity Framework Core (DbContext)
- Services layer
- AutoMapper for DTOs

### 2. **DTOs/** - Data Transfer Objects
```csharp
- UserDTOs.cs
  - RegisterDTO
  - LoginDTO
  - UserDTO
  - AuthResponseDTO

- PatientDTOs.cs
  - CreatePatientDTO
  - UpdatePatientDTO
  - PatientDTO

- DoctorDTOs.cs
  - CreateDoctorDTO
  - UpdateDoctorDTO
  - DoctorDTO

- AppointmentDTOs.cs
  - CreateAppointmentDTO
  - UpdateAppointmentDTO
  - AppointmentDTO

- PrescriptionDTOs.cs
  - PrescriptionDTO

- SpecializationDTOs.cs
  - SpecializationDTO
```

**Used by:**
- API controllers (parameters, return types)
- Web controllers (model binding, API responses)
- Views (model type declarations)
- AutoMapper for mapping

### 3. **Enums/** - Enumeration Types
```csharp
- Gender.cs         // Male, Female, Other
- AppointmentStatus.cs  // Pending, Confirmed, Cancelled, Completed
```

**Used by:**
- Entity properties
- DTO properties
- Validation logic

---

## 📊 Data Flow Using SmartHealthcare.Models Classes

### Example: Patient Registration

```
1. Web View (Register.cshtml)
   └─ Form submits to Web controller with data

2. Web Controller (AuthController.cs)
   └─ using SmartHealthcare.Models.DTOs;
   └─ Calls API: POST /api/auth/register (RegisterDTO)

3. API Controller (AuthController.cs)
   └─ using SmartHealthcare.Models.DTOs;
   └─ using SmartHealthcare.Models.Entities;
   └─ Receives RegisterDTO ← from SmartHealthcare.Models
   └─ Calls service with RegisterDTO

4. API Service (AuthService.cs)
   └─ Creates User entity ← from SmartHealthcare.Models.Entities
   └─ User properties are set
   └─ Creates Patient entity ← from SmartHealthcare.Models.Entities
   └─ Returns UserDTO ← from SmartHealthcare.Models.DTOs

5. Web Controller receives response
   └─ JSON deserialized to UserDTO ← from SmartHealthcare.Models.DTOs
   └─ Redirects to Dashboard
```

---

## ❓ Why NOT Use Dependency Injection for Models?

Because **SmartHealthcare.Models classes are NOT services**. They are:

- **Entities**: Database model classes (created with `new Entity()`)
- **DTOs**: Simple data containers (created with `new DTO()`)
- **Enums**: Value types (not instantiated)

DI is used for **services and repositories** that have logic and state:
- ✅ `IAuthService` is DI-registered
- ✅ `IDoctorRepository` is DI-registered
- ❌ `Doctor` entity is NOT DI-registered
- ❌ `DoctorDTO` is NOT DI-registered

---

## 🔧 Project Dependencies Summary

```
SmartHealthcare.API
├─ ProjectReference: SmartHealthcare.Models
├─ Uses: Entities (User, Patient, Doctor, etc.)
├─ Uses: DTOs (UserDTO, DoctorDTO, etc.)
├─ Uses: Enums (Gender, AppointmentStatus)
└─ Services handle business logic with these Models

SmartHealthcare.Web
├─ ProjectReference: SmartHealthcare.Models
├─ Uses: DTOs (RegisterDTO, LoginDTO, etc.)
├─ Uses: Enums (Gender, AppointmentStatus)
└─ Controllers use DTOs for model binding and API communication

SmartHealthcare.Models
├─ No dependencies on API or Web
├─ Contains: Entities, DTOs, Enums
└─ Shared across both projects
```

---

## 📝 Summary

| Aspect | SmartHealthcare.Models |
|--------|------------------------|
| **Type** | Shared Class Library |
| **Referenced by** | API & Web projects |
| **DI Registered?** | ❌ NO |
| **How Used** | Via `using` statements |
| **Instantiation** | Direct: `new User()`, `new DoctorDTO()` |
| **Contains** | Entities, DTOs, Enums |
| **Purpose** | Share data structures between layers |

---

## 🎓 What IS Registered in DI?

**SmartHealthcare.API/Program.cs:**
```csharp
// ── Repositories ──────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();

// ── Services ──────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();

// ── JWT Helper ────────────────────────────────────────
builder.Services.AddSingleton<JwtHelper>();

// ── Cache Helper ───────────────────────────────────────
builder.Services.AddScoped<CacheHelper>();

// ── AutoMapper ────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ── Database ──────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>();
```

**SmartHealthcare.Web/Program.cs:**
```csharp
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddHttpClient();
builder.Services.AddSession();
```

✅ **Services and Repositories = DI-registered**
❌ **Entities and DTOs = NOT DI-registered**

