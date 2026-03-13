using Customer___products.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CustomersController : Controller
{
    private readonly AppDBContext _context;

    public CustomersController(AppDBContext context)
    {
        _context = context;
    }

    // READ
    public IActionResult Index()
    {
        var customers = _context.Customers
            .Include(c => c.Orders)
            .ThenInclude(o => o.Product)
            .ToList();

        return View(customers);
    }

    // CREATE
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            _context.Customers.Add(customer);   // add here
            _context.SaveChanges();             // save to database

            return RedirectToAction("Index");
        }

        return View(customer);
    }
    // EDIT
    public IActionResult Edit(int id)
    {
        var customer = _context.Customers.Find(id);
        return View(customer);
    }

    [HttpPost]
    public IActionResult Edit(Customer customer)
    {
        _context.Customers.Update(customer);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // DELETE
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