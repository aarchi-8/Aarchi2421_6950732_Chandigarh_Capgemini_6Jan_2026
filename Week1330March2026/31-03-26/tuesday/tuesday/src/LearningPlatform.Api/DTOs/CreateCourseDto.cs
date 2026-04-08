using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Api.DTOs;

public class CreateCourseDto
{
  [Required]
  public string Title { get; set; } = string.Empty;

  [Required]
  public string Description { get; set; } = string.Empty;

  public string Category { get; set; } = string.Empty;
}
