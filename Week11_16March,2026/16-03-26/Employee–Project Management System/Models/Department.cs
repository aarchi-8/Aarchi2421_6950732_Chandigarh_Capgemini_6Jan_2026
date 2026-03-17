using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Employee_Project_Management_System.Models
{
    [Table("Departments")]
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
