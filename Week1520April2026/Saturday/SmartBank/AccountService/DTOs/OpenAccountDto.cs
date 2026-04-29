using System.ComponentModel.DataAnnotations;

namespace AccountService.DTOs
{
  public class OpenAccountDto
  {
    [Required]
    public int CustomerId { get; set; }

    [Required]
    [RegularExpression("^(Savings|Current)$")]
    public string AccountType { get; set; } = string.Empty;
  }
}
