using System.ComponentModel.DataAnnotations;
using LearningPlatform.Api.Models;

namespace LearningPlatform.Api.DTOs;

public class UserRegisterDto
{
  [Required]
  public string Username { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [MinLength(6)]
  public string Password { get; set; } = string.Empty;

  [Required]
  public Role Role { get; set; }

  public ProfileDto? Profile { get; set; }
}
