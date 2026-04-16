using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Api.Models;

public class Profile
{
  public int Id { get; set; }

  [Required]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  public string LastName { get; set; } = string.Empty;

  public int UserId { get; set; }
  public User? User { get; set; }
}
