using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Api.Models;

public class Course
{
  public int Id { get; set; }

  [Required]
  public string Title { get; set; } = string.Empty;

  [Required]
  public string Description { get; set; } = string.Empty;

  public string Category { get; set; } = string.Empty;

  public int InstructorId { get; set; }
  public User? Instructor { get; set; }

  public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
  public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
