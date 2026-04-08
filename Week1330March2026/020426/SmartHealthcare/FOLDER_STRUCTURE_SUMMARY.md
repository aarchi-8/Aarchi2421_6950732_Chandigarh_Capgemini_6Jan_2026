# 🏥 Smart Healthcare - Quick Folder Breakdown

## 3 Main Projects

### 1️⃣ **SmartHealthcare.Models** (Shared Data Layer)
**What**: Entities, DTOs, Enums shared between API & Web

**Folders**:
- **DTOs/** - Data transfer objects (UserDTOs, PatientDTOs, DoctorDTOs, AppointmentDTOs, etc.)
- **Entities/** - Database models (User, Patient, Doctor, Appointment, Prescription, Medicine, Specialization)
- **Enums/** - Gender, AppointmentStatus

---

### 2️⃣ **SmartHealthcare.API** (REST Backend - Port 5125)
**What**: Business logic, database operations, API endpoints

**Key Folders**:

| Folder | Purpose |
|--------|---------|
| **Controllers/** | API endpoints (Auth, Doctors, Patients, Appointments) |
| **Services/** | Business logic (AuthService, DoctorService, PatientService, AppointmentService) |
| **Repositories/** | Database access layer (GenericRepository + specialized repos) |
| **Data/** | Entity Framework DbContext, database seeding (4 doctors) |
| **Migrations/** | Database schema versioning |
| **Helpers/** | Utilities (JwtHelper, CacheHelper, ErrorResponse) |
| **Middleware/** | HTTP pipeline (ExceptionMiddleware, RequestLoggingMiddleware) |
| **Mappings/** | AutoMapper configurations (Entity ↔ DTO) |

**Main API Routes**:
- `POST /api/auth/register` - Register user (Patient/Doctor)
- `POST /api/auth/login` - Login with JWT token
- `GET /api/doctors` - Get all doctors (for booking)
- `GET /api/patients` - Doctors view their patients
- `POST /api/appointments` - Patient books appointment
- `GET /api/appointments/my` - User's appointments

---

### 3️⃣ **SmartHealthcare.Web** (MVC Frontend - Port 5272)
**What**: User interface, web controllers, views, styling

**Key Folders**:

| Folder | Purpose |
|--------|---------|
| **Controllers/** | MVC controllers (Home, Auth, Doctors, Patients, Appointments, Prescriptions) |
| **Views/** | Razor templates (.cshtml files) for rendering HTML |
| **Services/** | ApiClient wrapper for calling API endpoints |
| **Models/** | View models, ErrorViewModel |
| **wwwroot/** | Static assets (CSS, JavaScript, images) |

**Main MVC Routes**:
- `GET / or /Dashboard` - Home page
- `GET/POST /Auth/Register` - User registration
- `GET/POST /Auth/Login` - User login
- `GET /Doctors` - Browse doctors list
- `GET/POST /Appointments/Book` - Book appointment form
- `GET /Appointments` - View user's appointments
- `GET /Patients` - Doctors view patient list
- `GET /Auth/Profile` - User profile

---

## 📂 Views Detailed Breakdown

**Shared/** - Layout & common views
- `_Layout.cshtml` - Role-aware navbar (Doctor sees different menu than Patient)
- `_Layout.cshtml.css` - Styling

**Home/** - Dashboard
- `Dashboard.cshtml` - Role-aware dashboard (Doctor vs Patient different layouts)

**Auth/** - User authentication
- `Register.cshtml` - Registration form (Patient/Doctor role selector)
- `Login.cshtml` - Login form
- `Profile.cshtml` - User profile display

**Doctors/** - Doctor listing
- `Index.cshtml` - Doctor list with search filter
- `Details.cshtml` - Individual doctor profile

**Appointments/** - Appointment booking & viewing
- `Index.cshtml` - User's appointments list
- `Book.cshtml` - Book appointment form (Doctor dropdown from API)
- `Details.cshtml` - Appointment details

**Patients/** - Doctor's patient list (NEW)
- `Index.cshtml` - Patients table (Doctors only)
- `Details.cshtml` - Patient info card (Doctors only)

**Prescriptions/** - Medicine prescriptions
- `Index.cshtml` - Prescriptions list
- `Details.cshtml` - Prescription with instructions & side effects

---

## 🔄 How It Works Together

### User Registration Flow (Doctor):
```
1. Web: Register page → User fills form (Name, Email, Password, Role="Doctor")
2. Web: POST /Auth/Register calls API
3. API: AuthService.RegisterAsync()
   - Creates User entity
   - Auto-creates Doctor entity (Phone, Qualification, Experience, Fee, Specialization)
   - Returns JWT token
4. Web: Stores token in Session["AuthToken"]
5. User redirected to Login
```

### Doctor Booking by Patient:
```
1. Web: Patient clicks "Book Appointment"
2. Web: AppointmentsController.Book()
   - Calls API GET /api/doctors with JWT token
   - Renders form with doctor dropdown
3. Patient selects doctor, date, reason
4. Web: POST /Appointments/Create
   - Calls API POST /api/appointments with JWT token
5. API: Creates Appointment entity
6. Web: Redirects to appointments list
```

### Doctor Viewing Patients:
```
1. Web: Doctor clicks "Patients" navbar
2. Web: PatientsController.Index()
   - Calls API GET /api/patients with JWT token
   - Filters to show current doctor's patients
3. Web: Renders patient table
4. Doctor clicks "View Details"
5. Web: Shows patient card with full info
```

---

## 🎯 Role-Based Access

### Navigation Bar Changes by Role:

**Doctor Sees:**
- 🏠 Dashboard
- 👥 Patients
- 👤 Profile
- 🚪 Logout

**Patient Sees:**
- 🏠 Dashboard
- 📅 Book Appointment
- 📋 My Appointments
- 🔍 Find Doctor
- 👤 Profile
- 🚪 Logout

### Dashboard Changes by Role:

**Doctor Dashboard:**
- My Patients (count)
- Upcoming Appointments (count)
- Doctor Information card

**Patient Dashboard:**
- Upcoming Appointments (count)
- Your Doctors (count)
- Prescriptions count
- Recent appointments list
- Quick actions (Book, Find Doctor, Edit Profile)

---

## 🗄️ Database (SQL Server)

**Core Tables:**
- `Users` - User accounts
- `Patients` - Patient profiles (1-to-1 with Users)
- `Doctors` - Doctor profiles (1-to-1 with Users)
- `Appointments` - Booking records
- `Prescriptions` - Medicine prescriptions
- `Medicines` - Drug catalog
- `Specializations` - Cardiology, General, Pediatrics, etc.
- `DoctorSpecializations` - Many-to-many (Doctor can have multiple specializations)

**Seed Data (Auto-loaded):**
- 4 default doctors: Sarah Johnson, James Wilson, Emily Brown, Michael Davis

---

## 🔐 Authentication

- JWT tokens generated on login (60-minute expiration)
- Token stored in `Session["AuthToken"]`
- All API calls include token as Bearer header
- Password hashed with BCrypt

---

## 🎨 Styling

- **Color scheme**: Purple (#6c4675) & Teal (#2d7659)
- **Framework**: Bootstrap 5.1.3
- **Custom CSS**: Card layouts, buttons, badges
- **Icons**: Emoji for quick visual identification

---

## 📊 Key Features

✅ User Registration (Patient/Doctor)
✅ JWT Authentication
✅ Doctor Listing with Search
✅ Appointment Booking
✅ Appointment Management
✅ Patient Management (Doctor view)
✅ Role-Based Navigation
✅ Role-Specific Dashboards
✅ Prescription Viewing
✅ Profile Management
✅ Serilog Logging
✅ Error Handling Middleware
✅ AutoMapper for DTOs
✅ Caching with invalidation

---

## 🚀 Current Status

**Completed:**
- ✅ Registration (Patient & Doctor auto-profile creation)
- ✅ Login with JWT
- ✅ Doctor listing & browsing
- ✅ Appointment booking
- ✅ Appointment viewing
- ✅ Patient management (Doctor view)
- ✅ Role-based UI filtering
- ✅ Role-aware dashboard
- ✅ Prescription viewing (dummy data)
- ✅ User profile page

**Technology Stack:**
- ASP.NET Core 10.0
- Entity Framework Core
- SQL Server
- Bootstrap 5
- Serilog
- AutoMapper
- BCrypt
- JWT Tokens

