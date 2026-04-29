using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountService.Models
{
  [Table("Transactions")]
  public class Transaction
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TransactionId { get; set; }

    [Required]
    [StringLength(12)]
    public string AccountNo { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Credit|Debit)$")]
    public string Type { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(250)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
  }
}
