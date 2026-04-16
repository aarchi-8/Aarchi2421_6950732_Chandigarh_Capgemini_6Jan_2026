using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Smart.MVC.Models
{
    public class ProductCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Product Image")]
        public IFormFile ImageFile { get; set; }
    }
}
