# Smart Healthcare System - Complete Project Structure Guide

## 📁 Project Overview

The Smart Healthcare Management System is a full-stack .NET application with three main projects:

1. **SmartHealthcare.API** - REST API backend (Port: 5125)
2. **SmartHealthcare.Web** - MVC web frontend (Port: 5272)
3. **SmartHealthcare.Models** - Shared data models and DTOs

---

## 🏗️ Project 1: SmartHealthcare.Models (Data Layer)

**Purpose**: Contains all shared entities, DTOs, and enums used across API and Web.

### 📂 Folders & Files:

#### **DTOs/** - Data Transfer Objects
Used to transfer data between API and Web layers while exposing only necessary fields.

- **UserDTOs.cs**
  - `RegisterDTO` - User registration (FullName, Email, Password, Role)
  - `LoginDTO` - User login (Email, Password)
  - `UserDTO` - User response object
  - `AuthResponseDTO` - Login response with JWT token

- **PatientDTOs.cs**
  - `CreatePatientDTO` - Create patient profile
  - `UpdatePatientDTO` - Update patient info
  - `PatientDTO` - Full patient details response

- **DoctorDTOs.cs**
  - `CreateDoctorDTO` - Create doctor profile with specializations
  - `UpdateDoctorDTO` - Update doctor info
  - `DoctorDTO` - Full doctor details response

- **AppointmentDTOs.cs**
  - `CreateAppointmentDTO` - Book appointment (DoctorId, Date, Reason)
  - `UpdateAppointmentDTO` - Update appointment status
  - `AppointmentDTO` - Full appointment details

- **SpecializationDTOs.cs**
  - `SpecializationDTO` - Medical specialization (Cardiology, General, etc.)

- **PrescriptionDTOs.cs**
  - `PrescriptionDTO` - Prescription details with medicine list

#### **Entities/** - Database Models
Real database entities with relationships.

- **User.cs** - Core user account (1-to-1 with Patient or Doctor)
  - UserId (PK), FullName, Email, PasswordHash, Role, CreatedAt
  
- **Patient.cs** - Patient profile (extends User)
  - PatientId (PK), UserId (FK), Phone, DateOfBirth, Gender, Address, MedicalHistory
  - Navigation: User, Appointments, Prescriptions

- **Doctor.cs** - Doctor profile (extends User)
  - DoctorId (PK), UserId (FK), Phone, Qualification, ExperienceYears, ConsultationFee, IsAvailable
  - Navigation: User, Appointments, DoctorSpecializations

- **Appointment.cs** - Appointment booking
  - AppointmentId (PK), PatientId (FK), DoctorId (FK), AppointmentDate, Reason, Status, Notes
  - Status: Pending, Confirmed, Cancelled, Completed

- **Prescription.cs** - Medical prescription
  - PrescriptionId (PK), PatientId (FK), DoctorId (FK), IssuedDate, Instructions, SideEffects, Precautions, IsActive
  - Navigation: PrescriptionMedicines

- **Medicine.cs** - Medicine/drug information
  - MedicineId (PK), Name, Description, Dosage

- **PrescriptionMedicine.cs** - Many-to-Many: Prescription ↔ Medicine
  - Links medicines to prescriptions

- **Specialization.cs** - Doctor specializations
  - SpecializationId (PK), Name (Cardiology, General, etc.)

- **DoctorSpecialization.cs** - Many-to-Many: Doctor ↔ Specialization
  - Links doctors to their specializations

#### **Enums/** - Enumeration Types

- **Gender.cs** - Male, Female, Other
- **AppointmentStatus.cs** - Pending, Confirmed, Cancelled, Completed

---

## 🔌 Project 2: SmartHealthcare.API (Backend REST API)

**Purpose**: RESTful API providing all business logic and database operations.

### 📂 Folders & Files:

#### **Controllers/** - API Endpoints

**📌 AuthController.cs** - Authentication endpoints
```
POST /api/auth/register - User registration (Patient/Doctor)
POST /api/auth/login - User login with JWT token response
```
- Calls AuthService for business logic
- Returns JWT token valid for 60 minutes
- Auto-creates Patient profile if registering as Patient
- Auto-creates Doctor profile if registering as Doctor

**📌 DoctorsController.cs** - Doctor management endpoints
```
GET /api/doctors - Get all doctors with specializations
GET /api/doctors/{id} - Get single doctor by ID
GET /api/doctors/search - Advanced search with filters
GET /api/doctors/specialization/{specId} - Get doctors by specialization
POST /api/doctors - Create doctor profile [Authorize: Admin, Doctor]
PUT /api/doctors/{id} - Update doctor info [Authorize: Admin, Doctor]
DELETE /api/doctors/{id} - Delete doctor [Authorize: Admin]
```
- Implements caching for doctor lists (1-hour sliding expiration)
- Automatically caches newly registered doctors
- Returns list of doctors with specializations for patient booking

**📌 PatientsController.cs** - Patient management endpoints
```
GET /api/patients - Get all patients [Authorize: Admin, Doctor]
GET /api/patients/{id} - Get patient details [Authorize: Admin, Doctor, Patient]
GET /api/patients/my-profile - Get current patient profile [Authorize: Patient]
POST /api/patients - Create patient profile [Authorize: Patient]
PUT /api/patients/{id} - Update patient info [Authorize: Admin, Patient]
PATCH /api/patients/{id}/medical-history - Update medical history [Authorize: Admin, Doctor]
DELETE /api/patients/{id} - Delete patient [Authorize: Admin]
```
- Doctors can see their patients list
- Patients can view their own profiles
- Auto-created when patient registers

**📌 AppointmentsController.cs** - Appointment management endpoints
```
GET /api/appointments - Get all appointments [Authorize: Admin]
GET /api/appointments/{id} - Get appointment details [Authorize: All]
GET /api/appointments/my - Get current user's appointments [Authorize: Patient, Doctor]
POST /api/appointments - Create (book) appointment [Authorize: Patient]
PUT /api/appointments/{id} - Update appointment status [Authorize: Admin, Doctor]
DELETE /api/appointments/{id} - Cancel appointment [Authorize: Admin]
```
- Patients book appointments with available doctors
- Doctors can view appointment list
- Tracks appointment status: Pending, Confirmed, Cancelled, Completed

#### **Services/** - Business Logic Layer
Implements core business operations and calls repositories.

- **IAuthService & AuthService**
  - `RegisterAsync()` - Handles user registration
    - Creates User account
    - Auto-creates Patient or Doctor profile
    - Hashes password with BCrypt
  - `LoginAsync()` - Handles user authentication
    - Validates credentials
    - Generates JWT token with userId claim
    - Returns token valid for 60 minutes

- **IDoctorService & DoctorService**
  - `GetAllDoctorsAsync()` - Returns all doctors (cached)
  - `GetDoctorByIdAsync()` - Get single doctor with specializations
  - `GetDoctorsBySpecializationAsync()` - Filter by specialty
  - `CreateDoctorAsync()` - Create doctor profile with specializations
  - `UpdateDoctorAsync()` - Update doctor details
  - Implements caching with automatic invalidation

- **IPatientService & PatientService**
  - `GetAllPatientsAsync()` - Get all patients
  - `GetPatientByIdAsync()` - Get patient details
  - `GetPatientByUserIdAsync()` - Map userId to PatientId
  - `CreatePatientAsync()` - Create patient profile
  - `UpdatePatientAsync()` - Update patient info

- **IAppointmentService & AppointmentService**
  - `GetAllAppointmentsAsync()` - Get all appointments
  - `GetAppointmentByIdAsync()` - Get appointment details
  - `GetPatientAppointmentsAsync()` - Get patient's appointments
  - `CreateAppointmentAsync()` - Create appointment
  - `UpdateAppointmentAsync()` - Change appointment status

#### **Repositories/** - Data Access Layer
Pattern: Generic Repository with specialized repositories for each entity.

- **IGenericRepository & GenericRepository**
  - Generic CRUD operations: CreateAsync, UpdateAsync, DeleteAsync, GetAsync
  - Used as base for all typed repositories

- **IUserRepository & UserRepository**
  - User-specific queries: EmailExistsAsync, GetByEmailAsync

- **IPatientRepository & PatientRepository**
  - Patient-specific queries: GetPatientWithDetailsAsync, GetPatientByUserIdAsync

- **IDoctorRepository & DoctorRepository**
  - Doctor-specific queries: GetAllDoctorsWithDetailsAsync, GetDoctorWithDetailsAsync

- **IAppointmentRepository & AppointmentRepository**
  - Appointment queries: GetPatientAppointmentsAsync, GetDoctorAppointmentsAsync

#### **Data/** - Database Context
- **AppDbContext.cs** - Entity Framework Core DbContext
  - Defines DbSets for all entities
  - Configures relationships (1-to-1, 1-to-many, many-to-many)
  - OnModelCreating: Seeds initial doctors (Sarah Johnson, James Wilson, Emily Brown, Michael Davis)

#### **Migrations/** - Database Schema
EF Core migrations for database versioning.

- **20260402092128_InitialCreate** - Initial database schema
  - Creates tables for Users, Patients, Doctors, Appointments, Prescriptions, Medicines, Specializations
  - Sets up relationships and constraints

#### **Helpers/** - Utility Classes
- **JwtHelper.cs** - JWT token generation
  - Creates access tokens (60-minute expiration)
  - Creates refresh tokens (7-day expiration)
  - Validates token signatures

- **ErrorResponse.cs** - Standardized error response
  - Returns consistent error format to clients

- **CacheHelper.cs** - Memory caching utility
  - Generic caching with sliding expiration
  - Cache key management for doctors, patients, etc.
  - Pattern-based cache invalidation

#### **Middleware/** - HTTP Pipeline Handlers
- **ExceptionMiddleware.cs** - Global exception handling
  - Catches all exceptions and returns 500 with error details
  - Logs errors to Serilog

- **RequestLoggingMiddleware.cs** - Request/response logging
  - Logs all incoming requests
  - Logs response status and timing

#### **Mappings/** - AutoMapper Configuration
- **MappingProfile.cs** - DTO ↔ Entity mappings
  - User → UserDTO, LoginDTO, AuthResponseDTO
  - Patient → PatientDTO
  - Doctor → DoctorDTO
  - Appointment → AppointmentDTO

#### **Configuration Files**
- **appsettings.json**
  - Database connection string (SQL Server)
  - JWT settings (SecretKey, Issuer, Audience, ExpirationMinutes)
  - Serilog logging configuration

- **Program.cs**
  - Dependency injection setup
  - Database context registration
  - Authentication/JWT configuration
  - Swagger/OpenAPI setup
  - Service and repository registration

---

## 🖼️ Project 3: SmartHealthcare.Web (Frontend MVC)

**Purpose**: ASP.NET Core MVC web application providing user interface.

### 📂 Folders & Files:

#### **Controllers/** - MVC Controllers

**🏠 HomeController.cs** - Home & Dashboard
```
GET / - Home page (redirects to Dashboard if authenticated)
GET /Dashboard - Dashboard landing page
```
- Role-aware dashboard:
  - **Doctor dash**: My Patients, Appointments, Profile
  - **Patient dash**: Upcoming Appointments, Your Doctors, Prescriptions

**🔐 AuthController.cs** - User Authentication
```
GET /Auth/Register - Registration page (Patient/Doctor form)
POST /Auth/Register - Process registration
GET /Auth/Login - Login page
POST /Auth/Login - Process login (stores JWT in session)
GET /Auth/Profile - View user profile
POST /Auth/Logout - Logout (clears session)
```
- Handles user registration for both Patient and Doctor roles
- Stores JWT token in session: `Session["AuthToken"]`
- Profile shows: Name, Email, Role, Account created date

**👨‍⚕️ DoctorsController.cs** - Doctor Listing & Search
```
GET /Doctors/Index - List all doctors with search
GET /Doctors/Details/{id} - Doctor profile page
```
- Fetches from `/api/doctors` endpoint
- Implements client-side search filtering by name
- Shows: Doctor name, specializations, experience, consultation fee, availability
- Handles both direct array and paginated API responses
- Converts JsonElement to ExpandoObject for Razor binding

**🏥 PatientsController.cs** - Patient Management (Doctor-specific)
```
GET /Patients/Index - List patients [Authorize: Doctor, Admin]
GET /Patients/Details/{id} - Patient details [Authorize: Doctor, Admin]
```
- Doctors view their patient list
- Shows patient info: Name, Email, Phone, DOB, Gender, Address
- Click "View Details" for full patient information
- REST calls to `/api/patients` with JWT authentication

**📅 AppointmentsController.cs** - Appointment Management
```
GET /Appointments/Index - Current user's appointments
GET /Appointments/Book - Book appointment form [Authorize: Patient]
POST /Appointments/Create - Submit appointment [Authorize: Patient]
GET /Appointments/Details/{id} - View appointment details
```
- **Book**: Shows dropdown of all available doctors
- **Create**: Submits appointment with doctor, date, reason
- **Index**: Lists user's appointments (patient or doctor)
- **Details**: Full appointment information
- Fetches doctors from API and binds to dropdown
- Redirects to Index after successful booking

**💊 PrescriptionsController.cs** - Prescriptions (Dummy Data)
```
GET /Prescriptions/Index - List prescriptions
GET /Prescriptions/Details/{id} - Prescription details
```
- Currently uses dummy prescriptions (hardcoded)
- Shows: Medicine name, doctor, dosage, dates
- Instructions, side effects, precautions
- Print functionality with CSS styling

#### **Views/** - Razor Templates

**📁 Shared/** - Layout & Common Views
- **_Layout.cshtml** - Master layout template
  - Role-aware navigation bar:
    - **Doctor sees**: Dashboard, Patients, Profile
    - **Patient sees**: Dashboard, Book Appointment, My Appointments, Find Doctor, Profile
  - Header with logo: 🏥 SmartCare Health
  - Footer with copyright
  - Links to Bootstrap CSS and Serilog logging

- **_Layout.cshtml.css** - Global styles
  - Card layouts, buttons, colors
  - Navbar styling, responsive design

- **Error.cshtml** - Error display page

- **_ViewImports.cshtml** - Shared imports
  - Using statements for all views

- **_ViewStart.cshtml** - View initialization
  - Specifies default layout

**📁 Home/**
- **Index.cshtml** - Home page (before login)
  - Landing page with system description

- **Dashboard.cshtml** - Main dashboard (after login)
  - **Doctor Dashboard**:
    - Stats: My Patients, Appointments, Profile
    - Information card with quick links
  - **Patient Dashboard**:
    - Stats: Upcoming Appointments, Doctors, Prescriptions
    - Recent appointments list
    - Quick actions: Book, Find Doctor, Edit Profile

**📁 Auth/**
- **Register.cshtml** - Registration form
  - Fields: Full Name, Email, Password, Role dropdown (Patient/Doctor)
  - Validates email exists before submission

- **Login.cshtml** - Login form
  - Fields: Email, Password
  - Displays error messages on failed login
  - Links to registration page

- **Profile.cshtml** - User profile display
  - Shows: Name, Email, Role, Account created date
  - Edit button to modify profile

**📁 Doctors/**
- **Index.cshtml** - Doctor listing page
  - Search bar to filter doctors by name
  - Table with: Name, Specializations, Experience, Fee, Availability
  - "View Details" button for each doctor

- **Details.cshtml** - Doctor profile card
  - Full doctor information
  - Specializations list
  - Contact information
  - Back button, Book Appointment button

**📁 Appointments/**
- **Index.cshtml** - Appointment list
  - Shows user's appointments (patient or doctor)
  - Table with: Doctor, Date, Time, Status, Reason
  - "View Details" button for each appointment

- **Book.cshtml** - Book appointment form
  - Doctor dropdown (fetched from API)
  - Date picker
  - Reason/symptoms text area
  - Submit button
  - [Authorize: Patient] - Doctors can't access

- **Details.cshtml** - Appointment details
  - Full information display
  - Doctor name, date, time, status
  - Reason and any notes

**📁 Patients/**
- **Index.cshtml** - Patients list (Doctor view)
  - Table of all patients
  - Name, Email, Phone, DOB, Gender
  - "View Details" button
  - Gender icons (👨 for male, 👩 for female)

- **Details.cshtml** - Patient profile (Doctor view)
  - Full patient card with information
  - Name, Email, Phone, DOB, Gender, Address
  - Back button, Create Appointment button

**📁 Prescriptions/**
- **Index.cshtml** - Prescriptions list
  - Shows patient's prescriptions
  - Medicine names, doctors, dates
  - Status badges (Active/Completed)

- **Details.cshtml** - Prescription details
  - Prescription card layout
  - Medicine information
  - Doctor who prescribed
  - Dosage, dates, instructions
  - Side effects, precautions
  - Print button with print-specific CSS

#### **Services/** - API Client
- **IApiClient & ApiClient**
  - HttpClient wrapper for API calls
  - Generic `GetAsync<T>`, `PostAsync<T>` methods
  - Automatically includes JWT bearer token from session
  - Centralizes API communication logic

#### **Models/** - View Models
- **ErrorViewModel.cs** - Error display model
  - RequestId, ShowRequestId properties

#### **wwwroot/** - Static Assets
- **css/style.css** - Custom styling
  - Card layouts (card-custom, card-header-custom, card-body-custom)
  - Buttons (btn-primary-custom, btn-block-custom)
  - Badge styles (badge-custom, badge-success, badge-danger)
  - Color scheme: Purple (#6c4675), Teal (#2d7659)
  - Navbar styling, responsive design

- **js/site.js** - Client-side JavaScript
  - Form validation
  - Bootstrap modal handling
  - Search functionality

#### **Configuration Files**
- **appsettings.json**
  - API base URL: http://localhost:5125
  - Logging configuration

- **Program.cs**
  - Dependency injection for HttpClient, IApiClient
  - Session configuration
  - Authentication middleware
  - Swagger setup (for testing)

---

## 🔄 Data Flow Architecture

### User Registration (Doctor Example):
1. User fills registration form (Name, Email, Password, Role=Doctor)
2. **Web** → `AuthController.Register` (MVC)
3. **Web** → `AuthService.RegisterAsync()` (API)
4. `AuthService` creates User entity
5. Auto-creates Doctor entity with:
   - Phone: +1234567890
   - Qualification: "Not Specified"
   - Experience: 0 years
   - Consultation Fee: 500
   - Default specialization: "General"
6. Doctor cache invalidated
7. Login page displays

### User Login:
1. User enters Email & Password
2. **Web** → `AuthService.LoginAsync()` (API)
3. Validates credentials with BCrypt
4. Generates JWT token (userId claim, 60-min expiration)
5. Returns token to **Web**
6. **Web** stores token in `Session["AuthToken"]`
7. Redirects to Dashboard

### Doctor Booking by Patient:
1. Patient clicks "Book Appointment"
2. **Web** → `AppointmentsController.Book` (MVC)
3. Fetches doctors from `/api/doctors` with JWT token
4. **Web** returns form with doctor dropdown
5. Patient selects doctor, date, reason
6. **Web** → `AppointmentsController.Create` (MVC)
7. **Web** → `/api/appointments` POST with JWT token (API)
8. `AppointmentService` creates Appointment
9. Returns to appointment list

### Doctor Viewing Patients:
1. Doctor clicks "Patients" in navbar
2. **Web** → `PatientsController.Index()` (MVC)
3. Extracts JWT from Session
4. **Web** → `/api/patients` GET with JWT bearer token (API)
5. `PatientsController` returns all patients (role: Doctor)
6. **Web** displays patient table
7. Doctor clicks "View Details"
8. **Web** → `PatientsController.Details()` (MVC)
9. Displays patient card

---

## 🗄️ Database Schema (SQL Server)

### Tables:
- **Users** - User accounts (Patient, Doctor, Admin)
- **Patients** - Patient profiles (1-to-1 with Users)
- **Doctors** - Doctor profiles (1-to-1 with Users)
- **Appointments** - Appointments (Patient-Doctor linking)
- **Prescriptions** - Medical prescriptions
- **Medicines** - Medicine/drug catalog
- **PrescriptionMedicines** - Many-to-many (Prescription-Medicine)
- **Specializations** - Doctor specialties
- **DoctorSpecializations** - Many-to-many (Doctor-Specialization)

### Seed Data:
- 4 default doctors seeded on startup:
  1. Sarah Johnson - Cardiology
  2. James Wilson - General
  3. Emily Brown - Pediatrics
  4. Michael Davis - Orthopedics

---

## 🔐 Authentication & Authorization

### JWT Token Structure:
```json
{
  "userId": 1,
  "email": "doctor@example.com",
  "name": "Dr. Smith",
  "role": "Doctor",
  "iat": 1234567890,
  "exp": 1234571490
}
```

### Authorization Levels:
- **Public**: Index, Register, Login
- **Patient**: Book Appointment, My Appointments, Find Doctor, My Prescriptions
- **Doctor**: Patients list, My Appointments, Edit Profile
- **Admin**: All endpoints

### Role-Based Navigation:
- **Doctor navbar**: Dashboard, Patients, Profile, Logout
- **Patient navbar**: Dashboard, Book Appointment, My Appointments, Find Doctor, Profile, Logout

---

## 📊 Features Summary

| Feature | Patient | Doctor | Admin |
|---------|---------|--------|-------|
| Register | ✅ | ✅ | ✅ |
| View Dashboard | ✅ | ✅ | ✅ |
| Book Appointment | ✅ | ❌ | ✅ |
| View Appointments | ✅ | ✅ | ✅ |
| Find Doctor | ✅ | ❌ | ✅ |
| View Patients | ❌ | ✅ | ✅ |
| View Prescriptions | ✅ | ❌ | ✅ |
| Edit Profile | ✅ | ✅ | ✅ |

---

## 🚀 API Endpoints Quick Reference

### Auth
- `POST /api/auth/register`
- `POST /api/auth/login`

### Doctors
- `GET /api/doctors`
- `GET /api/doctors/{id}`
- `GET /api/doctors/search`
- `POST /api/doctors`
- `PUT /api/doctors/{id}`

### Patients
- `GET /api/patients`
- `GET /api/patients/{id}`
- `GET /api/patients/my-profile`
- `POST /api/patients`
- `PUT /api/patients/{id}`

### Appointments
- `GET /api/appointments`
- `GET /api/appointments/my`
- `GET /api/appointments/{id}`
- `POST /api/appointments`
- `PUT /api/appointments/{id}`

---

## 🛠️ Technologies Used

- **Backend**: ASP.NET Core 10.0 (REST API)
- **Frontend**: ASP.NET Core 10.0 (MVC)
- **Database**: SQL Server (KHUSHI23\SQLEXPRESS)
- **ORM**: Entity Framework Core
- **Authentication**: JWT Tokens (60-min access, 7-day refresh)
- **Mapping**: AutoMapper
- **Caching**: In-memory cache with sliding expiration
- **Hashing**: BCrypt for password hashing
- **Logging**: Serilog structured logging
- **UI Framework**: Bootstrap 5.1.3
- **HTTP Client**: HttpClient with custom wrapper

---

## 📝 Key Implementation Details

### Doctor Registration Flow:
1. User registers with Role="Doctor"
2. AuthService auto-creates Doctor entity
3. Doctor appears in doctor list immediately
4. Cache invalidated for fresh list

### Patient Book Appointment:
1. Patient selects doctor from dropdown
2. Appointment created with status="Pending"
3. Doctor can view in appointments list
4. Status can be updated (Confirmed, Cancelled, Completed)

### Role-Based UI:
- Navigation bar changes based on user role
- Dashboard shows different stats and actions
- Controllers enforce authorization with `[Authorize(Roles = "...")]`

### JSON Handling:
- API returns JsonDocument
- Web converts to ExpandoObject for Razor view binding
- Handles both array and paginated responses

### Session Management:
- JWT token stored in Session["AuthToken"]
- Token included in all API requests via bearer header
- Automatic logout on session expire

