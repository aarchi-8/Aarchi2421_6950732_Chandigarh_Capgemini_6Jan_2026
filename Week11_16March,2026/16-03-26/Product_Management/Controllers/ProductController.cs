using Microsoft.AspNetCore.Mvc;
using ProductManagementApp.Models;
using ProductManagementApp.Filters;
using System.Collections.Generic;

namespace ProductManagementApp.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            // Simulating an error to test exception filter
            throw new System.Exception("Test exception from Index action");

            var products = new List<Product>
            {
                new Product{Id=1, Name="Laptop", Price=80000},
                new Product{Id=2, Name="Phone", Price=50000},
                new Product{Id=3, Name="Headphones", Price=5000}
            };

            return View(products);
        }
    }
}