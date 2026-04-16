using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthCareSystem.Shared.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [StringLength(100)]
        public string? Specialization { get; set; }

        [Range(0, 50)]
        public int? ExperienceYears { get; set; }

        [StringLength(100)]
        public string? Availability { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
