using Employee_Project_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee_Project_Management_System.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Projects.ToList());
        }

        public IActionResult EmployeesInProject(int id)
        {
            var employees = _context.EmployeeProjects
    .Include(ep => ep.Employee)
    .ThenInclude(e => e.Department)
    .Where(ep => ep.ProjectId == id)
    .Select(ep => ep.Employee)
    .ToList();
            return View(employees);
        }

        public IActionResult Dashboard()
        {
            var dashboard = _context.Projects
                .Include(p => p.EmployeeProjects)
                .ThenInclude(ep => ep.Employee)
                .ThenInclude(e => e.Department)
                .ToList();

            return View(dashboard);
        }

        public IActionResult Create()
        {
            ViewBag.Employees = _context.Employees.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Project project, List<int> employeeIds)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();

            if (employeeIds != null)
            {
                foreach (var eid in employeeIds)
                {
                    _context.EmployeeProjects.Add(new EmployeeProject
                    {
                        EmployeeId = eid,
                        ProjectId = project.ProjectId,
                        AssignedDate = DateTime.Now
                    });
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
