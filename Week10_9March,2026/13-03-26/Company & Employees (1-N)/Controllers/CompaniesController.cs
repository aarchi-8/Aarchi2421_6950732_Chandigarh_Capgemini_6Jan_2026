using Company___Employees.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Company___Employees.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly AppDBContext _context;

        public CompaniesController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var companies = _context.Companies
                            .Include(c => c.Employees)
                            .ToList();

            return View(companies);
        }
        // GET: Create Company
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create Company
        [HttpPost]
        public IActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Companies.Add(company);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company);
        }
    }
}
