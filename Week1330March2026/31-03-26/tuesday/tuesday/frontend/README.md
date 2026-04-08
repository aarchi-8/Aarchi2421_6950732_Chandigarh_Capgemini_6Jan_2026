# Learning Platform Frontend

A simple HTML/CSS/JavaScript frontend for the Learning Platform API.

## Features

- User registration and login
- Browse courses by category
- View course details and lessons
- Course enrollment (for students)
- Course creation (for instructors)
- Lesson management (for instructors)

## Setup

1. Make sure the backend API is running on `http://localhost:5265`
2. Open `index.html` in a web browser
3. Register a new account or login with existing credentials

## API Integration

The frontend communicates with the backend API using:

- Fetch API for HTTP requests
- JWT tokens stored in localStorage
- Automatic token inclusion in Authorization headers

## File Structure

```
frontend/
├── index.html          # Main HTML page
├── css/
│   └── styles.css      # Main stylesheet
└── js/
    ├── app.js          # Main application logic
    ├── auth.js         # Authentication functions
    └── courses.js      # Course-related functions
```

## Browser Support

Works in all modern browsers that support:

- ES6 modules
- Fetch API
- localStorage

## Security Notes

- JWT tokens are stored in localStorage (not secure for production)
- No HTTPS enforcement in development
- Client-side validation only (server validation is primary)
