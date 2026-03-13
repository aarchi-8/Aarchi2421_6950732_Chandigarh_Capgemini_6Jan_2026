using Authors___Books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class AuthorBooksController : Controller
{
    private readonly AppDBContext _context;

    public AuthorBooksController(AppDBContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var data = _context.AuthorBooks
            .Include(ab => ab.Author)
            .Include(ab => ab.Book)
            .ToList();

        return View(data);
    }

    public IActionResult Create()
    {
        ViewBag.Authors = new SelectList(_context.Authors, "AuthorId", "Name");
        ViewBag.Books = new SelectList(_context.Books, "BookId", "Title");

        return View();
    }

    [HttpPost]
    public IActionResult Create(AuthorBook authorBook)
    {
        _context.AuthorBooks.Add(authorBook);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}