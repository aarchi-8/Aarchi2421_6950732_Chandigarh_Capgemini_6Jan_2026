using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;

[Authorize]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        var topProducts = _context.OrderItems
            .GroupBy(o => o.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalSold = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(5)
            .ToList();

        var pendingOrders = _context.ShippingDetails
            .Where(s => s.Status == "Pending")
            .ToList();

        var customers = _context.Customers
            .Include(c => c.Orders)
            .ToList();

        ViewBag.TopProducts = topProducts;
        ViewBag.PendingOrders = pendingOrders;
        ViewBag.Customers = customers;

        return View();
    }

    public IActionResult UpdateShipping(int id)
    {
        var shipping = _context.ShippingDetails.Find(id);
        shipping.Status = "Shipped";
        _context.SaveChanges();

        return RedirectToAction("Dashboard");
    }
}