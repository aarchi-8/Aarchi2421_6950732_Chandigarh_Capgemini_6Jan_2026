using E_Commerce_Order_Management.Data;
using E_Commerce_Order_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int? categoryId)
    {
        var products = _context.Products.Include(p => p.Category).AsQueryable();

        if (categoryId != null)
            products = products.Where(p => p.CategoryId == categoryId);

        return View(products.ToList());
    }

    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
        return View(product);
    }
    [HttpPost]
    public IActionResult BuyNow(int productId)
    {
        // 🔥 TEMP: pick first customer (or you can select manually)
        var customer = _context.Customers.FirstOrDefault();

        if (customer == null)
        {
            return Content("No customer found. Please add customer first.");
        }

        var order = new Order
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.Now
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        // Add product to order
        var orderItem = new OrderItem
        {
            OrderId = order.Id,
            ProductId = productId,
            Quantity = 1
        };

        _context.OrderItems.Add(orderItem);

        // Add shipping
        var shipping = new ShippingDetail
        {
            OrderId = order.Id,
            Status = "Pending"
        };

        _context.ShippingDetails.Add(shipping);

        _context.SaveChanges();

        return RedirectToAction("Index", "Order");
    }
}