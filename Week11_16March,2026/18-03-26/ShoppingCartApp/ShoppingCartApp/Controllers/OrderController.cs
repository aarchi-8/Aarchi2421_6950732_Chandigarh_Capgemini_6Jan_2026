using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Models;

public class OrderController : Controller
{
    public IActionResult Checkout()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Checkout(Order order)
    {
        TempData["success"] = "Order Placed Successfully!";
        CartController.cart.Clear();
        return RedirectToAction("Confirmation");
    }

    public IActionResult Confirmation()
    {
        return View();
    }
}