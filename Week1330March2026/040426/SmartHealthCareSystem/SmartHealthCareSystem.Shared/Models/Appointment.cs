using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthCareSystem.Shared.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [RegularExpression("Booked|Completed|Cancelled")]
        public string Status { get; set; } = "Booked";

        // Navigation properties
        [ForeignKey("PatientId")]
        public User? Patient { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        public Prescription? Prescription { get; set; }
        public Bill? Bill { get; set; }
    }
}
