using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace SessionBasedLogin.Controllers
{
    public class AccountController : Controller
    {

        // Login page
        public IActionResult Login()
        {
            return View();
        }

        // Login form submit
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "123")
            {
                HttpContext.Session.SetString("username", username);
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.Error = "Invalid Username or Password";
                return View();
            }
        }

        // Dashboard page
        public IActionResult Dashboard()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Username = username;
            return View();
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}