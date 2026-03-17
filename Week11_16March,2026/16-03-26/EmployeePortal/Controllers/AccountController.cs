using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementPortal.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "admin123")
            {
                HttpContext.Session.SetString("user", username);
                HttpContext.Session.SetString("role", "admin");

                return RedirectToAction("Dashboard");
            }

            ViewBag.msg = "Invalid Login";
            return View();
        }

        public IActionResult Dashboard()
        {
            var user = HttpContext.Session.GetString("user");

            if (user == null)
                return RedirectToAction("Login");

            ViewBag.user = user;

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}