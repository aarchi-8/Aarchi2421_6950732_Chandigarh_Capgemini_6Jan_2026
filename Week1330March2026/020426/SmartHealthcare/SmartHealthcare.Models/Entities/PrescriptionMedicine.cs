using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthcare.Models.Entities
{
    // Junction table: Prescription <-> Medicine
    public class PrescriptionMedicine
    {
        public int PrescriptionId { get; set; }
        public int MedicineId { get; set; }

        [MaxLength(100)]
        public string Dosage { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Frequency { get; set; } = string.Empty;

        public int DurationDays { get; set; }

        // Navigation
        [ForeignKey("PrescriptionId")]
        public Prescription? Prescription { get; set; }

        [ForeignKey("MedicineId")]
        public Medicine? Medicine { get; set; }
    }
}