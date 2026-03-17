using EmployeePortal.Filters;
using EmployeePortal.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementPortal.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    public class HRController : Controller
    {
        static List<Employee> employees = new List<Employee>();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EmployeeList()
        {
            return View(employees);
        }

        public IActionResult Reports()
        {
            // Test exception
            throw new Exception("Test error in Reports");

            return View();
        }
    }
}