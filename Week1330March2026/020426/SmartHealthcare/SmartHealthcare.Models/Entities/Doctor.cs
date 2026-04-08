using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthcare.Models.Entities
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        public int UserId { get; set; }  // FK to User (1-to-1)

        [Required, MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Qualification { get; set; } = string.Empty;

        public int ExperienceYears { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal ConsultationFee { get; set; }

        public bool IsAvailable { get; set; } = true;

        // Navigation
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        // Many-to-Many
        public ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
    }
}