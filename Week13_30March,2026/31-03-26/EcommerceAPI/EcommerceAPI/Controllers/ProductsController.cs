using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/v1/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger  = logger;
    }

    // ─── PUBLIC: Get all products ─────────────────────────────────────────────
    /// <summary>Returns all products. Supports optional search by name.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Category.Contains(search));

        var products = await query.ToListAsync();
        return Ok(products);
    }

    // ─── PUBLIC: Get by ID (int constraint avoids route conflict) ─────────────
    /// <summary>Returns a single product by its integer ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product {Id} not found.", id);
            return NotFound(new { message = $"Product with ID {id} not found." });
        }
        return Ok(product);
    }

    // ─── PUBLIC: Filter by category ───────────────────────────────────────────
    /// <summary>Returns all products belonging to a specific category.</summary>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var products = await _context.Products
            .Where(p => p.Category.ToLower() == category.ToLower())
            .ToListAsync();

        return Ok(products);
    }

    // ─── ADMIN: Create product ────────────────────────────────────────────────
    /// <summary>Creates a new product. Requires Admin role.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        product.CreatedAt = DateTime.UtcNow;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product created: {Name} (ID: {Id})", product.Name, product.Id);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // ─── ADMIN: Update product ────────────────────────────────────────────────
    /// <summary>Updates an existing product by ID. Requires Admin role.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(int id, [FromBody] Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _context.Products.FindAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Product with ID {id} not found." });

        existing.Name        = product.Name;
        existing.Price       = product.Price;
        existing.Category    = product.Category;
        existing.Description = product.Description;
        existing.UpdatedAt   = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Product updated: ID {Id}", id);
        return NoContent();
    }

    // ─── ADMIN: Delete product ────────────────────────────────────────────────
    /// <summary>Deletes a product by ID. Requires Admin role.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound(new { message = $"Product with ID {id} not found." });

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product deleted: ID {Id}", id);
        return NoContent();
    }
}