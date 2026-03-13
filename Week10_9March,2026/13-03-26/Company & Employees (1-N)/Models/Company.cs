using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Company___Employees.Models
{
    [Table("Companies")]
    public class Company
    {
        [Key]
        
        //primary key
        public int Id { get; set; } 
        [Required]
        public string Name { get; set; }
        public string Location { get; set; }

        //one company -> many employees
        public ICollection<Employee> Employees { get; set; }
    }
}
