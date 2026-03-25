using E_Commerce_Order_Management_API.Models;
using E_Commerce_Order_Management_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Order_Management_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }


        [HttpPost]
        public IActionResult Create(ProductDTO dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price
            };

            return Ok(_service.Add(product));
        }


        [HttpGet("search")]
        public IActionResult Search(string name)
        {
            return Ok(_service.Search(name));
        }

        [HttpGet("paged")]
        public IActionResult GetPaged(int page = 1, int pageSize = 5)
        {
            return Ok(_service.GetPaged(page, pageSize));
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, ProductDTO dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price
            };

            var updated = _service.Update(id, product);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _service.Delete(id);

            if (!result)
                return NotFound();

            return Ok("Deleted");
        }
    }

}
