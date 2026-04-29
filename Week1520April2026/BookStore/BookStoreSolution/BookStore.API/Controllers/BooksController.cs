using Asp.Versioning;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BookStore.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    public BooksController(IBookService bookService) => _bookService = bookService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<BookDto>>>> GetBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] int? categoryId = null)
    { return Ok(ApiResponse<PaginatedResult<BookDto>>.Ok(await _bookService.GetBooksAsync(page, pageSize, search, categoryId))); }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BookDto>>> GetBook(int id)
    { var b = await _bookService.GetBookByIdAsync(id); if (b == null) return NotFound(ApiResponse.Fail("Book not found.", 404)); return Ok(ApiResponse<BookDto>.Ok(b)); }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<BookDto>>> CreateBook([FromForm] BookCreateDto dto, IFormFile? imageFile, [FromServices] IBlobService blobService)
    {
        try
        {
            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                imageUrl = await blobService.UploadImageAsync(stream, imageFile.FileName);
                Console.WriteLine("===== IMAGE DEBUG START =====");
                Console.WriteLine($"FINAL URL: {imageUrl}");
                Console.WriteLine("===== IMAGE DEBUG END =====");
            }
            dto.ImageUrl = imageUrl;
            var b = await _bookService.CreateBookAsync(dto);
            return CreatedAtAction(nameof(GetBook), new { id = b.BookId }, ApiResponse<BookDto>.Ok(b, "Book created."));
        }
        catch (Exception ex)
        {
            // Optionally delete the uploaded image if creation fails
            if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                // blobService.DeleteImageAsync(dto.ImageUrl); // if implemented
            }
            return BadRequest(ApiResponse.Fail($"Error creating book: {ex.Message}", 400));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> UpdateBook(int id, [FromForm] BookUpdateDto dto, IFormFile? imageFile, [FromServices] IBlobService blobService)
    {
        try
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imageUrl = await blobService.UploadImageAsync(stream, imageFile.FileName);
                dto.ImageUrl = imageUrl;
            }
            if (!await _bookService.UpdateBookAsync(id, dto)) return NotFound(ApiResponse.Fail("Book not found.", 404));
            return Ok(ApiResponse.Ok("Book updated."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail($"Error updating book: {ex.Message}", 400));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> DeleteBook(int id)
    { if (!await _bookService.SoftDeleteBookAsync(id)) return NotFound(ApiResponse.Fail("Book not found.", 404)); return Ok(ApiResponse.Ok("Book deleted.")); }

    [HttpPost("{id}/upload-image")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> UploadImage(int id, IFormFile file, [FromServices] IBlobService blobService)
    {
        if (file == null || file.Length == 0) return BadRequest(ApiResponse.Fail("No file uploaded."));
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null) return NotFound(ApiResponse.Fail("Book not found.", 404));
        using var stream = file.OpenReadStream();
        var url = await blobService.UploadImageAsync(stream, file.FileName);
        await _bookService.UpdateBookAsync(id, new BookUpdateDto { Title = book.Title, ISBN = book.ISBN, Price = book.Price, Stock = book.Stock, ImageUrl = url, CategoryId = 0, AuthorId = 0, PublisherId = 0 });
        return Ok(ApiResponse.Ok($"Image uploaded: {url}"));
    }
}