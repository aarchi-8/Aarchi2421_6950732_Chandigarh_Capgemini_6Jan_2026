using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Models;
using StudentManagementSystem.Filters;

namespace StudentManagementSystem.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    public class ProductController : Controller
    {
        static List<Product> products = new List<Product>()
        {
            new Product{Id=1,Name="Laptop",Price=70000},
            new Product{Id=2,Name="Phone",Price=30000}
        };

        public IActionResult Index()
        {
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = products.FirstOrDefault(x => x.Id == id);

            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            products.Add(product);

            return RedirectToAction("Index");
        }
    }
}