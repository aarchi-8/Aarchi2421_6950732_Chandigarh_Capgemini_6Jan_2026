using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Api.Models;

public class Lesson
{
  public int Id { get; set; }

  [Required]
  public string Title { get; set; } = string.Empty;

  [Required]
  public string Content { get; set; } = string.Empty;

  public int CourseId { get; set; }
  public Course? Course { get; set; }
}
