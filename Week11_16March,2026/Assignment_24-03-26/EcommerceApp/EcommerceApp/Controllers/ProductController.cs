using Microsoft.AspNetCore.Mvc;
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

        if (categoryId.HasValue)
            products = products.Where(p => p.CategoryId == categoryId);

        ViewBag.Categories = _context.Categories.ToList();

        return View(products.ToList());
    }

    public IActionResult Create()
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // 🔥 FIX
        ViewBag.Categories = _context.Categories.ToList();
        return View(product);
    }

    public IActionResult Edit(int id)
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View(_context.Products.Find(id));
    }

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Update(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // 🔥 FIX
        ViewBag.Categories = _context.Categories.ToList();
        return View(product);
    }

    public IActionResult Delete(int id)
    {
        return View(_context.Products.Find(id));
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var product = _context.Products.Find(id);
        _context.Remove(product);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}