using Authors___Books.Models;
using Microsoft.AspNetCore.Mvc;

public class AuthorsController : Controller
{
    private readonly AppDBContext _context;

    public AuthorsController(AppDBContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Authors.ToList());
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Author author)
    {
        if (ModelState.IsValid)
        {
            _context.Authors.Add(author);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        return View(author);
    }
    public IActionResult Edit(int id)
    {
        var author = _context.Authors.Find(id);
        return View(author);
    }

    [HttpPost]
    public IActionResult Edit(Author author)
    {
        _context.Authors.Update(author);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var author = _context.Authors.Find(id);
        return View(author);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        var author = _context.Authors.Find(id);
        _context.Authors.Remove(author);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}