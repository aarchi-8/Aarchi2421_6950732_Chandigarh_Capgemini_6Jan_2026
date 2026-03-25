using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace E_Commerce_Order_Management.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
