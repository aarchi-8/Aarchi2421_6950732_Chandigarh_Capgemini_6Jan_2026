// Authentication functions

// Handle login form submission
async function handleLogin(e) {
  e.preventDefault();

  const submitButton = e.target.querySelector('button[type="submit"]');
  const email = document.getElementById("email").value;
  const password = document.getElementById("password").value;

  // Client-side validation
  if (!email || !password) {
    showMessage("Please fill in all fields", "error");
    return;
  }

  if (!isValidEmail(email)) {
    showMessage("Please enter a valid email address", "error");
    return;
  }

  showLoading(submitButton, "Signing in...");

  try {
    const response = await fetch(`${API_BASE}/api/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ email, password }),
    });

    const data = await response.json();

    if (response.ok) {
      // Store token and user info
      localStorage.setItem("token", data.token);
      localStorage.setItem("user", JSON.stringify(data.user));

      currentToken = data.token;
      currentUser = data.user;

      updateNavbar(true);
      showMessage("Login successful!", "success");
      loadCoursesPage();
    } else {
      showMessage(data.message || "Login failed", "error");
    }
  } catch (error) {
    console.error("Login error:", error);
    showMessage("Network error. Please try again.", "error");
  } finally {
    hideLoading(submitButton);
  }
}

// Handle register form submission
async function handleRegister(e) {
  e.preventDefault();

  const submitButton = e.target.querySelector('button[type="submit"]');
  const username = document.getElementById("username").value;
  const email = document.getElementById("reg-email").value;
  const password = document.getElementById("reg-password").value;
  const role = document.getElementById("role").value;
  const firstName = document.getElementById("firstName").value;
  const lastName = document.getElementById("lastName").value;

  // Client-side validation
  if (!username || !email || !password || !role) {
    showMessage("Please fill in all required fields", "error");
    return;
  }

  if (!isValidEmail(email)) {
    showMessage("Please enter a valid email address", "error");
    return;
  }

  if (password.length < 6) {
    showMessage("Password must be at least 6 characters long", "error");
    return;
  }

  const profile = firstName && lastName ? { firstName, lastName } : null;

  showLoading(submitButton, "Creating account...");

  try {
    const response = await fetch(`${API_BASE}/api/auth/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ username, email, password, role, profile }),
    });

    const data = await response.json();

    if (response.ok) {
      showMessage("Registration successful! Please login.", "success");
      loadLoginPage();
    } else {
      showMessage(data.title || "Registration failed", "error");
    }
  } catch (error) {
    console.error("Register error:", error);
    showMessage("Network error. Please try again.", "error");
  } finally {
    hideLoading(submitButton);
  }
}

// API call helper with authentication
async function apiCall(endpoint, options = {}) {
  const defaultOptions = {
    headers: {
      "Content-Type": "application/json",
      ...options.headers,
    },
  };

  if (currentToken) {
    defaultOptions.headers["Authorization"] = `Bearer ${currentToken}`;
  }

  const response = await fetch(`${API_BASE}${endpoint}`, {
    ...defaultOptions,
    ...options,
  });

  if (response.status === 401) {
    // Token expired or invalid
    logout();
    throw new Error("Authentication required");
  }

  return response;
}

// Email validation helper
function isValidEmail(email) {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}
