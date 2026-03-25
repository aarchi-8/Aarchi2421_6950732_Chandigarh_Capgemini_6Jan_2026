using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CustomerController : Controller
{
    private readonly AppDbContext _context;

    public CustomerController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Customers.ToList());
    }

    public IActionResult Details(int id)
    {
        var customer = _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefault(c => c.Id == id);

        return View(customer);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            _context.Add(customer);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(customer);
    }

    public IActionResult Edit(int id)
    {
        var customer = _context.Customers.Find(id);
        return View(customer);
    }

    [HttpPost]
    public IActionResult Edit(Customer customer)
    {
        if (ModelState.IsValid)
        {
            _context.Update(customer);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(customer);
    }

    public IActionResult Delete(int id)
    {
        var customer = _context.Customers.Find(id);
        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var customer = _context.Customers.Find(id);
        _context.Customers.Remove(customer);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}