# Learning Platform

A full-stack online learning platform built with ASP.NET Core Web API (.NET 9) and vanilla HTML/CSS/JavaScript.

## Features

### Backend (ASP.NET Core Web API)

- **Authentication**: JWT-based authentication with role-based authorization
- **Database**: EF Core with SQLite (One-to-One, One-to-Many, Many-to-Many relationships)
- **API Endpoints**:
  - User registration and login
  - Course CRUD operations
  - Lesson management
  - Course enrollment
- **Security**: Password hashing, JWT tokens, role-based access control
- **Caching**: In-memory caching for course listings
- **Validation**: Server-side validation with proper error responses
- **Documentation**: Swagger/OpenAPI documentation

### Frontend (HTML/CSS/JavaScript)

- **Authentication**: Login/register forms with client-side validation
- **Course Management**: Browse courses, view details, enroll (students)
- **Instructor Features**: Create courses, add lessons
- **Responsive Design**: Mobile-friendly UI
- **API Integration**: Fetch API with JWT token handling

## Project Structure

```
LearningPlatform.slnx
src/
├── LearningPlatform.Api/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── CoursesController.cs
│   │   └── EnrollController.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── DTOs/
│   │   ├── CourseDto.cs
│   │   ├── CreateCourseDto.cs
│   │   ├── CreateLessonDto.cs
│   │   ├── EnrollDto.cs
│   │   ├── LessonDto.cs
│   │   ├── ProfileDto.cs
│   │   ├── UserLoginDto.cs
│   │   └── UserRegisterDto.cs
│   ├── Models/
│   │   ├── Course.cs
│   │   ├── Enrollment.cs
│   │   ├── Lesson.cs
│   │   ├── Profile.cs
│   │   ├── Role.cs
│   │   └── User.cs
│   ├── Profiles/
│   │   └── MappingProfile.cs
│   ├── Services/
│   │   ├── IJwtService.cs
│   │   ├── JwtService.cs
│   │   └── PasswordHasher.cs
│   ├── appsettings.json
│   └── Program.cs
tests/
└── LearningPlatform.Tests/
    └── UnitTest1.cs
frontend/
├── index.html
├── demo.html
├── css/
│   └── styles.css
├── js/
│   ├── app.js
│   ├── auth.js
│   └── courses.js
└── README.md
```

## Prerequisites

- .NET 9 SDK
- SQLite (included with .NET)
- Modern web browser

## Setup and Running

### Backend Setup

1. **Navigate to the API project**:

   ```bash
   cd src/LearningPlatform.Api
   ```

2. **Restore packages**:

   ```bash
   dotnet restore
   ```

3. **Run the API**:

   ```bash
   dotnet run
   ```

   The API will be available at:
   - HTTPS: `http://localhost:5265`
   - Swagger UI: `http://localhost:5265/swagger`

### Frontend Setup

1. **Open the demo page**:
   - Open `frontend/demo.html` in your web browser
   - This provides a simple interface to test all API endpoints

2. **Or use the full application**:
   - Open `frontend/index.html` in your web browser
   - This provides the complete user interface

## API Endpoints

### Authentication

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

### Courses

- `GET /api/v1/courses` - Get all courses (cached)
- `GET /api/v1/courses/{id}` - Get course by ID
- `GET /api/v1/courses/category/{name}` - Get courses by category
- `POST /api/v1/courses` - Create course (Instructor/Admin only)
- `POST /api/v1/courses/{id}/lessons` - Add lesson to course (Instructor/Admin only)

### Enrollment

- `POST /api/v1/enroll` - Enroll in course (Student only)

## Database Schema

### Entities & Relationships

- **User** ↔ **Profile** (One-to-One)
- **User** → **Course** (One-to-Many, Instructor)
- **Course** → **Lesson** (One-to-Many)
- **User** ↔ **Course** (Many-to-Many via Enrollment)

### Roles

- **Student**: View courses, enroll
- **Instructor**: Create courses, add lessons
- **Admin**: Full access

## Testing

### Unit Tests

```bash
cd tests/LearningPlatform.Tests
dotnet test
```

### Manual Testing

1. Start the API
2. Open `frontend/demo.html`
3. Test registration, login, and course operations

## Security Features

- JWT token authentication
- Password hashing with SHA256
- Role-based authorization
- CORS enabled for frontend
- Input validation and sanitization

## Development Notes

- Database is automatically created on first run
- JWT secret is configured in `appsettings.json`
- Caching is implemented for course listings (5-minute expiry)
- Error handling includes proper HTTP status codes

## Browser Compatibility

Frontend works in all modern browsers supporting:

- ES6 features
- Fetch API
- localStorage
- CSS Grid and Flexbox

## Troubleshooting

### API Won't Start

- Ensure .NET 9 SDK is installed
- Check that ports 5001 are available
- Verify the connection string in `appsettings.json`

### Frontend Can't Connect

- Ensure API is running on HTTPS
- Check CORS settings in `Program.cs`
- Verify API_BASE URL in frontend JavaScript

### Database Issues

- Delete `learning.db` file to reset database
- Check SQLite installation

## Future Enhancements

- Pagination for course listings
- Search functionality
- File upload for course materials
- Email notifications
- Refresh token implementation
- Unit test coverage expansion
