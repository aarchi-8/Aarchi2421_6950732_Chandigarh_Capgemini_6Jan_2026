using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthCareSystem.Shared.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [StringLength(255)]
        public string? Diagnosis { get; set; }

        public string? Medicines { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        // Navigation property
        [ForeignKey("AppointmentId")]
        public Appointment? Appointment { get; set; }
    }
}
