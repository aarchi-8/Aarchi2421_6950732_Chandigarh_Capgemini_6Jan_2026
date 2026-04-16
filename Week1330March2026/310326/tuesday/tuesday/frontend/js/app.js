// API base URL - adjust this to match your backend
const API_BASE = "http://localhost:5265";

// Global state
let currentUser = null;
let currentToken = null;

// DOM elements
const navbar = document.getElementById("navbar");
const mainContent = document.getElementById("main-content");
const navLinks = document.getElementById("nav-links");

// Initialize app
document.addEventListener("DOMContentLoaded", () => {
  setupNavigation();
  checkAuthStatus();
  loadHomePage();
});

// Setup navigation event listeners
function setupNavigation() {
  document.getElementById("login-link").addEventListener("click", (e) => {
    e.preventDefault();
    loadLoginPage();
  });

  document.getElementById("register-link").addEventListener("click", (e) => {
    e.preventDefault();
    loadRegisterPage();
  });

  document.getElementById("courses-link").addEventListener("click", (e) => {
    e.preventDefault();
    loadCoursesPage();
  });

  document
    .getElementById("create-course-link")
    .addEventListener("click", (e) => {
      e.preventDefault();
      loadCreateCoursePage();
    });

  document.getElementById("logout-link").addEventListener("click", (e) => {
    e.preventDefault();
    logout();
  });
}

// Check if user is authenticated
function checkAuthStatus() {
  const token = localStorage.getItem("token");
  const user = localStorage.getItem("user");

  if (token && user) {
    currentToken = token;
    currentUser = JSON.parse(user);
    updateNavbar(true);
  } else {
    updateNavbar(false);
  }
}

// Update navbar based on auth status
function updateNavbar(isAuthenticated) {
  const loginLink = document.getElementById("login-link");
  const registerLink = document.getElementById("register-link");
  const coursesLink = document.getElementById("courses-link");
  const createCourseLink = document.getElementById("create-course-link");
  const logoutLink = document.getElementById("logout-link");

  if (isAuthenticated) {
    loginLink.style.display = "none";
    registerLink.style.display = "none";
    coursesLink.style.display = "inline";
    logoutLink.style.display = "inline";

    // Show create course link only for instructors and admins
    if (
      currentUser &&
      (currentUser.role === "Instructor" || currentUser.role === "Admin")
    ) {
      createCourseLink.style.display = "inline";
    }
  } else {
    loginLink.style.display = "inline";
    registerLink.style.display = "inline";
    coursesLink.style.display = "none";
    createCourseLink.style.display = "none";
    logoutLink.style.display = "none";
  }
}

// Load home page
function loadHomePage() {
  if (currentUser) {
    loadCoursesPage();
  } else {
    mainContent.innerHTML = `
            <div class="container">
                <div class="welcome-section">
                    <h1>Welcome to Learning Platform</h1>
                    <p>Discover new skills, expand your knowledge, and achieve your learning goals with our comprehensive course catalog.</p>
                    <div style="margin-top: 2rem;">
                        <button class="btn" onclick="loadLoginPage()">Login</button>
                        <button class="btn btn-secondary" onclick="loadRegisterPage()">Get Started</button>
                    </div>
                </div>
            </div>
        `;
  }
}

// Load login page
function loadLoginPage() {
  mainContent.innerHTML = `
        <div class="container">
            <div class="form-container">
                <h2>Login</h2>
                <form id="login-form">
                    <div class="form-group">
                        <label for="email">Email:</label>
                        <input type="email" id="email" name="email" required>
                    </div>
                    <div class="form-group">
                        <label for="password">Password:</label>
                        <input type="password" id="password" name="password" required>
                    </div>
                    <button type="submit" class="btn">Login</button>
                </form>
                <div id="login-message"></div>
            </div>
        </div>
    `;

  document.getElementById("login-form").addEventListener("submit", handleLogin);
}

// Load register page
function loadRegisterPage() {
  mainContent.innerHTML = `
        <div class="container">
            <div class="form-container">
                <h2>Register</h2>
                <form id="register-form">
                    <div class="form-group">
                        <label for="username">Username:</label>
                        <input type="text" id="username" name="username" required>
                    </div>
                    <div class="form-group">
                        <label for="reg-email">Email:</label>
                        <input type="email" id="reg-email" name="reg-email" required>
                    </div>
                    <div class="form-group">
                        <label for="reg-password">Password:</label>
                        <input type="password" id="reg-password" name="reg-password" required minlength="6">
                    </div>
                    <div class="form-group">
                        <label for="role">Role:</label>
                        <select id="role" name="role" required>
                            <option value="Student">Student</option>
                            <option value="Instructor">Instructor</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="firstName">First Name:</label>
                        <input type="text" id="firstName" name="firstName">
                    </div>
                    <div class="form-group">
                        <label for="lastName">Last Name:</label>
                        <input type="text" id="lastName" name="lastName">
                    </div>
                    <button type="submit" class="btn">Register</button>
                </form>
                <div id="register-message"></div>
            </div>
        </div>
    `;

  document
    .getElementById("register-form")
    .addEventListener("submit", handleRegister);
}

// Load courses page
function loadCoursesPage() {
  if (!currentToken) {
    loadLoginPage();
    return;
  }

  mainContent.innerHTML = `
        <div class="container">
            <h1>Courses</h1>
            <div class="form-group" style="margin-bottom: 2rem;">
                <label for="category-filter">Filter by Category:</label>
                <select id="category-filter">
                    <option value="">All Categories</option>
                </select>
            </div>
            <div id="courses-container" class="grid">
                <div class="loading">
                    <div class="spinner"></div>
                    <p>Loading courses...</p>
                </div>
            </div>
        </div>
    `;

  loadCourses();
  loadCategories();
}

// Load create course page
function loadCreateCoursePage() {
  if (
    !currentToken ||
    (currentUser.role !== "Instructor" && currentUser.role !== "Admin")
  ) {
    showMessage("Access denied. Only instructors can create courses.", "error");
    return;
  }

  mainContent.innerHTML = `
        <div class="container">
            <div class="form-container">
                <h2>Create New Course</h2>
                <form id="create-course-form">
                    <div class="form-group">
                        <label for="course-title">Title:</label>
                        <input type="text" id="course-title" name="title" required>
                    </div>
                    <div class="form-group">
                        <label for="course-description">Description:</label>
                        <textarea id="course-description" name="description" required rows="4"></textarea>
                    </div>
                    <div class="form-group">
                        <label for="course-category">Category:</label>
                        <input type="text" id="course-category" name="category">
                    </div>
                    <button type="submit" class="btn">Create Course</button>
                </form>
                <div id="create-course-message"></div>
            </div>
        </div>
    `;

  document
    .getElementById("create-course-form")
    .addEventListener("submit", handleCreateCourse);
}

// Logout
function logout() {
  localStorage.removeItem("token");
  localStorage.removeItem("user");
  currentToken = null;
  currentUser = null;
  updateNavbar(false);
  loadHomePage();
}

// Show message
function showMessage(message, type = "info") {
  const messageDiv = document.createElement("div");
  messageDiv.className = `alert alert-${type}`;
  messageDiv.innerHTML = `
    <div style="display: flex; align-items: center; gap: 0.5rem;">
      <span style="font-weight: bold;">
        ${type === "success" ? "✓" : type === "error" ? "✕" : type === "info" ? "ℹ" : "⚠"}
      </span>
      ${message}
    </div>
  `;
  mainContent.insertBefore(messageDiv, mainContent.firstChild);

  setTimeout(() => {
    messageDiv.style.animation = "fadeOut 0.3s ease-out";
    setTimeout(() => messageDiv.remove(), 300);
  }, 5000);
}
