using Microsoft.AspNetCore.Mvc;
using SmartHealthCareSystem.MVC.Services;

namespace SmartHealthCareSystem.MVC.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ApiService _apiService;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseController(ApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected string? GetUserRole() => 
            _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

        protected int? GetUserId() => 
            _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");

        protected string? GetUserName() => 
            _httpContextAccessor.HttpContext?.Session.GetString("UserName");

        protected string? GetUserEmail() => 
            _httpContextAccessor.HttpContext?.Session.GetString("UserEmail");

        protected string? GetJwtToken() => 
            _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");

        protected bool IsAuthenticated() => 
            !string.IsNullOrEmpty(GetJwtToken());

        protected bool IsInRole(string role) => 
            GetUserRole() == role;

        protected bool IsAdmin() => IsInRole("Admin");
        protected bool IsDoctor() => IsInRole("Doctor");
        protected bool IsPatient() => IsInRole("Patient");

        protected IActionResult RequireAuth()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");
            return new EmptyResult();
        }

        protected IActionResult RequireRole(string role)
        {
            if (!IsInRole(role))
                return Unauthorized();
            return new EmptyResult();
        }

        protected async Task<IActionResult> ExecuteWithFallbackAsync<T>(Func<Task<T>> action, T fallbackValue)
        {
            try
            {
                var result = await action();
                return View(result);
            }
            catch
            {
                return View(fallbackValue);
            }
        }
    }
}
