# 📝 Smart Healthcare - API Testing Guide

## **POSTMAN/THUNDER CLIENT COLLECTIONS**

Import these requests into Postman or Thunder Client for easy testing.

---

## **🔐 AUTHENTICATION FLOWS**

### **1. Register New User**
```
Method: POST
URL: http://localhost:5125/api/auth/register

Headers:
Content-Type: application/json

Body (JSON):
{
  "fullName": "Jane Smith",
  "email": "jane@example.com",
  "password": "SecurePassword123!",
  "role": "Patient"
}

Expected Response (201):
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "fullName": "Jane Smith",
  "email": "jane@example.com",
  "role": "Patient",
  "createdAt": "2026-04-02T10:30:00"
}
```

### **2. Login to Get Tokens**
```
Method: POST
URL: http://localhost:5125/api/auth/login

Headers:
Content-Type: application/json

Body (JSON):
{
  "email": "jane@example.com",
  "password": "SecurePassword123!"
}

Expected Response (200):
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI1NTBlODQwMC1lMjliLTQxZDQtYTcxNi00NDY2NTU0NDAwMDAiLCJlbWFpbCI6ImphbmVAZXhhbXBsZS5jb20iLCJyb2xlIjoiUGF0aWVudCIsIm5iZiI6MTc0NDczNDYwMCwiZXhwIjoxNzQ0NzM4MjAwfQ.xGZJjkkjdnsl...",
  "refreshToken": "jHk9+dKsL2M3nO4p5q6r7s8t9u0v1w2x3y4z5a6b7c8d9e0f1g2h3i4j5k6l7m8n9o0p",
  "fullName": "Jane Smith",
  "email": "jane@example.com",
  "expiry": "2026-04-02T11:30:00"
}

⚠️ IMPORTANT: Save the 'token' and 'refreshToken' for subsequent requests!
```

### **3. Use Token for Authenticated Requests**
```
For any request requiring authentication, add this header:

Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1l...
```

### **4. Refresh Expired Token**
```
Method: POST
URL: http://localhost:5125/api/auth/refresh-token

Headers:
Content-Type: application/json

Body (JSON):
{
  "refreshToken": "jHk9+dKsL2M3nO4p5q6r7s8t9u0v1w2x3y4z5a6b7c8d9e0f1g2h3i4j5k6l7m8n9o0p"
}

Expected Response (200):
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOi...",
  "refreshToken": "new_refresh_token_here_7_days_valid",
  "expiry": "2026-04-02T12:30:00"
}
```

---

## **👨‍⚕️ DOCTOR ENDPOINTS**

### **1. GET All Doctors (Cached)**
```
Method: GET
URL: http://localhost:5125/api/doctors

Expected Response (200):
[
  {
    "id": "...",
    "userId": "...",
    "phone": "9876543210",
    "qualification": "MBBS, MD",
    "experienceYears": 8,
    "consultationFee": 500,
    "isAvailable": true,
    "user": {
      "id": "...",
      "fullName": "Dr. Rajesh Kumar",
      "email": "rajesh@example.com",
      "role": "Doctor"
    }
  },
  ...
]

Note: First call hits database, second call returns cached (30 min TTL)
```

### **2. GET Doctor by ID (Cached)**
```
Method: GET
URL: http://localhost:5125/api/doctors/{id}

Example: http://localhost:5125/api/doctors/12345

Expected Response (200):
{
  "id": "12345",
  "phone": "9876543210",
  "qualification": "MBBS, MD",
  "experienceYears": 8,
  "consultationFee": 500,
  "isAvailable": true,
  "user": {
    "fullName": "Dr. Rajesh Kumar",
    "email": "rajesh@example.com",
    "role": "Doctor"
  }
}
```

### **3. Search Doctors with Pagination & Filtering** ⭐
```
Method: GET
URL: http://localhost:5125/api/doctors/search

Query Parameters (all optional):
- pageNumber: 1
- pageSize: 10
- searchTerm: cardio
- sortBy: name|experience|fee
- isDescending: true|false
- specializationId: 123
- minFee: 300
- maxFee: 700
- isAvailable: true

Example:
http://localhost:5125/api/doctors/search?pageNumber=1&pageSize=5&searchTerm=Dr&sortBy=fee&isDescending=false&isAvailable=true

Expected Response (200):
{
  "items": [
    {
      "id": "...",
      "phone": "9876543210",
      "qualification": "MBBS, MD",
      "experienceYears": 8,
      "consultationFee": 500,
      "isAvailable": true,
      "user": {
        "fullName": "Dr. Rajesh Kumar",
        "email": "rajesh@example.com"
      }
    }
  ],
  "totalCount": 25,
  "totalPages": 5,
  "pageNumber": 1,
  "pageSize": 5,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### **4. GET Doctors by Specialization (Cached)**
```
Method: GET
URL: http://localhost:5125/api/doctors/specialization/{specializationId}

Example: http://localhost:5125/api/doctors/specialization/3

Expected Response (200):
[
  {
    "id": "...",
    "phone": "9876543210",
    "qualification": "MBBS, MD",
    "experienceYears": 8,
    "consultationFee": 500,
    "isAvailable": true,
    "user": {
      "fullName": "Dr. Rajesh Kumar"
    }
  },
  ...
]
```

### **5. CREATE Doctor (Admin Only)**
```
Method: POST
URL: http://localhost:5125/api/doctors

Headers:
Authorization: Bearer YOUR_ADMIN_TOKEN
Content-Type: application/json

Body (JSON):
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "phone": "9876543210",
  "qualification": "MBBS, MD",
  "experienceYears": 8,
  "consultationFee": 500,
  "isAvailable": true
}

Expected Response (201):
{
  "id": "...",
  "userId": "...",
  "phone": "9876543210",
  "qualification": "MBBS, MD",
  "experienceYears": 8,
  "consultationFee": 500,
  "isAvailable": true
}

Note: Cache is cleared automatically after creation
```

### **6. UPDATE Doctor**
```
Method: PUT
URL: http://localhost:5125/api/doctors/{id}

Headers:
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

Body (JSON):
{
  "userId": "...",
  "phone": "9876543211",
  "qualification": "MBBS, MD, Fellowship",
  "experienceYears": 9,
  "consultationFee": 600,
  "isAvailable": true
}

Expected Response (200):
{
  "id": "...",
  "phone": "9876543211",
  "qualification": "MBBS, MD, Fellowship",
  "experienceYears": 9,
  "consultationFee": 600,
  "isAvailable": true
}

Note: Cache is cleared automatically after update
```

### **7. DELETE Doctor**
```
Method: DELETE
URL: http://localhost:5125/api/doctors/{id}

Headers:
Authorization: Bearer YOUR_TOKEN

Expected Response (204 No Content)

Note: Cache is cleared automatically after deletion
```

---

## **👥 PATIENT ENDPOINTS**

### **1. GET All Patients**
```
Method: GET
URL: http://localhost:5125/api/patients

Expected Response (200):
[
  {
    "id": "...",
    "userId": "...",
    "phone": "9876543210",
    "dateOfBirth": "1995-05-15",
    "gender": "Male",
    "address": "123 Main Street",
    "medicalHistory": "Hypertension",
    "user": {
      "fullName": "John Doe",
      "email": "john@example.com"
    }
  },
  ...
]
```

### **2. GET Patient by ID**
```
Method: GET
URL: http://localhost:5125/api/patients/{id}

Expected Response (200):
{
  "id": "...",
  "phone": "9876543210",
  "dateOfBirth": "1995-05-15",
  "gender": "Male",
  "address": "123 Main Street",
  "medicalHistory": "Hypertension",
  "user": {
    "fullName": "John Doe"
  }
}
```

### **3. GET My Profile (Logged-in Patient)**
```
Method: GET
URL: http://localhost:5125/api/patients/my-profile

Headers:
Authorization: Bearer YOUR_TOKEN

Expected Response (200):
{
  "id": "...",
  "phone": "9876543210",
  "dateOfBirth": "1995-05-15",
  "gender": "Male",
  "address": "123 Main Street",
  "medicalHistory": "Hypertension",
  "user": {
    "fullName": "John Doe",
    "email": "john@example.com"
  }
}
```

### **4. CREATE Patient**
```
Method: POST
URL: http://localhost:5125/api/patients

Headers:
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

Body (JSON):
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "phone": "9876543210",
  "dateOfBirth": "1995-05-15",
  "gender": "Male",
  "address": "123 Main Street",
  "medicalHistory": "Hypertension, Diabetes"
}

Expected Response (201):
{
  "id": "...",
  "userId": "...",
  "phone": "9876543210",
  "dateOfBirth": "1995-05-15",
  "gender": "Male",
  "address": "123 Main Street",
  "medicalHistory": "Hypertension, Diabetes"
}
```

### **5. UPDATE Patient**
```
Method: PUT
URL: http://localhost:5125/api/patients/{id}

Headers:
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

Body (JSON):
{
  "userId": "...",
  "phone": "9876543211",
  "dateOfBirth": "1995-05-15",
  "gender": "Male",
  "address": "456 New Street",
  "medicalHistory": "Hypertension"
}

Expected Response (200):
{
  "id": "...",
  "phone": "9876543211",
  "address": "456 New Street",
  ...
}
```

### **6. UPDATE Medical History**
```
Method: PATCH
URL: http://localhost:5125/api/patients/{id}/medical-history

Headers:
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

Body (JSON):
{
  "medicalHistory": "Hypertension, Diabetes, Asthma"
}

Expected Response (200):
{
  "id": "...",
  "medicalHistory": "Hypertension, Diabetes, Asthma",
  ...
}
```

### **7. DELETE Patient**
```
Method: DELETE
URL: http://localhost:5125/api/patients/{id}

Headers:
Authorization: Bearer YOUR_TOKEN

Expected Response (204 No Content)
```

---

## **📅 APPOINTMENT ENDPOINTS**

### **1. GET All Appointments**
```
Method: GET
URL: http://localhost:5125/api/appointments

Expected Response (200):
[
  {
    "id": "...",
    "patientId": "...",
    "doctorId": "...",
    "appointmentDate": "2026-04-10T14:30:00",
    "reason": "General Checkup",
    "status": "Scheduled",
    "notes": "Patient has fever",
    "createdAt": "2026-04-02T10:00:00",
    "patient": {
      "id": "...",
      "user": {
        "fullName": "John Doe"
      }
    },
    "doctor": {
      "id": "...",
      "user": {
        "fullName": "Dr. Rajesh Kumar"
      }
    }
  },
  ...
]
```

### **2. GET Appointment by ID**
```
Method: GET
URL: http://localhost:5125/api/appointments/{id}

Expected Response (200):
{
  "id": "...",
  "patientId": "...",
  "doctorId": "...",
  "appointmentDate": "2026-04-10T14:30:00",
  "reason": "General Checkup",
  "status": "Scheduled",
  "notes": "Patient has fever",
  "createdAt": "2026-04-02T10:00:00",
  "patient": {
    "user": {
      "fullName": "John Doe"
    }
  },
  "doctor": {
    "user": {
      "fullName": "Dr. Rajesh Kumar"
    }
  }
}
```

### **3. BOOK Appointment (CREATE)**
```
Method: POST
URL: http://localhost:5125/api/appointments

Headers:
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

Body (JSON):
{
  "patientId": "550e8400-e29b-41d4-a716-446655440000",
  "doctorId": "660e8400-e29b-41d4-a716-446655440000",
  "appointmentDate": "2026-04-10T14:30:00",
  "reason": "General Checkup"
}

Expected Response (201):
{
  "id": "...",
  "patientId": "...",
  "doctorId": "...",
  "appointmentDate": "2026-04-10T14:30:00",
  "reason": "General Checkup",
  "status": "Scheduled",
  "createdAt": "2026-04-02T10:00:00"
}

⚠️ Important: appointmentDate cannot be in the past
```

### **4. UPDATE Appointment**
```
Method: PUT
URL: http://localhost:5125/api/appointments/{id}

Headers:
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

Body (JSON):
{
  "patientId": "...",
  "doctorId": "...",
  "appointmentDate": "2026-04-11T15:00:00",
  "reason": "Follow-up Checkup"
}

Expected Response (200):
{
  "id": "...",
  "appointmentDate": "2026-04-11T15:00:00",
  "reason": "Follow-up Checkup",
  "status": "Scheduled",
  ...
}
```

### **5. UPDATE Appointment Status**
```
Method: PATCH
URL: http://localhost:5125/api/appointments/{id}/status

Headers:
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

Body (JSON):
{
  "status": "Confirmed"
}

Valid Statuses: "Scheduled", "Confirmed", "Cancelled", "Completed"

Expected Response (200):
{
  "id": "...",
  "status": "Confirmed",
  "notes": "Appointment confirmed by doctor",
  ...
}
```

### **6. DELETE Appointment**
```
Method: DELETE
URL: http://localhost:5125/api/appointments/{id}

Headers:
Authorization: Bearer YOUR_TOKEN

Expected Response (204 No Content)
```

---

## **⚠️ ERROR HANDLING**

### **Error Response Format**
```json
{
  "statusCode": 400,
  "message": "Validation failed or bad request",
  "errors": ["Field is required", "Email format invalid"]
}
```

### **Common HTTP Status Codes**
| Code | Meaning | Example |
|------|---------|---------|
| 200 | Success | GET request successful |
| 201 | Created | New resource created |
| 204 | No Content | DELETE successful |
| 400 | Bad Request | Invalid input data |
| 401 | Unauthorized | Missing or invalid token |
| 403 | Forbidden | No permission for action |
| 404 | Not Found | Resource doesn't exist |
| 500 | Server Error | Internal server error |

### **Example Error Response**
```
Method: POST /api/auth/login
Body: { "email": "invalid@example.com", "password": "wrong" }

Response (401):
{
  "statusCode": 401,
  "message": "Invalid credentials",
  "errors": ["Email or password is incorrect"]
}
```

---

## **🧪 COMPLETE TEST SCENARIO**

Follow these steps to test the entire flow:

### **Step 1: Register a User**
```
POST /api/auth/register
Body:
{
  "fullName": "Test User",
  "email": "test@example.com",
  "password": "TestPassword123!",
  "role": "Patient"
}
```

### **Step 2: Login**
```
POST /api/auth/login
Body:
{
  "email": "test@example.com",
  "password": "TestPassword123!"
}

Save the returned 'token' for next steps!
```

### **Step 3: View Doctors**
```
GET /api/doctors/search?pageNumber=1&pageSize=5&sortBy=consultationFee&isDescending=false

Headers:
Authorization: Bearer YOUR_TOKEN (from Step 2)
```

### **Step 4: Create Appointment**
```
POST /api/appointments
Headers:
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

Body:
{
  "patientId": "YOUR_PATIENT_ID",
  "doctorId": "SELECTED_DOCTOR_ID",
  "appointmentDate": "2026-04-10T14:30:00",
  "reason": "General Checkup"
}
```

### **Step 5: View My Appointments**
```
GET /api/appointments

Headers:
Authorization: Bearer YOUR_TOKEN
```

### **Step 6: Update Appointment Status**
```
PATCH /api/appointments/{appointmentId}/status

Headers:
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

Body:
{
  "status": "Confirmed"
}
```

---

## **💡 TESTING TIPS**

1. **Keep Token Handy** - Copy the token from login response and paste in Authorization header for subsequent requests
2. **Test Progressive** - Start with authentication, then move to other endpoints
3. **Use Postman Collections** - Save these as a collection for quick testing later
4. **Monitor Network** - Watch the response times (first request slower due to caching)
5. **Test with Invalid Data** - Try sending incomplete data to see error messages
6. **Check DateTime Format** - Always use `YYYY-MM-DDTHH:mm:ss` for dates
7. **Test Pagination** - Try different pageSize values (5, 10, 20) to see pagination work

---

**Happy Testing! 🚀**
