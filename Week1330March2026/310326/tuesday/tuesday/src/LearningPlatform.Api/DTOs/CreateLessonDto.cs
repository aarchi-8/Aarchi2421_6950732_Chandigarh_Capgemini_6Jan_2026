using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Api.DTOs;

public class CreateLessonDto
{
  [Required]
  public string Title { get; set; } = string.Empty;

  [Required]
  public string Content { get; set; } = string.Empty;
}
