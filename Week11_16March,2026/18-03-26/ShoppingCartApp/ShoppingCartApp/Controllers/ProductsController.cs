using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Models;
using System.Collections.Generic;

public class ProductsController : Controller
{
    public static List<Product> products = new List<Product>()
    {
        new Product { Id = 1, Name = "Laptop", Price = 80000, ImageUrl = "https://via.placeholder.com/300" },
        new Product { Id = 2, Name = "Phone", Price = 50000, ImageUrl = "https://via.placeholder.com/300" }
    };

    public IActionResult Index()
    {
        return View(products);
    }
}