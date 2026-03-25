using System.ComponentModel.DataAnnotations;

namespace SecureEmployeeManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Department { get; set; }

        [Range(1,double.MaxValue,ErrorMessage = "Salary must be greater than 0.")]
        public double Salary { get; set; }
    }
}
