using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetail.API.Data;
using SmartRetail.API.Models;

namespace SmartRetail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly BlobService _blobService;

        public ProductController(AppDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] string name, [FromForm] decimal price, IFormFile file)
        {
            var imageUrl = await _blobService.UploadFileAsync(file);

            var product = new Product
            {
                Name = name,
                Price = price,
                ImageUrl = imageUrl
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Products.ToList());
        }
    }

}