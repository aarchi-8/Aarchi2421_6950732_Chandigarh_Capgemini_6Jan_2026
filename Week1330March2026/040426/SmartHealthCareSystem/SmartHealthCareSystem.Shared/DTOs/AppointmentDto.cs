using System.ComponentModel.DataAnnotations;

namespace SmartHealthCareSystem.Shared.DTOs
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
    }

    public class CreateAppointmentDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [StringLength(250)]
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateAppointmentDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [RegularExpression("Scheduled|Completed|Cancelled|Booked")]
        public string Status { get; set; } = string.Empty;

        [StringLength(250)]
        public string Notes { get; set; } = string.Empty;
    }
}
