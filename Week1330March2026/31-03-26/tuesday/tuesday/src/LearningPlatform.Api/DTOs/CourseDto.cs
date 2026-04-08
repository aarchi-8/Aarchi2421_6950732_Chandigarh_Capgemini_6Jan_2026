namespace LearningPlatform.Api.DTOs;

public class CourseDto
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Category { get; set; } = string.Empty;
  public string InstructorUsername { get; set; } = string.Empty;
  public List<LessonDto> Lessons { get; set; } = new();
}
