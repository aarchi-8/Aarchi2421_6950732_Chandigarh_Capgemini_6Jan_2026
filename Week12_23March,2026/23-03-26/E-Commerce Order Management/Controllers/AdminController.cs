using E_Commerce_Order_Management.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Order_Management.Controllers
{
    
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
                .Select(g => new {
                    ProductId = g.Key,
                    Count = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            var pendingOrders = _context.ShippingDetails
                .Where(s => s.Status == "Pending")
                .ToList();

            ViewBag.TopProducts = topProducts;
            ViewBag.PendingOrders = pendingOrders;

            return View();
        }
    }
}
