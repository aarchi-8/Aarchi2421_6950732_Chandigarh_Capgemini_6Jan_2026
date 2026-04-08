using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartHealthcare.Web.Controllers
{
    [Authorize]
    public class HealthController : Controller
    {
        public IActionResult Profile()
        {
            // Redirect to unified Auth profile action for now.
            return RedirectToAction("Profile", "Auth");
        }

        public IActionResult Log()
        {
            return View();
        }

        public IActionResult Goals()
        {
            return View();
        }
    }
}