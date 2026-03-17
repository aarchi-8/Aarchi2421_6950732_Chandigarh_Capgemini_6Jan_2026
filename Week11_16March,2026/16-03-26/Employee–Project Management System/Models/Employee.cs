using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Employee_Project_Management_System.Models
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public string Name { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public List<EmployeeProject> EmployeeProjects { get; set; }

    }
}
