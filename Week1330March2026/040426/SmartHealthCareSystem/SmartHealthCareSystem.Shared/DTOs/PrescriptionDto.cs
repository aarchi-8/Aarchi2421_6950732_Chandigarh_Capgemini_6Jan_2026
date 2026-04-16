using System.ComponentModel.DataAnnotations;

namespace SmartHealthCareSystem.Shared.DTOs
{
    public class PrescriptionDto
    {
        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Medicines { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class CreatePrescriptionDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [StringLength(255)]
        public string Diagnosis { get; set; } = string.Empty;

        public string Medicines { get; set; } = string.Empty;

        [StringLength(255)]
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdatePrescriptionDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [StringLength(255)]
        public string Diagnosis { get; set; } = string.Empty;

        public string Medicines { get; set; } = string.Empty;

        [StringLength(255)]
        public string Notes { get; set; } = string.Empty;
    }
}
