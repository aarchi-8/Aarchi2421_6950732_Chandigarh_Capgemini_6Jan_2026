using System.ComponentModel.DataAnnotations;

namespace SmartHealthCareSystem.Shared.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        // Navigation properties
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
