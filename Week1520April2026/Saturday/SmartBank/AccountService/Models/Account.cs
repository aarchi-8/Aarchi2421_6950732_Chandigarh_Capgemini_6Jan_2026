using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountService.Models
{
  [Table("Accounts")]
  public class Account
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AccountId { get; set; }

    [Required]
    [StringLength(12)]
    public string AccountNo { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Savings|Current)$")]
    public string AccountType { get; set; } = string.Empty;

    // CustomerId comes from CustomerService
    [Required]
    public int CustomerId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }

    [Required]
    [RegularExpression("^(Active|Closed)$")]
    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; }
  }
}
