using BookStore.Web.Models;
using BookStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
namespace BookStore.Web.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _api;
    public AuthController(ApiService api) => _api = api;
    public IActionResult Login() => View();
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var r = await _api.PostWithResponseAsync<object, AuthResult>("/api/v1/auth/login", new { model.Email, model.Password });
        if (!r.Success || r.Data == null) { ModelState.AddModelError("", "Invalid credentials."); return View(model); }
        HttpContext.Session.SetString("JwtToken", r.Data.Token); HttpContext.Session.SetString("UserName", r.Data.FullName); HttpContext.Session.SetString("UserRole", r.Data.Role);
        return r.Data.Role == "Admin" ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Books");
    }
    public IActionResult Register() => View();
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var r = await _api.PostAsync("/api/v1/auth/register", new { model.FullName, model.Email, model.Password, model.Phone });
        if (!r.Success) { ModelState.AddModelError("", r.Message); return View(model); }
        TempData["Message"] = "Registration successful! Please login."; return RedirectToAction("Login");
    }
    public IActionResult Logout() { HttpContext.Session.Clear(); return RedirectToAction("Login"); }
}