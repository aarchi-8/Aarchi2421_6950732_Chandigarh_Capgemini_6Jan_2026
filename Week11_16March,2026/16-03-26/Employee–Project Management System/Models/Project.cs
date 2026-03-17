
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_Project_Management_System.Models
{
    [Table("Projects")]
    public class Project
    {
        public int ProjectId { get; set; }

        [Required]
        public string Title { get; set; }

        public List<EmployeeProject> EmployeeProjects { get; set; }
    }
}
