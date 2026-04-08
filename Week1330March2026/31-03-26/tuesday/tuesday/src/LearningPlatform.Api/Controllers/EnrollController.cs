using LearningPlatform.Api.Data;
using LearningPlatform.Api.DTOs;
using LearningPlatform.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/enroll")]
[Authorize(Roles = "Student")]
public class EnrollController : ControllerBase
{
  private readonly AppDbContext _context;

  public EnrollController(AppDbContext context)
  {
    _context = context;
  }

  // POST: api/v1/enroll
  [HttpPost]
  public async Task<IActionResult> Enroll(EnrollDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    // Get current user ID from JWT token
    var userIdClaim = User.FindFirst("id");
    if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
      return Unauthorized();

    // Check if course exists
    var course = await _context.Courses.FindAsync(dto.CourseId);
    if (course == null)
      return NotFound("Course not found");

    // Check if already enrolled
    var existingEnrollment = await _context.Enrollments
        .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == dto.CourseId);

    if (existingEnrollment != null)
      return BadRequest("Already enrolled in this course");

    var enrollment = new Enrollment
    {
      UserId = userId,
      CourseId = dto.CourseId
    };

    _context.Enrollments.Add(enrollment);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Successfully enrolled in course" });
  }
}