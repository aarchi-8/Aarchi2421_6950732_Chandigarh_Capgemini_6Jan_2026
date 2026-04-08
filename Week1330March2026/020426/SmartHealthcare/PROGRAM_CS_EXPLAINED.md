# Program.cs - Line by Line Explanation

## 🔌 SmartHealthcare.API/Program.cs

### **1. Using Statements**
```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;    // JWT authentication
using Microsoft.EntityFrameworkCore;                     // Database ORM
using Microsoft.IdentityModel.Tokens;                    // JWT token validation
using Microsoft.OpenApi.Models;                          // Swagger documentation
using Serilog;                                           // Logging framework
using SmartHealthcare.API.Data;                          // DbContext
using SmartHealthcare.API.Helpers;                       // JWT, Cache helpers
using SmartHealthcare.API.Mappings;                      // AutoMapper profiles
using SmartHealthcare.API.Middleware;                    // Custom middleware
using SmartHealthcare.API.Repositories;                  // Data access
using SmartHealthcare.API.Repositories.Interfaces;       // Repository contracts
using SmartHealthcare.API.Services;                      // Business logic
using SmartHealthcare.API.Services.Interfaces;           // Service contracts
using System.Text;                                       // Text encoding
```

---

### **2. Serilog Configuration**
```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();
```
**Purpose:** Initialize structured logging from `appsettings.json`
- Reads logging settings (file sink, levels, format)
- Creates logger instance for use throughout app
- Logs to `Logs/` folder

---

### **3. Create WebApplication Builder**
```csharp
var builder = WebApplication.CreateBuilder(args);
```
**Purpose:** Start ASP.NET Core configuration
- Creates builder object to register services
- Sets up dependency injection container
- Reads configuration from appsettings.json

---

### **4. Register Serilog**
```csharp
builder.Host.UseSerilog();
```
**Purpose:** Use Serilog as the logging provider
- Replaces default logging with Serilog
- All logging now goes through Serilog pipeline
- Shows structured logs in console and file

---

### **5. Register Database Context**
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```
**Purpose:** Configure Entity Framework Core + SQL Server
- **AddDbContext**: Registers AppDbContext in DI container (Scoped lifespan)
- **UseSqlServer**: Uses SQL Server as database
- **GetConnectionString**: Reads connection from appsettings.json
- Connection: `Server=KHUSHI23\SQLEXPRESS;Database=SmartHealthcareDB;Trusted_Connection=true;`

---

### **6. Register Controllers**
```csharp
builder.Services.AddControllers();
```
**Purpose:** Enable API controllers
- Registers controller routing
- Enables model binding, validation
- Sets up JSON serialization

---

### **7. Register AutoMapper**
```csharp
builder.Services.AddAutoMapper(typeof(MappingProfile));
```
**Purpose:** Enable automatic object-to-object mapping
- Finds all AutoMapper profiles in assembly
- Maps between Entities and DTOs (User ↔ UserDTO)
- Reduces boilerplate mapping code

---

### **8. Register Repositories (Data Access)**
```csharp
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
```
**Purpose:** Register data access layer in DI container
- **Scoped**: New instance per HTTP request
- Interface → Implementation mapping
- Enables dependency injection in services
- Example: `public DoctorService(IDoctorRepository repo) { ... }`

---

### **9. Register Services (Business Logic)**
```csharp
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
```
**Purpose:** Register business logic layer in DI container
- **Scoped**: New instance per HTTP request
- AuthService: Registration & login logic
- DoctorService: Doctor CRUD & caching
- PatientService: Patient CRUD
- AppointmentService: Appointment management

---

### **10. Register JWT Helper**
```csharp
builder.Services.AddSingleton<JwtHelper>();
```
**Purpose:** Register JWT token generation utility
- **Singleton**: One instance for entire app lifetime
- Used by AuthService to create JWT tokens
- Generates access tokens (60-min expiration)

---

### **11. Register Cache Helper**
```csharp
builder.Services.AddScoped<CacheHelper>();
```
**Purpose:** Register caching utility
- **Scoped**: New instance per HTTP request
- Used by DoctorService for caching doctor lists
- Implements sliding expiration (1 hour)

---

### **12. Configure JWT Authentication**
```csharp
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;
```
**Purpose:** Read JWT settings from appsettings.json
- Gets JWT configuration section
- Extracts secret key for token signing
- Settings: SecretKey, Issuer, Audience, ExpirationMinutes

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,              // Verify token issuer matches config
        ValidateAudience = true,            // Verify target audience
        ValidateLifetime = true,            // Check token expiration
        ValidateIssuerSigningKey = true,    // Verify signature with secret key
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
```
**Purpose:** Enable JWT bearer token authentication
- Sets JWT as default authentication scheme
- Configures token validation rules
- Enables `[Authorize]` attribute on controllers
- Validates incoming tokens from Authorization header

---

### **13. Register In-Memory Cache**
```csharp
builder.Services.AddMemoryCache();
```
**Purpose:** Enable in-memory caching
- Used for caching doctor lists
- Faster than database queries
- Auto-invalidated on updates

---

### **14. Configure Swagger (API Documentation)**
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartHealthcare API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { ... });
});
```
**Purpose:** Enable Swagger/OpenAPI documentation
- Generates interactive API docs at `http://localhost:5125/swagger`
- Shows all endpoints, parameters, responses
- Includes JWT bearer authentication option
- Test endpoints directly in browser

---

### **15. Build Application**
```csharp
var app = builder.Build();
```
**Purpose:** Finalize configuration and build middleware pipeline
- Creates IWebApplication instance
- Locks in all service registrations
- Ready to configure middleware

---

### **16. Seed Database with Sample Doctors**
```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    try
    {
        dbContext.Database.EnsureCreated();
        
        if (!dbContext.Doctors.Any())
        {
            // Create 4 sample doctors...
        }
    }
}
```
**Purpose:** Populate database with initial data on startup
- Creates database if it doesn't exist (EnsureCreated)
- Checks if doctors exist (avoid duplicates)
- Seeds 4 doctors: Sarah Johnson, James Wilson, Emily Brown, Michael Davis
- Sets passwords, specializations, consultation fees
- Only runs once (checks if data exists)

---

### **17. Register Custom Middleware**
```csharp
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
```
**Purpose:** Add custom HTTP pipeline handlers (in order)
- **ExceptionMiddleware**: Catches all exceptions, returns consistent error responses
- **RequestLoggingMiddleware**: Logs all incoming requests and response status
- Middleware order matters! (Pipeline chain)

---

### **18. Enable Swagger in Development**
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```
**Purpose:** Enable API documentation UI in development only
- **Development**: Show Swagger UI at `/swagger`
- **Production**: Disabled (security & performance)
- Accessible at `http://localhost:5125/swagger`

---

### **19. Redirect HTTP to HTTPS**
```csharp
app.UseHttpsRedirection();
```
**Purpose:** Force HTTPS connections
- Redirects `http://` to `https://`
- Improves security
- Enforced in production

---

### **20. Enable Authentication & Authorization**
```csharp
app.UseAuthentication();
app.UseAuthorization();
```
**Purpose:** Enable JWT token verification
- **UseAuthentication**: Validates JWT tokens from Authorization header
- **UseAuthorization**: Checks `[Authorize]` and role restrictions
- Order matters: Authentication before Authorization

---

### **21. Map Controller Routes**
```csharp
app.MapControllers();
```
**Purpose:** Enable API endpoint routing
- Maps all controller actions to routes
- Example: `[Route("api/[controller]")]` → `/api/doctors`
- Enables automatic route mapping

---

### **22. Start Application with Error Handling**
```csharp
try
{
    Log.Information("SmartHealthcare API starting...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}
```
**Purpose:** Run app with logging and cleanup
- Starts HTTP server on port 5125
- Logs startup message
- Catches fatal errors during startup
- Flushes remaining logs before shutdown

---

---

## 🖼️ SmartHealthcare.Web/Program.cs (MVC Web)

### **1. Using Statements**
```csharp
using Microsoft.AspNetCore.Authentication.Cookies;  // Cookie-based auth
using SmartHealthcare.Web.Services;                // Custom services (ApiClient)
```

---

### **2. Create Builder**
```csharp
var builder = WebApplication.CreateBuilder(args);
```
**Purpose:** Initialize ASP.NET Core MVC configuration

---

### **3. Register Controllers with Views**
```csharp
builder.Services.AddControllersWithViews();
```
**Purpose:** Enable MVC controllers and Razor views
- Registers controllers
- Enables Razor view rendering (.cshtml files)
- Model binding from forms

---

### **4. Register HttpContextAccessor**
```csharp
builder.Services.AddHttpContextAccessor();
```
**Purpose:** Access current HTTP context properties
- Used to access HttpContext.Session
- Enables session token retrieval
- Available to services via dependency injection

---

### **5. Configure Session**
```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);        // 1-hour inactivity timeout
    options.Cookie.Name = "SmartHealthcare.Session";    // Cookie name
    options.Cookie.HttpOnly = true;                     // Cannot be accessed by JavaScript
    options.Cookie.IsEssential = true;                  // Always created (GDPR)
});
```
**Purpose:** Enable server-side session storage
- Stores JWT token in `Session["AuthToken"]`
- Timeout: 1 hour of inactivity
- HttpOnly cookie prevents XSS attacks
- Session data stored on server

---

### **6. Configure Cookie Authentication**
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Auth/Login";              // Redirect if not authenticated
        options.LogoutPath = "/Auth/Logout";            // Logout endpoint
        options.AccessDeniedPath = "/Home/AccessDenied"; // Redirect if unauthorized
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // 1-hour cookie expiration
    });
```
**Purpose:** Enable user authentication via cookies
- Creates authentication cookie
- Stores user identity in cookie
- Redirects to login if not authenticated
- Auto-logout after 1-hour inactivity

---

### **7. Register API Client**
```csharp
builder.Services.AddHttpClient<IApiClient, ApiClient>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://localhost:5125/api/"));
```
**Purpose:** Register HTTP client for API calls
- **AddHttpClient**: Creates HttpClient factory
- **IApiClient**: Interface for API communication
- **ApiClient**: Implementation with bearer token support
- **BaseAddress**: API endpoint URL (http://localhost:5125/api/)
- Automatically injected into controllers
- Example: `public DoctorsController(IApiClient apiClient) { ... }`

---

### **8. Build Application**
```csharp
var app = builder.Build();
```
**Purpose:** Finalize configuration

---

### **9. Configure Error Handling**
```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // HTTP Strict Transport Security (1 year)
}
```
**Purpose:** Error handling for production
- **Development**: Shows detailed error pages
- **Production**: Shows generic error page
- **HSTS**: Forces HTTPS for 1 year

---

### **10. Redirect HTTP to HTTPS**
```csharp
app.UseHttpsRedirection();
```
**Purpose:** Force HTTPS connections

---

### **11. Enable Routing**
```csharp
app.UseRouting();
```
**Purpose:** Enable URL routing for MVC
- Maps URLs to controllers and actions

---

### **12. Enable Session Middleware**
```csharp
app.UseSession();
```
**Purpose:** Activate session storage
- Must come after routing, before authentication
- Enables `HttpContext.Session.GetString("AuthToken")`

---

### **13. Enable Authentication**
```csharp
app.UseAuthentication();
app.UseAuthorization();
```
**Purpose:** Validate user identity and permissions
- Reads authentication cookie
- Populates `User.Identity`
- Enables `[Authorize]` attribute

---

### **14. Map Static Files**
```csharp
app.MapStaticAssets();
```
**Purpose:** Serve static files (CSS, JS, images)
- From `wwwroot/` folder
- CSS, JavaScript, images
- Cached by browser

---

### **15. Map Default Route**
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
```
**Purpose:** Define default MVC routing
- **controller=Home**: Default controller
- **action=Index**: Default action/method
- **id?**: Optional ID parameter
- Example routes:
  - `/` → HomeController.Index()
  - `/Doctors` → DoctorsController.Index()
  - `/Doctors/Details/1` → DoctorsController.Details(1)

---

### **16. Run Application**
```csharp
app.Run();
```
**Purpose:** Start web server on port 5272
- Listens for HTTP requests
- Runs until manually stopped (Ctrl+C)

---

## 📋 Quick Comparison

| Aspect | API | Web |
|--------|-----|-----|
| **Port** | 5125 | 5272 |
| **Type** | REST API | MVC Web App |
| **Auth** | JWT Bearer Token | Cookie + Session |
| **Database** | EF Core + SQL Server | N/A (calls API) |
| **Logging** | Serilog | Default |
| **Middleware** | Custom (Exception, Logging) | Built-in |
| **Swagger** | Yes | No |
| **Session** | N/A | Yes |
| **ApiClient** | N/A | Yes |

---

## 🔄 Key Execution Order (Startup)

### **API Startup:**
1. Serilog configured
2. Services registered (DI container)
3. Middleware pipeline built
4. Database seeded (4 doctors)
5. HTTP server started on :5125

### **Web Startup:**
1. Services registered (DI container)
2. Authentication configured
3. Session enabled
4. ApiClient registered
5. Default route configured
6. HTTP server started on :5272

