using Microsoft.AspNetCore.Mvc;
using Smart.MVC.Models;
using Smart.MVC.Services;

namespace SmartRetail.MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        // List
        public async Task<IActionResult> Index()
        {
            var products = await _service.GetProducts();
            return View(products);
        }

        // Create GET
        public IActionResult Create()
        {
            return View();
        }

        // Create POST
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _service.CreateProductWithImage(model.Name, model.Price, model.ImageFile);
            if (success)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to create product. Please try again.");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _service.GetProduct(id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            var success = await _service.UpdateProduct(product);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to update product.");
                return View(product);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteProduct(id);
            if (!success)
                return BadRequest();
            return RedirectToAction("Index");
        }
    }
}