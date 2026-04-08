using System.ComponentModel.DataAnnotations;
using SmartHealthcare.Models.Enums;

namespace SmartHealthcare.Models.DTOs
{
    public class CreateAppointmentDTO
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required, MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    public class UpdateAppointmentDTO
    {
        public DateTime? AppointmentDate { get; set; }
        public string? Reason { get; set; }
        public AppointmentStatus? Status { get; set; }
        public string? Notes { get; set; }
    }

    public class AppointmentDTO
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}