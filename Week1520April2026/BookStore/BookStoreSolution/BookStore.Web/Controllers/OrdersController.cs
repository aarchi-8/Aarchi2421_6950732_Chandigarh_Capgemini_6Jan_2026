using BookStore.Web.Models;
using BookStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
namespace BookStore.Web.Controllers;
public class OrdersController : Controller
{
    private readonly ApiService _api;
    public OrdersController(ApiService api) => _api = api;
    public async Task<IActionResult> Index() { var o = await _api.GetAsync<List<OrderView>>("/api/v1/orders/my"); return View(o ?? new()); }
}