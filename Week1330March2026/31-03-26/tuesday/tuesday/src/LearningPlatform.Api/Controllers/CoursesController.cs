using LearningPlatform.Api.Data;
using LearningPlatform.Api.DTOs;
using LearningPlatform.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LearningPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/courses")]
public class CoursesController : ControllerBase
{
  private readonly AppDbContext _context;
  private readonly IMemoryCache _cache;

  public CoursesController(AppDbContext context, IMemoryCache cache)
  {
    _context = context;
    _cache = cache;
  }

  // GET: api/v1/courses
  [HttpGet]
  [Authorize]
  public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
  {
    const string cacheKey = "courses";

    if (!_cache.TryGetValue(cacheKey, out IEnumerable<CourseDto>? courses))
    {
      courses = await _context.Courses
          .Include(c => c.Lessons)
          .Select(c => new CourseDto
          {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Category = c.Category,
            InstructorUsername = c.Instructor != null ? c.Instructor.Username : string.Empty,
            Lessons = c.Lessons.Select(l => new LessonDto
            {
              Id = l.Id,
              Title = l.Title,
              Content = l.Content
            }).ToList()
          })
          .ToListAsync();

      var cacheOptions = new MemoryCacheEntryOptions()
          .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

      _cache.Set(cacheKey, courses, cacheOptions);
    }

    return Ok(courses);
  }

  // GET: api/v1/courses/{id}
  [HttpGet("{id}")]
  [Authorize]
  public async Task<ActionResult<CourseDto>> GetCourse(int id)
  {
    var course = await _context.Courses
        .Include(c => c.Lessons)
        .Include(c => c.Instructor)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (course == null)
      return NotFound();

    var courseDto = new CourseDto
    {
      Id = course.Id,
      Title = course.Title,
      Description = course.Description,
      Category = course.Category,
      InstructorUsername = course.Instructor?.Username ?? string.Empty,
      Lessons = course.Lessons.Select(l => new LessonDto
      {
        Id = l.Id,
        Title = l.Title,
        Content = l.Content
      }).ToList()
    };

    return Ok(courseDto);
  }

  // GET: api/v1/courses/category/{name}
  [HttpGet("category/{name}")]
  [Authorize]
  public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByCategory(string name)
  {
    var courses = await _context.Courses
        .Include(c => c.Lessons)
        .Where(c => c.Category == name)
        .Select(c => new CourseDto
        {
          Id = c.Id,
          Title = c.Title,
          Description = c.Description,
          Category = c.Category,
          InstructorUsername = c.Instructor != null ? c.Instructor.Username : string.Empty,
          Lessons = c.Lessons.Select(l => new LessonDto
          {
            Id = l.Id,
            Title = l.Title,
            Content = l.Content
          }).ToList()
        })
        .ToListAsync();

    return Ok(courses);
  }

  // POST: api/v1/courses
  [HttpPost]
  [Authorize(Roles = "Instructor,Admin")]
  public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    // Get current user ID from JWT token
    var userIdClaim = User.FindFirst("id");
    if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
      return Unauthorized();

    var course = new Course
    {
      Title = dto.Title,
      Description = dto.Description,
      Category = dto.Category,
      InstructorId = userId
    };

    _context.Courses.Add(course);
    await _context.SaveChangesAsync();

    // Clear cache
    _cache.Remove("courses");

    var courseDto = new CourseDto
    {
      Id = course.Id,
      Title = course.Title,
      Description = course.Description,
      Category = course.Category,
      InstructorUsername = User.Identity?.Name ?? string.Empty,
      Lessons = new List<LessonDto>()
    };

    return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, courseDto);
  }

  // POST: api/v1/courses/{id}/lessons
  [HttpPost("{id}/lessons")]
  [Authorize(Roles = "Instructor,Admin")]
  public async Task<ActionResult<LessonDto>> AddLesson(int id, CreateLessonDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var course = await _context.Courses.FindAsync(id);
    if (course == null)
      return NotFound();

    // Check if user is the instructor of this course
    var userIdClaim = User.FindFirst("id");
    if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId) || course.InstructorId != userId)
      return Forbid();

    var lesson = new Lesson
    {
      Title = dto.Title,
      Content = dto.Content,
      CourseId = id
    };

    _context.Lessons.Add(lesson);
    await _context.SaveChangesAsync();

    // Clear cache
    _cache.Remove("courses");

    var lessonDto = new LessonDto
    {
      Id = lesson.Id,
      Title = lesson.Title,
      Content = lesson.Content
    };

    return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, lessonDto);
  }
}