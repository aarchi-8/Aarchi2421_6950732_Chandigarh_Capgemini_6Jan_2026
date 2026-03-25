using E_Commerce_Order_Management.Data;
using E_Commerce_Order_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class OrderController : Controller
{
    private readonly AppDbContext _context;

    public OrderController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Orders
    public IActionResult Index()
    {
        var orders = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.ShippingDetail)
            .ToList();

        return View(orders);
    }

    // GET: Create Order
    public IActionResult Create()
    {
        ViewBag.Customers = _context.Customers.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View();
    }
    [HttpPost]
    public IActionResult Create(int CustomerId, List<int> productIds)
    {
        if (productIds == null || !productIds.Any())
        {
            return Content("Please select at least one product");
        }

        var order = new Order
        {
            CustomerId = CustomerId,
            OrderDate = DateTime.Now
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        var shipping = new ShippingDetail
        {
            OrderId = order.Id,
            Status = "Pending"   // 🔥 default
        };

        _context.ShippingDetails.Add(shipping);

        foreach (var pid in productIds)
        {
            _context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                ProductId = pid,
                Quantity = 1
            });
        }


        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult UpdateStatus(int orderId, string status)
    {
        var shipping = _context.ShippingDetails
            .FirstOrDefault(s => s.OrderId == orderId);

        if (shipping != null)
        {
            shipping.Status = status;
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}