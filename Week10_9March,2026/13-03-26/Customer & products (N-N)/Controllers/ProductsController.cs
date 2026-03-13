using Customer___products.Models;
using Microsoft.AspNetCore.Mvc;

public class ProductsController : Controller
{
    private readonly AppDBContext _context;

    public ProductsController(AppDBContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Products.ToList());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}