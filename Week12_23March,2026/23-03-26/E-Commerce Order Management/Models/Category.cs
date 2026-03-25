using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Order_Management.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
