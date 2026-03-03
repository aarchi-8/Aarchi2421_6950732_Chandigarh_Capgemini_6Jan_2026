function showLogin() {
    document.getElementById("registerBox").style.display = "none";
    document.getElementById("loginBox").style.display = "block";
}

function showRegister() {
    document.getElementById("registerBox").style.display = "block";
    document.getElementById("loginBox").style.display = "none";
}

function register() {

    var username = document.getElementById("regUser").value;
    var email = document.getElementById("regEmail").value;
    var password = document.getElementById("regPass").value;
    var confirm = document.getElementById("regConfirm").value;

    if (username == "" || email == "" || password == "" || confirm == "") {
        alert("Please fill all fields");
        return;
    }

    if (email.indexOf("@") == -1) {
        alert("Invalid email");
        return;
    }

    if (password.length < 6) {
        alert("Password must be at least 6 characters");
        return;
    }

    if (password != confirm) {
        alert("Passwords do not match");
        return;
    }

    localStorage.setItem("username", username);
    localStorage.setItem("password", password);

    alert("Registration Successful!");
    showLogin();
}

function login() {

    var username = document.getElementById("loginUser").value;
    var password = document.getElementById("loginPass").value;

    var savedUser = localStorage.getItem("username");
    var savedPass = localStorage.getItem("password");

    if (username == savedUser && password == savedPass) {
        alert("Login Successful!");
    } else {
        alert("Invalid Username or Password");
    }
}