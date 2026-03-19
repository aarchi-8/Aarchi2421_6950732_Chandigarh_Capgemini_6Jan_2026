using Microsoft.AspNetCore.Mvc;
using LibraryApp.Models;
using LibraryApp.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace LibraryApp.Controllers
{
    public class BookController : Controller
    {
        // Temporary in-memory list
        private static List<Book> books = new List<Book>()
        {
            new Book { Id = 1, Title = "The Alchemist", Author = "Paulo Coelho", PublishedYear = 1988, Genre = "Fiction" },
            new Book { Id = 2, Title = "Clean Code", Author = "Robert C. Martin", PublishedYear = 2008, Genre = "Programming" }
        };

        // INDEX
        public IActionResult Index()
        {
            var bookViewModels = books.Select(b => new BookViewModel
            {
                Book = b,
                IsAvailable = true,
                BorrowerName = ""
            }).ToList();

            ViewBag.Message = "Welcome to the Library!";
            ViewData["TotalBooks"] = books.Count;

            return View(bookViewModels);
        }

        // GET CREATE
        public IActionResult Create()
        {
            return View();
        }

        // POST CREATE
        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                book.Id = books.Count + 1;
                books.Add(book);
                return RedirectToAction("Index");
            }
            return View(book);
        }
    }
}