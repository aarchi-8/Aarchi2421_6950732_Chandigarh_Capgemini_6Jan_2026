using BookStore.Web.Models;
using BookStore.Web.Services;
using BookStore.Shared;
using Microsoft.AspNetCore.Mvc;
namespace BookStore.Web.Controllers;

public class AdminController : Controller
{
    private readonly ApiService _api;
    public AdminController(ApiService api) => _api = api;
    private bool IsAdmin => HttpContext.Session.GetString("UserRole") == "Admin";

    public IActionResult Index() { if (!IsAdmin) return RedirectToAction("Login", "Auth"); return View(); }

    public async Task<IActionResult> Books(int page = 1, string? search = null)
    { if (!IsAdmin) return RedirectToAction("Login", "Auth"); var r = await _api.GetAsync<PaginatedResult<BookListItem>>($"/api/v1/books?page={page}&pageSize=20&search={search}"); ViewBag.Search = search; ViewBag.Page = page; return View(r); }

    public async Task<IActionResult> CreateBook() { if (!IsAdmin) return RedirectToAction("Login", "Auth"); await LoadDropdowns(); return View(new BookFormModel()); }
    [HttpPost]
    public async Task<IActionResult> CreateBook(BookFormModel model)
    { if (!IsAdmin) return RedirectToAction("Login", "Auth"); var r = await _api.PostBookWithImageAsync("/api/v1/books", model); if (r.Success) { TempData["Message"] = "Book created."; return RedirectToAction("Books"); } TempData["Error"] = r.Message; await LoadDropdowns(); return View(model); }

    public async Task<IActionResult> EditBook(int id)
    { if (!IsAdmin) return RedirectToAction("Login", "Auth"); var b = await _api.GetAsync<BookListItem>($"/api/v1/books/{id}"); if (b == null) return NotFound(); await LoadDropdowns(); ViewBag.BookId = id; return View(new BookFormModel { Title = b.Title, ISBN = b.ISBN, Price = b.Price, Stock = b.Stock, ImageUrl = b.ImageUrl }); }
    [HttpPost]
    public async Task<IActionResult> EditBook(int id, BookFormModel model)
    { if (!IsAdmin) return RedirectToAction("Login", "Auth"); var r = await _api.PutBookWithImageAsync($"/api/v1/books/{id}", model); if (r.Success) { TempData["Message"] = "Book updated."; return RedirectToAction("Books"); } TempData["Error"] = r.Message; await LoadDropdowns(); ViewBag.BookId = id; return View(model); }

    [HttpPost]
    public async Task<IActionResult> DeleteBook(int id)
    { if (!IsAdmin) return RedirectToAction("Login", "Auth"); await _api.DeleteAsync($"/api/v1/books/{id}"); TempData["Message"] = "Book deleted."; return RedirectToAction("Books"); }

    public async Task<IActionResult> Orders()
    { if (!IsAdmin) return RedirectToAction("Login", "Auth"); var o = await _api.GetAsync<List<OrderView>>("/api/v1/orders/all"); return View(o ?? new()); }
    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
    { if (!IsAdmin) return RedirectToAction("Login", "Auth"); await _api.PatchAsync($"/api/v1/orders/{orderId}/status", status); TempData["Message"] = $"Order #{orderId} marked as {status}."; return RedirectToAction("Orders"); }

    public async Task<IActionResult> Reports()
    { if (!IsAdmin) return RedirectToAction("Login", "Auth"); var r = await _api.GetAsync<SalesReportView>("/api/v1/reports/sales"); return View(r ?? new()); }

    public async Task<IActionResult> Categories()
    { if (!IsAdmin) return RedirectToAction("Login", "Auth"); var c = await _api.GetAsync<List<CategoryView>>("/api/v1/categories"); return View(c ?? new()); }
    [HttpPost]
    public async Task<IActionResult> CreateCategory(string name)
    { if (!IsAdmin) return RedirectToAction("Login", "Auth"); await _api.PostAsync("/api/v1/categories", new { CategoryId = 0, Name = name }); TempData["Message"] = "Category created."; return RedirectToAction("Categories"); }
    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    { if (!IsAdmin) return RedirectToAction("Login", "Auth"); var r = await _api.DeleteAsync($"/api/v1/categories/{id}"); if (r.Success) TempData["Message"] = "Category deleted."; else TempData["Error"] = r.Message; return RedirectToAction("Categories"); }

    private async Task LoadDropdowns()
    { ViewBag.Categories = await _api.GetAsync<List<CategoryView>>("/api/v1/categories") ?? new(); ViewBag.Authors = await _api.GetAsync<List<AuthorView>>("/api/v1/categories/authors") ?? new(); ViewBag.Publishers = await _api.GetAsync<List<PublisherView>>("/api/v1/categories/publishers") ?? new(); }
}