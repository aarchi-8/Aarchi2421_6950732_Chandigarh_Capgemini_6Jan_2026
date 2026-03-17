using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }
    }
}