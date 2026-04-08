using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartHealthcare.Models.Enums;

namespace SmartHealthcare.Models.Entities
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        public int UserId { get; set; }  // FK to User (1-to-1)

        [Required, MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        [MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        public string? MedicalHistory { get; set; }

        // Navigation
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}