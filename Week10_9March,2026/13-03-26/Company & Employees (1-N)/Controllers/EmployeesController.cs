using Company___Employees.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class EmployeesController : Controller
{
    private readonly AppDBContext _context;

    public EmployeesController(AppDBContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var employees = _context.Employees
                        .Include(e => e.Company)
                        .ToList();

        return View(employees);
    }
    public IActionResult Create()
    {
        ViewBag.Companies = new SelectList(_context.Companies, "Id", "Name");
        return View();
    }

    [HttpPost]
    public IActionResult Create(Employee employee)
    {
        if (ModelState.IsValid)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
            return RedirectToAction("Index", "Companies");
        }

        ViewBag.Companies = new SelectList(_context.Companies, "Id", "Name");
        return View(employee);
    }
}