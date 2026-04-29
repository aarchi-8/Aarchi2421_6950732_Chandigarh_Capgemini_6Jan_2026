using System.Security.Claims;
using Asp.Versioning;
using BookStore.Application.DTOs;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using BookStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BookStore.API.Controllers;
[ApiVersion("1.0")] [Route("api/v{version:apiVersion}/[controller]")] [ApiController]
public class ReviewsController : ControllerBase
{
    private readonly BookStoreDbContext _context;
    public ReviewsController(BookStoreDbContext context) => _context = context;
    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<ApiResponse<List<ReviewDto>>>> GetBookReviews(int bookId)
    { var r = await _context.Reviews.Include(r => r.User).Include(r => r.Book).Where(r => r.BookId == bookId).Select(r => new ReviewDto { ReviewId = r.ReviewId, BookId = r.BookId, BookTitle = r.Book.Title, Rating = r.Rating, Comment = r.Comment, UserName = r.User.FullName }).ToListAsync(); return Ok(ApiResponse<List<ReviewDto>>.Ok(r)); }
    [HttpPost] [Authorize]
    public async Task<ActionResult<ApiResponse>> Create(ReviewCreateDto dto)
    { var uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); _context.Reviews.Add(new Review { UserId = uid, BookId = dto.BookId, Rating = dto.Rating, Comment = dto.Comment }); await _context.SaveChangesAsync(); return Ok(ApiResponse.Ok("Review added.")); }
}