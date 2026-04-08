using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Web.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SmartHealthcare.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IApiClient apiClient, ILogger<AuthController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Dashboard", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _apiClient.PostAsync<AuthResponseDTO>("auth/login", model);
                
                if (result != null)
                {
                    // Decode JWT to get userId
                    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(result.Token);
                    var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? "0";
                    
                    // Try to get PatientId from JWT claims
                    var patientIdClaim = token.Claims.FirstOrDefault(c => c.Type == "patientId")?.Value;

                    // Store token and userId
                    HttpContext.Session.SetString("AuthToken", result.Token);
                    HttpContext.Session.SetString("UserId", userIdClaim);
                    
                    if (!string.IsNullOrEmpty(patientIdClaim))
                    {
                        HttpContext.Session.SetString("PatientId", patientIdClaim);
                        _logger.LogInformation("Stored PatientId from JWT: {PatientId}", patientIdClaim);
                    }
                    else
                    {
                        // Try to fetch patient info from API if role is Patient
                        if (result.Role == "Patient" && int.TryParse(userIdClaim, out var userId))
                        {
                            try
                            {
                                _apiClient.SetAuthToken(result.Token);
                                // Try calling a hypothetical endpoint to get current patient
                                // For now, we'll use a workaround: query patients/my-profile or similar
                                _logger.LogInformation("PatientId not in JWT, will fetch from API if needed");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Could not fetch PatientId from API");
                            }
                        }
                    }

                    // Create claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, result.Email),
                        new Claim(ClaimTypes.Name, result.FullName),
                        new Claim(ClaimTypes.Role, result.Role),
                        new Claim("userId", userIdClaim)
                    };
                    
                    if (!string.IsNullOrEmpty(patientIdClaim))
                        claims.Add(new Claim("patientId", patientIdClaim));

                    // Create principal and sign in
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = result.Expiry
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation("User {Email} logged in successfully with userId: {UserId}", result.Email, userIdClaim);
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ViewBag.Error = "Invalid email or password";
                    _logger.LogWarning("Failed login attempt for {Email}", model.Email);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred. Please try again.";
                _logger.LogError(ex, "Login error for {Email}", model.Email);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Dashboard", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _apiClient.PostAsync<UserDTO>("auth/register", model);
                
                if (result != null)
                {
                    ViewBag.Message = "Registration successful! Please log in.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = "Email already exists or registration failed";
                    _logger.LogWarning("Registration failed for {Email}", model.Email);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred. Please try again.";
                _logger.LogError(ex, "Registration error for {Email}", model.Email);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login");

            return View();
        }
    }
}
