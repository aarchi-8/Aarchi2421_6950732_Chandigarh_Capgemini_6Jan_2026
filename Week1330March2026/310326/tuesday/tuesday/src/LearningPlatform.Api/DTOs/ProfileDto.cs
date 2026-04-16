using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Api.DTOs;

public class ProfileDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;
}

