using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.DTOs
{
    public class CreatePrescriptionDTO
    {
        [Required]
        public int AppointmentId { get; set; }

        public string Diagnosis { get; set; } = string.Empty;

        public string Instructions { get; set; } = string.Empty;

        public List<PrescriptionMedicineDTO> Medicines { get; set; } = new();
    }

    public class PrescriptionMedicineDTO
    {
        public int MedicineId { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public int DurationDays { get; set; }
    }

    public class PrescriptionDTO
    {
        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; }
        public List<PrescriptionMedicineDTO> Medicines { get; set; } = new();
    }
}