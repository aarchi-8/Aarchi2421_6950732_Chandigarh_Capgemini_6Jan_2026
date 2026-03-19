using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Models;
using System.Collections.Generic;
using System.Linq;

public class CartController : Controller
{
    public static List<CartItem> cart = new List<CartItem>();

    public IActionResult Index()
    {
        return View(cart);
    }

    public IActionResult Add(int id)
    {
        var product = ProductsController.products.FirstOrDefault(p => p.Id == id);

        var existing = cart.FirstOrDefault(c => c.ProductId == id);

        if (existing != null)
            existing.Quantity++;
        else
            cart.Add(new CartItem { ProductId = id, Product = product, Quantity = 1 });

        return RedirectToAction("Index");
    }

    public IActionResult Remove(int id)
    {
        var item = cart.FirstOrDefault(c => c.ProductId == id);
        if (item != null)
            cart.Remove(item);

        return RedirectToAction("Index");
    }
}