using Employee_Project_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class EmployeesController : Controller
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var employees = _context.Employees.Include(e => e.Department).ToList();
        return View(employees);
    }

    public IActionResult EmployeeProjects(int id)
    {
        var projects = _context.EmployeeProjects
            .Include(ep => ep.Project)
            .Where(ep => ep.EmployeeId == id)
            .Select(ep => ep.Project)
            .ToList();

        return View(projects);
    }

    public IActionResult Create()
    {
        ViewBag.Departments = _context.Departments.ToList();
        ViewBag.Projects = _context.Projects.ToList();
        return View();
    }

    [HttpPost]
    public IActionResult Create(Employee employee, List<int> projectIds)
    {
        _context.Employees.Add(employee);
        _context.SaveChanges();

        if (projectIds != null)
        {
            foreach (var pid in projectIds)
            {
                _context.EmployeeProjects.Add(new EmployeeProject
                {
                    EmployeeId = employee.EmployeeId,
                    ProjectId = pid,
                    AssignedDate = DateTime.Now
                });
            }

            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}