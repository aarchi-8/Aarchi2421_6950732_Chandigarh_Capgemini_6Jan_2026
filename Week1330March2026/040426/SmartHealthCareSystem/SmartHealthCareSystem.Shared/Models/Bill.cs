using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthCareSystem.Shared.Models
{
    public class Bill
    {
        [Key]
        public int BillId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ConsultationFee { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MedicineCharges { get; set; }

        public decimal TotalAmount { get; set; }

        [Required]
        [RegularExpression("Paid|Unpaid")]
        public string PaymentStatus { get; set; } = "Unpaid";

        // Navigation property
        [ForeignKey("AppointmentId")]
        public Appointment? Appointment { get; set; }
    }
}
