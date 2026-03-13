using Customer___products.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class OrdersController : Controller
{
    private readonly AppDBContext _context;

    public OrdersController(AppDBContext context)
    {
        _context = context;
    }

    public IActionResult Create()
    {
        ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name");
        ViewBag.Products = new SelectList(_context.Products, "Id", "Name");

        return View();
    }

    [HttpPost]
    public IActionResult Create(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();

        return RedirectToAction("Index", "Customers");
    }
}