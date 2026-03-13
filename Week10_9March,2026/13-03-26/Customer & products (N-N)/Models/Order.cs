using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Customer___products.Models
{
    [Table("Orders")]
    public class Order
    {
        [Required]
        public int Id { get; set; } 
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }    
    }
}
