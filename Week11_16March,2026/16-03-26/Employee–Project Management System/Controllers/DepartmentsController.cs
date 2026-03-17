using Employee_Project_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class DepartmentsController : Controller
{
    private readonly AppDbContext _context;

    public DepartmentsController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var departments = _context.Departments.ToList();
        return View(departments);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Department department)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
        }

        _context.Departments.Add(department);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult DepartmentEmployeeCount()
    {
        var result = _context.Departments
            .Select(d => new
            {
                DepartmentName = d.Name,
                EmployeeCount = d.Employees.Count()
            }).ToList();

        return View(result);
    }
}