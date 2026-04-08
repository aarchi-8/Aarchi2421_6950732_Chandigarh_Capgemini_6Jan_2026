using System.ComponentModel.DataAnnotations;

namespace SmartHealthCareSystem.Shared.DTOs
{
    public class BillDto
    {
        public int BillId { get; set; }
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public decimal ConsultationFee { get; set; }
        public decimal MedicineCharges { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string AppointmentStatus { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
    }

    public class CreateBillDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ConsultationFee { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MedicineCharges { get; set; }

        [Required]
        [RegularExpression("Paid|Unpaid")]
        public string PaymentStatus { get; set; } = "Unpaid";
    }

    public class UpdateBillDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ConsultationFee { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MedicineCharges { get; set; }

        [Required]
        [RegularExpression("Paid|Unpaid")]
        public string PaymentStatus { get; set; } = string.Empty;
    }
}
