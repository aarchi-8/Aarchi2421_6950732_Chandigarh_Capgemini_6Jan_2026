using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Api.DTOs;

public class EnrollDto
{
    [Required]
    public int CourseId { get; set; }
}
