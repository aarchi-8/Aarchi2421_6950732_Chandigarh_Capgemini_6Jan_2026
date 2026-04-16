using Microsoft.AspNetCore.Mvc;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.MVC.Services;

namespace SmartHealthCareSystem.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApiService apiService, ILogger<AuthController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login failed - ModelState invalid");
                return View(loginDto);
            }

            try
            {
                var loginResult = await _apiService.LoginAsync(loginDto);
                
                if (loginResult == null)
                {
                    _logger.LogWarning("Login failed - API returned null for email: {Email}", loginDto.Email);
                    ModelState.AddModelError("", "Invalid email or password");
                    return View(loginDto);
                }
                
                _logger.LogInformation("Login successful for user: {UserId}, Role: {Role}", loginResult.UserId, loginResult.Role);
                
                // Store session data
                HttpContext.Session.SetString("JWTToken", loginResult.Token);
                HttpContext.Session.SetString("UserRole", loginResult.Role);
                HttpContext.Session.SetString("UserName", loginResult.FullName);
                HttpContext.Session.SetString("UserEmail", loginDto.Email);
                HttpContext.Session.SetInt32("UserId", loginResult.UserId);
                
                _logger.LogInformation("Session data stored. Role: {Role}, UserId: {UserId}", loginResult.Role, loginResult.UserId);
                
                // Role-based redirection
                if (loginResult.Role == "Admin")
                {
                    _logger.LogInformation("Redirecting Admin to Dashboard");
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (loginResult.Role == "Doctor")
                {
                    _logger.LogInformation("Redirecting Doctor to DoctorProfile");
                    return RedirectToAction("DoctorProfile", "Home");
                }
                else if (loginResult.Role == "Patient")
                {
                    _logger.LogInformation("Redirecting Patient to PatientDashboard");
                    return RedirectToAction("PatientDashboard", "Home");
                }
                
                _logger.LogWarning("Unknown role: {Role}, redirecting to Home", loginResult.Role);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login exception for email: {Email}", loginDto.Email);
                ModelState.AddModelError("", $"Login failed: {ex.Message}");
                return View(loginDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            try
            {
                var departments = await _apiService.GetDepartmentsAsync();
                ViewBag.Departments = departments ?? new List<DepartmentDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load departments for registration");
                ViewBag.Departments = new List<DepartmentDto>();
                ViewBag.ErrorMessage = "Unable to load departments. Please try again later.";
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CreateUserDto registerDto)
        {
            if (!ModelState.IsValid)
                return View(registerDto);

            try
            {
                await _apiService.RegisterAsync(registerDto);
                return RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError("", "Registration failed");
                return View(registerDto);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWTToken");
            return RedirectToAction("Index", "Home");
        }
    }
}
