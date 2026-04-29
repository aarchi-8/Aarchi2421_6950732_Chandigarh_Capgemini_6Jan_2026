using Asp.Versioning;
using BookStore.Application.DTOs;
using BookStore.Infrastructure.Data;
using BookStore.Domain.Entities;
using BookStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BookStore.API.Controllers;
[ApiVersion("1.0")] [Route("api/v{version:apiVersion}/[controller]")] [ApiController]
public class CategoriesController : ControllerBase
{
    private readonly BookStoreDbContext _context;
    public CategoriesController(BookStoreDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAll()
    { var cats = await _context.Categories.Select(c => new CategoryDto { CategoryId = c.CategoryId, Name = c.Name }).ToListAsync(); return Ok(ApiResponse<List<CategoryDto>>.Ok(cats)); }

    [HttpPost] [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> Create(CategoryDto dto)
    { _context.Categories.Add(new Category { Name = dto.Name }); await _context.SaveChangesAsync(); return Ok(ApiResponse.Ok("Category created.")); }

    [HttpDelete("{id}")] [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        var cat = await _context.Categories.Include(c => c.Books).FirstOrDefaultAsync(c => c.CategoryId == id);
        if (cat == null) return NotFound(ApiResponse.Fail("Category not found.", 404));
        if (cat.Books.Any(b => !b.IsDeleted)) return BadRequest(ApiResponse.Fail("Cannot delete category with active books."));
        _context.Categories.Remove(cat); await _context.SaveChangesAsync(); return Ok(ApiResponse.Ok("Category deleted."));
    }

    [HttpGet("authors")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetAuthors()
    { var a = await _context.Authors.Select(a => new { a.AuthorId, a.Name }).ToListAsync(); return Ok(ApiResponse<List<object>>.Ok(a.Cast<object>().ToList())); }

    [HttpGet("publishers")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetPublishers()
    { var p = await _context.Publishers.Select(p => new { p.PublisherId, p.Name }).ToListAsync(); return Ok(ApiResponse<List<object>>.Ok(p.Cast<object>().ToList())); }
}