using System.Net.Http.Headers;
using System.Text.Json;
using BookStore.Shared;
using BookStore.Web.Models;
using BookStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers;

public class BooksController : Controller
{
    private readonly ApiService _api;
    private static readonly JsonSerializerOptions J = new() { PropertyNameCaseInsensitive = true };

    public BooksController(ApiService api) => _api = api;

    // ✅ FIXED INDEX
    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        var result = await _api.GetAsync<PaginatedResult<BookListItem>>(
            $"/api/v1/books?page={page}&pageSize=12&search={search}"
        );

        ViewBag.Search = search;
        ViewBag.Page = page;
        return View(result ?? new PaginatedResult<BookListItem> { Items = new List<BookListItem>() });
    }

    // ✅ FIXED DETAILS
    public async Task<IActionResult> Details(int id)
    {
        var book = await _api.GetAsync<BookListItem>($"/api/v1/books/{id}");

        if (book == null) return NotFound();

        return View(book);
    }

    [HttpPost]
    public IActionResult AddToCart(int bookId, string title, decimal price)
    {
        if (HttpContext.Session.GetString("UserRole") == "Admin")
        {
            TempData["Error"] = "Admin cannot add items to cart.";
            return RedirectToAction("Index");
        }

        var cart = GetCart();
        var ex = cart.Find(c => c.BookId == bookId);

        if (ex != null) ex.Qty++;
        else cart.Add(new CartItem { BookId = bookId, Title = title, Price = price, Qty = 1 });

        SaveCart(cart);
        return RedirectToAction("Cart");
    }

    public IActionResult Cart()
    {
        if (HttpContext.Session.GetString("UserRole") == "Admin")
        {
            TempData["Error"] = "Admin cannot place orders.";
            return RedirectToAction("Index", "Admin");
        }

        return View(GetCart());
    }

    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        if (HttpContext.Session.GetString("UserRole") == "Admin")
            return RedirectToAction("Index", "Admin");

        var cart = GetCart();

        if (cart.Count == 0)
            return RedirectToAction("Cart");

        var r = await _api.PostAsync("/api/v1/orders",
            new { Items = cart.Select(c => new { c.BookId, c.Qty }).ToList() });

        if (r.Success)
        {
            SaveCart(new List<CartItem>());
            TempData["Message"] = "Order placed successfully!";
            return RedirectToAction("Index", "Orders");
        }

        TempData["Error"] = r.Message;
        return RedirectToAction("Cart");
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int bookId)
    {
        var cart = GetCart();
        cart.RemoveAll(c => c.BookId == bookId);
        SaveCart(cart);
        return RedirectToAction("Cart");
    }

    private List<CartItem> GetCart()
    {
        var j = HttpContext.Session.GetString("Cart");
        return string.IsNullOrEmpty(j)
            ? new()
            : JsonSerializer.Deserialize<List<CartItem>>(j, J) ?? new();
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
    }
}