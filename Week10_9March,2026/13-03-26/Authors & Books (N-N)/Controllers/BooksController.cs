using Authors___Books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class BooksController : Controller
{
    private readonly AppDBContext _context;

    public BooksController(AppDBContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var books = _context.Books
            .Include(b => b.AuthorBooks)
            .ThenInclude(ab => ab.Author)
            .ToList();

        return View(books);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Book book)
    {
        if (ModelState.IsValid)
        {
            _context.Books.Add(book);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        return View(book);
    }

    public IActionResult Edit(int id)
    {
        var book = _context.Books.Find(id);
        return View(book);
    }

    [HttpPost]
    public IActionResult Edit(Book book)
    {
        _context.Books.Update(book);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var book = _context.Books.Find(id);
        return View(book);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        var book = _context.Books.Find(id);
        _context.Books.Remove(book);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}