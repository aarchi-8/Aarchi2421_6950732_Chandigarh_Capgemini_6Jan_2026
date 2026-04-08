using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartHealthCareSystem.Shared.Data;
using SmartHealthCareSystem.Shared.Models;
using SmartHealthCareSystem.API.Middleware;
using SmartHealthCareSystem.API.Repositories;
using SmartHealthCareSystem.API.Services;
using SmartHealthCareSystem.API.Validations;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartHealthCareSystem API",
        Version = "v1"
    });
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("SmartHealthCareSystem"));

// Add Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Add Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<IBillService, BillService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedDatabase(dbContext);
}

static void SeedDatabase(ApplicationDbContext dbContext)
{
    if (!dbContext.Departments.Any())
    {
        var departments = new List<Department>
        {
            new Department { DepartmentName = "Cardiology", Description = "Heart and cardiovascular disease care" },
            new Department { DepartmentName = "Neurology", Description = "Brain and nervous system disorders" },
            new Department { DepartmentName = "Orthopedics", Description = "Bone, joint and sports medicine" },
            new Department { DepartmentName = "Pediatrics", Description = "Child and adolescent health care" },
            new Department { DepartmentName = "Dermatology", Description = "Skin, hair and aesthetic treatments" },
            new Department { DepartmentName = "General Medicine", Description = "General and internal medicine" },
            new Department { DepartmentName = "Psychiatry", Description = "Mental health and behavioral care" },
            new Department { DepartmentName = "Ophthalmology", Description = "Eye care and vision" },
            new Department { DepartmentName = "ENT", Description = "Ear, nose and throat disorders" },
            new Department { DepartmentName = "Gynecology", Description = "Women's health and obstetrics" }
        };

        dbContext.Departments.AddRange(departments);
        dbContext.SaveChanges();
    }

        if (!dbContext.Users.Any())
        {
            var admin = new User
            {
                FullName = "System Administrator",
                Email = "admin@hospital.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = DateTime.Now
            };

            // Create 20+ Doctors across different departments
            var doctors = new List<User>
            {
                // Cardiology (4 doctors)
                new User { FullName = "Dr. Jane Carter", Email = "jane.carter@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. Michael Smith", Email = "michael.smith@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. Robert Williams", Email = "robert.williams@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. Patricia Lee", Email = "patricia.lee@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                
                // Neurology (4 doctors)
                new User { FullName = "Dr. Sarah Johnson", Email = "sarah.johnson@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. Aisha Khan", Email = "aisha.khan@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. James Chen", Email = "james.chen@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. Maria Garcia", Email = "maria.garcia@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                
                // Orthopedics (4 doctors)
                new User { FullName = "Dr. Nathan Lee", Email = "nathan.lee@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. Victoria Brown", Email = "victoria.brown@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. William Taylor", Email = "william.taylor@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. Elizabeth Martinez", Email = "elizabeth.martinez@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                
                // Pediatrics (3 doctors)
                new User { FullName = "Dr. Christopher Anderson", Email = "christopher.anderson@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. Jennifer White", Email = "jennifer.white@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. David Kumar", Email = "david.kumar@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                
                // Dermatology (3 doctors)
                new User { FullName = "Dr. Lisa Thompson", Email = "lisa.thompson@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. Daniel Jackson", Email = "daniel.jackson@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. Rachel Green", Email = "rachel.green@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                
                // General Medicine (2 doctors)
                new User { FullName = "Dr. Kevin Harris", Email = "kevin.harris@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now },
                new User { FullName = "Dr. Amanda Clark", Email = "amanda.clark@hospital.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"), Role = "Doctor", CreatedAt = DateTime.Now }
            };

            // Create 25+ Patients
            var patients = new List<User>
            {
                new User { FullName = "John Patterson", Email = "john.patterson@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Emma Wilson", Email = "emma.wilson@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "David Brown", Email = "david.brown@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Lisa Anderson", Email = "lisa.anderson@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Maya Patel", Email = "maya.patel@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Samuel Green", Email = "samuel.green@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Sophie Turner", Email = "sophie.turner@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Michael Torres", Email = "michael.torres@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Angela Rivera", Email = "angela.rivera@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "James Campbell", Email = "james.campbell@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Catherine Hayes", Email = "catherine.hayes@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Charles Wright", Email = "charles.wright@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Diana Fletcher", Email = "diana.fletcher@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Edward Sanchez", Email = "edward.sanchez@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Fiona Morris", Email = "fiona.morris@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "George Rogers", Email = "george.rogers@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Hannah Bell", Email = "hannah.bell@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Isaac Palmer", Email = "isaac.palmer@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Julia Cox", Email = "julia.cox@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Kevin Hunt", Email = "kevin.hunt@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Laura Fox", Email = "laura.fox@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Marcus Stone", Email = "marcus.stone@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Natalie Price", Email = "natalie.price@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Oscar Bennett", Email = "oscar.bennett@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now },
                new User { FullName = "Patricia Adams", Email = "patricia.adams@email.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"), Role = "Patient", CreatedAt = DateTime.Now }
            };

            var allUsers = new List<User> { admin };
            allUsers.AddRange(doctors);
            allUsers.AddRange(patients);

            dbContext.Users.AddRange(allUsers);
            dbContext.SaveChanges();

            // Get Departments
            var cardiology = dbContext.Departments.First(d => d.DepartmentName == "Cardiology");
            var neurology = dbContext.Departments.First(d => d.DepartmentName == "Neurology");
            var orthopedics = dbContext.Departments.First(d => d.DepartmentName == "Orthopedics");
            var pediatrics = dbContext.Departments.First(d => d.DepartmentName == "Pediatrics");
            var dermatology = dbContext.Departments.First(d => d.DepartmentName == "Dermatology");
            var generalMedicine = dbContext.Departments.First(d => d.DepartmentName == "General Medicine");
            var psychiatry = dbContext.Departments.First(d => d.DepartmentName == "Psychiatry");
            var ophthalmology = dbContext.Departments.First(d => d.DepartmentName == "Ophthalmology");
            var ent = dbContext.Departments.First(d => d.DepartmentName == "ENT");
            var gynecology = dbContext.Departments.First(d => d.DepartmentName == "Gynecology");

            // Create comprehensive Doctor profiles with experience and availability
            var doctorProfiles = new List<Doctor>
            {
                // Cardiology Doctors
                new Doctor { UserId = doctors[0].UserId, DepartmentId = cardiology.DepartmentId, Specialization = "Interventional Cardiology", ExperienceYears = 15, Availability = "Mon-Fri 9am - 5pm" },
                new Doctor { UserId = doctors[1].UserId, DepartmentId = cardiology.DepartmentId, Specialization = "Echocardiography", ExperienceYears = 12, Availability = "Tue-Thu 10am - 4pm" },
                new Doctor { UserId = doctors[2].UserId, DepartmentId = cardiology.DepartmentId, Specialization = "Cardiac Surgery", ExperienceYears = 20, Availability = "Mon-Wed-Fri 8am - 6pm" },
                new Doctor { UserId = doctors[3].UserId, DepartmentId = cardiology.DepartmentId, Specialization = "Preventive Cardiology", ExperienceYears = 10, Availability = "Tue-Fri 11am - 5pm" },
                
                // Neurology Doctors
                new Doctor { UserId = doctors[4].UserId, DepartmentId = neurology.DepartmentId, Specialization = "Neuropsychiatry", ExperienceYears = 14, Availability = "Mon-Thu 9am - 5pm" },
                new Doctor { UserId = doctors[5].UserId, DepartmentId = neurology.DepartmentId, Specialization = "Epilepsy Specialist", ExperienceYears = 11, Availability = "Tue-Thu-Sat 10am - 4pm" },
                new Doctor { UserId = doctors[6].UserId, DepartmentId = neurology.DepartmentId, Specialization = "Stroke & Cerebrovascular", ExperienceYears = 13, Availability = "Mon-Wed-Fri 8am - 4pm" },
                new Doctor { UserId = doctors[7].UserId, DepartmentId = neurology.DepartmentId, Specialization = "Neuromuscular Disorders", ExperienceYears = 9, Availability = "Tue-Fri 1pm - 6pm" },
                
                // Orthopedics Doctors
                new Doctor { UserId = doctors[8].UserId, DepartmentId = orthopedics.DepartmentId, Specialization = "Sports Medicine", ExperienceYears = 13, Availability = "Mon-Fri 9am - 5pm" },
                new Doctor { UserId = doctors[9].UserId, DepartmentId = orthopedics.DepartmentId, Specialization = "Joint Replacement", ExperienceYears = 18, Availability = "Mon-Wed-Fri 8am - 4pm" },
                new Doctor { UserId = doctors[10].UserId, DepartmentId = orthopedics.DepartmentId, Specialization = "Spine Surgery", ExperienceYears = 16, Availability = "Tue-Thu 10am - 3pm" },
                new Doctor { UserId = doctors[11].UserId, DepartmentId = orthopedics.DepartmentId, Specialization = "Trauma & Fracture Care", ExperienceYears = 10, Availability = "Mon-Fri 9am - 5pm" },
                
                // Pediatrics Doctors
                new Doctor { UserId = doctors[12].UserId, DepartmentId = pediatrics.DepartmentId, Specialization = "Pediatric Cardiology", ExperienceYears = 12, Availability = "Mon-Thu 9am - 4pm" },
                new Doctor { UserId = doctors[13].UserId, DepartmentId = pediatrics.DepartmentId, Specialization = "Neonatology", ExperienceYears = 11, Availability = "Tue-Fri 10am - 5pm" },
                new Doctor { UserId = doctors[14].UserId, DepartmentId = pediatrics.DepartmentId, Specialization = "General Pediatrics", ExperienceYears = 8, Availability = "Mon-Fri 9am - 5pm" },
                
                // Dermatology Doctors  
                new Doctor { UserId = doctors[15].UserId, DepartmentId = dermatology.DepartmentId, Specialization = "Cosmetic Dermatology", ExperienceYears = 10, Availability = "Tue-Sat 10am - 6pm" },
                new Doctor { UserId = doctors[16].UserId, DepartmentId = dermatology.DepartmentId, Specialization = "Dermatopathology", ExperienceYears = 12, Availability = "Mon-Wed-Fri 9am - 4pm" },
                new Doctor { UserId = doctors[17].UserId, DepartmentId = dermatology.DepartmentId, Specialization = "Clinical Dermatology", ExperienceYears = 9, Availability = "Mon-Fri 10am - 5pm" },
                
                // General Medicine Doctors
                new Doctor { UserId = doctors[18].UserId, DepartmentId = generalMedicine.DepartmentId, Specialization = "Internal Medicine", ExperienceYears = 15, Availability = "Mon-Fri 8am - 5pm" },
                new Doctor { UserId = doctors[19].UserId, DepartmentId = generalMedicine.DepartmentId, Specialization = "Family Medicine", ExperienceYears = 12, Availability = "Mon-Fri 9am - 5pm" }
            };

            dbContext.Doctors.AddRange(doctorProfiles);
            dbContext.SaveChanges();

            // Create multiple appointments and related data
            var appointments = new List<Appointment>();
            var prescriptions = new List<Prescription>();
            var bills = new List<Bill>();
            
            // Generate 40+ appointments with related prescriptions and bills
            for (int i = 0; i < patients.Count; i++)
            {
                var appointmentDates = new[] { 2, 5, 10, 15, 20, 25 };
                foreach (var dayOffset in appointmentDates)
                {
                    if (appointments.Count >= 40) break;
                    
                    var doctor = doctorProfiles[i % doctorProfiles.Count];
                    var appointment = new Appointment
                    {
                        PatientId = patients[i].UserId,
                        DoctorId = doctor.DoctorId,
                        AppointmentDate = DateTime.Now.AddDays(dayOffset).Date.AddHours(9 + (i % 8)),
                        Status = dayOffset <= 5 ? "Booked" : (dayOffset <= 15 ? "Scheduled" : "Completed")
                    };
                    appointments.Add(appointment);
                }
            }

            dbContext.Appointments.AddRange(appointments);
            dbContext.SaveChanges();

            var diagnosisList = new[] {
                "Hypertension",
                "Type 2 Diabetes",
                "High Cholesterol",
                "Migraine Headaches",
                "Fractured Arm",
                "Lower Back Pain",
                "Atrial Fibrillation",
                "Asthma",
                "Seasonal Allergies",
                "Skin Rash",
                "Anxiety Disorder",
                "Bronchitis",
                "Knee Osteoarthritis",
                "Urinary Tract Infection",
                "Common Cold"
            };

            // Create prescriptions for each appointment
            foreach (var appointment in appointments)
            {
                var diagnosis = diagnosisList[appointments.IndexOf(appointment) % diagnosisList.Length];
                var prescription = new Prescription
                {
                    AppointmentId = appointment.AppointmentId,
                    Diagnosis = diagnosis,
                    Medicines = "Medication as prescribed",
                    Notes = "Follow up recommended"
                };
                prescriptions.Add(prescription);

                var bill = new Bill
                {
                    AppointmentId = appointment.AppointmentId,
                    ConsultationFee = 150 + (appointments.IndexOf(appointment) % 10) * 10,
                    MedicineCharges = 50 + (appointments.IndexOf(appointment) % 5) * 20,
                    TotalAmount = 0,
                    PaymentStatus = appointments.IndexOf(appointment) % 2 == 0 ? "Paid" : "Unpaid"
                };
                bill.TotalAmount = bill.ConsultationFee + bill.MedicineCharges;
                bills.Add(bill);
            }

            dbContext.Prescriptions.AddRange(prescriptions);
            dbContext.Bills.AddRange(bills);
            dbContext.SaveChanges();
        }
    }

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartHealthCareSystem API V1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

// Use Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
