using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Api.Models;

public class User
{
  public int Id { get; set; }

  [Required]
  public string Username { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string PasswordHash { get; set; } = string.Empty;

  [Required]
  public Role Role { get; set; }

  public Profile? Profile { get; set; }
  public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
  public ICollection<Course> Courses { get; set; } = new List<Course>();
}
