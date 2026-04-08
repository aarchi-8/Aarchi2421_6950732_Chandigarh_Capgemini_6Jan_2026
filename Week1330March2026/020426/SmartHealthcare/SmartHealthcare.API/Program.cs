using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Helpers;
using SmartHealthcare.API.Mappings;
using SmartHealthcare.API.Middleware;
using SmartHealthcare.API.Repositories;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services;
using SmartHealthcare.API.Services.Interfaces;
using System.Text;
using SmartHealthcare.Models.Entities;
using SmartHealthcare.Models.Enums;

// ── Serilog Setup ─────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// ── Use Serilog ───────────────────────────────────────
builder.Host.UseSerilog();

// ── Database ──────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Controllers ───────────────────────────────────────
builder.Services.AddControllers();

// ── AutoMapper ────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ── Repositories ──────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// ── Services ──────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// ── JWT Helper ────────────────────────────────────────
builder.Services.AddSingleton<JwtHelper>();

// ── Cache Helper ───────────────────────────────────────
builder.Services.AddScoped<CacheHelper>();

// ── JWT Authentication ────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// ── Caching ───────────────────────────────────────────
builder.Services.AddMemoryCache();

// ── Swagger ───────────────────────────────────────────
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
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── Seed Database ─────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    try
    {
        dbContext.Database.EnsureCreated();
        
        Log.Information("Seeding / verifying sample doctors...");

        var doctors = new[]
        {
            new { Email = "sarah.johnson@hospital.com", Name = "Sarah Johnson", SpecId = 1, Fee = 500m, Years = 8 },
            new { Email = "james.wilson@hospital.com", Name = "James Wilson", SpecId = 2, Fee = 400m, Years = 12 },
            new { Email = "emily.brown@hospital.com", Name = "Emily Brown", SpecId = 5, Fee = 350m, Years = 6 },
            new { Email = "michael.davis@hospital.com", Name = "Michael Davis", SpecId = 3, Fee = 450m, Years = 10 }
        };

        foreach (var doctorData in doctors)
        {
                var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == doctorData.Email);

                if (existingUser == null)
                {
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword("DoctorPass123!");

                    var user = new User
                    {
                        FullName = doctorData.Name,
                        Email = doctorData.Email,
                        PasswordHash = hashedPassword,
                        Role = "Doctor",
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();

                    existingUser = user;
                }
                else
                {
                    if (!BCrypt.Net.BCrypt.Verify("DoctorPass123!", existingUser.PasswordHash))
                    {
                        existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("DoctorPass123!");
                        dbContext.SaveChanges();
                    }
                }

                // Create doctor profile if not exists
                if (!dbContext.Doctors.Any(d => d.UserId == existingUser.UserId))
                {
                    var doctor = new Doctor
                    {
                        UserId = existingUser.UserId,
                        Phone = $"+1-555-{1000 + dbContext.Doctors.Count():0000}",
                        Qualification = "MBBS, MD",
                        ExperienceYears = doctorData.Years,
                        ConsultationFee = doctorData.Fee,
                        IsAvailable = true
                    };

                    dbContext.Doctors.Add(doctor);
                    dbContext.SaveChanges();

                    // Link specialization
                    var doctorSpec = new DoctorSpecialization
                    {
                        DoctorId = doctor.DoctorId,
                        SpecializationId = doctorData.SpecId
                    };
                    dbContext.DoctorSpecializations.Add(doctorSpec);
                    dbContext.SaveChanges();

                    Log.Information($"Created doctor: {doctorData.Name} ({doctorData.Email})");
                }
                else
                {
                    Log.Information($"Doctor profile already exists for user {existingUser.UserId} ({doctorData.Email})");
                }
            }

        // Seed patients
        Log.Information("Seeding / verifying sample patients...");

        var patients = new[]
        {
            new { Email = "john.doe@email.com", Name = "John Doe", Phone = "+1-555-1001", DOB = new DateTime(1985, 5, 15), Gender = Gender.Male, Address = "123 Main St, City, State", History = "Hypertension" },
            new { Email = "jane.smith@email.com", Name = "Jane Smith", Phone = "+1-555-1002", DOB = new DateTime(1990, 8, 22), Gender = Gender.Female, Address = "456 Oak Ave, City, State", History = "Diabetes" },
            new { Email = "mike.johnson@email.com", Name = "Mike Johnson", Phone = "+1-555-1003", DOB = new DateTime(1978, 12, 10), Gender = Gender.Male, Address = "789 Pine Rd, City, State", History = "Back pain" },
            new { Email = "sarah.wilson@email.com", Name = "Sarah Wilson", Phone = "+1-555-1004", DOB = new DateTime(1995, 3, 8), Gender = Gender.Female, Address = "321 Elm St, City, State", History = "Allergies" },
            new { Email = "david.brown@email.com", Name = "David Brown", Phone = "+1-555-1005", DOB = new DateTime(1982, 7, 30), Gender = Gender.Male, Address = "654 Maple Dr, City, State", History = "Heart condition" },
            new { Email = "lisa.davis@email.com", Name = "Lisa Davis", Phone = "+1-555-1006", DOB = new DateTime(1988, 11, 5), Gender = Gender.Female, Address = "987 Cedar Ln, City, State", History = "Migraine" }
        };

        var patientUsers = new List<User>();
        foreach (var patientData in patients)
        {
            var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == patientData.Email);

            if (existingUser == null)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("PatientPass123!");

                var user = new User
                {
                    FullName = patientData.Name,
                    Email = patientData.Email,
                    PasswordHash = hashedPassword,
                    Role = "Patient",
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                existingUser = user;
            }
            else
            {
                if (!BCrypt.Net.BCrypt.Verify("PatientPass123!", existingUser.PasswordHash))
                {
                    existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("PatientPass123!");
                    dbContext.SaveChanges();
                }
            }

            patientUsers.Add(existingUser);

            // Create patient profile if not exists
            if (!dbContext.Patients.Any(p => p.UserId == existingUser.UserId))
            {
                var patient = new Patient
                {
                    UserId = existingUser.UserId,
                    Phone = patientData.Phone,
                    DateOfBirth = patientData.DOB,
                    Gender = patientData.Gender,
                    Address = patientData.Address,
                    MedicalHistory = patientData.History
                };

                dbContext.Patients.Add(patient);
                dbContext.SaveChanges();

                Log.Information($"Created patient: {patientData.Name} ({patientData.Email})");
            }
        }

        // Seed appointments
        Log.Information("Seeding sample appointments...");

        var doctorsList = dbContext.Doctors.Include(d => d.User).ToList();
        var patientsList = dbContext.Patients.Include(p => p.User).ToList();

        if (doctorsList.Any() && patientsList.Any())
        {
            var appointments = new[]
            {
                new { PatientIndex = 0, DoctorIndex = 0, Date = DateTime.UtcNow.AddDays(1).Date.AddHours(10), Reason = "Regular checkup", Status = AppointmentStatus.Confirmed },
                new { PatientIndex = 1, DoctorIndex = 1, Date = DateTime.UtcNow.AddDays(2).Date.AddHours(14), Reason = "Follow-up consultation", Status = AppointmentStatus.Confirmed },
                new { PatientIndex = 2, DoctorIndex = 2, Date = DateTime.UtcNow.AddDays(3).Date.AddHours(9), Reason = "Blood pressure check", Status = AppointmentStatus.Confirmed },
                new { PatientIndex = 3, DoctorIndex = 3, Date = DateTime.UtcNow.AddDays(4).Date.AddHours(11), Reason = "Allergy treatment", Status = AppointmentStatus.Confirmed },
                new { PatientIndex = 4, DoctorIndex = 0, Date = DateTime.UtcNow.AddDays(5).Date.AddHours(15), Reason = "Cardiac evaluation", Status = AppointmentStatus.Confirmed },
                new { PatientIndex = 5, DoctorIndex = 1, Date = DateTime.UtcNow.AddDays(6).Date.AddHours(13), Reason = "Headache consultation", Status = AppointmentStatus.Confirmed },
                new { PatientIndex = 0, DoctorIndex = 2, Date = DateTime.UtcNow.AddDays(7).Date.AddHours(10), Reason = "Annual physical", Status = AppointmentStatus.Confirmed }
            };

            foreach (var apptData in appointments)
            {
                var patient = patientsList.ElementAtOrDefault(apptData.PatientIndex);
                var doctor = doctorsList.ElementAtOrDefault(apptData.DoctorIndex);

                if (patient != null && doctor != null)
                {
                    // Check if appointment already exists
                    if (!dbContext.Appointments.Any(a => a.PatientId == patient.PatientId && a.DoctorId == doctor.DoctorId && a.AppointmentDate == apptData.Date))
                    {
                        var appointment = new Appointment
                        {
                            PatientId = patient.PatientId,
                            DoctorId = doctor.DoctorId,
                            AppointmentDate = apptData.Date,
                            Reason = apptData.Reason,
                            Status = apptData.Status,
                            CreatedAt = DateTime.UtcNow
                        };

                        dbContext.Appointments.Add(appointment);
                        dbContext.SaveChanges();

                        Log.Information($"Created appointment: {patient.User?.FullName} with {doctor.User?.FullName} on {apptData.Date}");
                    }
                }
            }
        }
        else
        {
            Log.Warning("No doctors or patients found for seeding appointments");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error seeding database");
    }
}

// ── Middlewares ───────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

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