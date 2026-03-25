using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Order_Management_API.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Range(1, 100000)]
        public decimal Price { get; set; }

        public string? Category { get; set; }
    }
}
