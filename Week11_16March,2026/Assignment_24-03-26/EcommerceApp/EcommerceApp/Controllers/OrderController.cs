using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class OrderController : Controller
{
    private readonly AppDbContext _context;

    public OrderController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var orders = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.ShippingDetail)
            .ToList();

        return View(orders);
    }

    public IActionResult Create()
    {
        ViewBag.Customers = _context.Customers.ToList();
        ViewBag.Products = _context.Products.ToList();
        return View();
    }

    [HttpPost]
    public IActionResult Create(int customerId, List<int> productIds, List<int> quantities, string address)
    {
        if (productIds == null || quantities == null || productIds.Count == 0)
        {
            // 🔥 FIX
            ViewBag.Customers = _context.Customers.ToList();
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        var order = new Order
        {
            CustomerId = customerId,
            OrderDate = DateTime.Now
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        for (int i = 0; i < productIds.Count; i++)
        {
            var item = new OrderItem
            {
                OrderId = order.Id,
                ProductId = productIds[i],
                Quantity = quantities[i]
            };
            _context.OrderItems.Add(item);
        }

        var shipping = new ShippingDetail
        {
            OrderId = order.Id,
            Address = address,
            Status = "Pending"
        };

        _context.ShippingDetails.Add(shipping);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}