using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        static List<Student> students = new List<Student>();

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Student student)
        {
            if (ModelState.IsValid)
            {
                students.Add(student);

                TempData["msg"] = "Student Registered Successfully";

                return RedirectToAction("Details", student);
            }

            return View(student);
        }

        public IActionResult Details(Student student)
        {
            return View(student);
        }
    }
}