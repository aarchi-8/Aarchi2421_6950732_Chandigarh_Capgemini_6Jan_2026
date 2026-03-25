using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Order_Management.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1, 100000)]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }  

        public Category? Category { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}