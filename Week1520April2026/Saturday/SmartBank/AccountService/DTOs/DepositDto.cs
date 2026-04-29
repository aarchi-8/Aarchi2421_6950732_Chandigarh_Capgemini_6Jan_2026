using System.ComponentModel.DataAnnotations;

namespace AccountService.DTOs
{
  public class DepositDto
  {
    [Required]
    [StringLength(12)]
    public string AccountNo { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(250)]
    public string Description { get; set; } = string.Empty;
  }
}
