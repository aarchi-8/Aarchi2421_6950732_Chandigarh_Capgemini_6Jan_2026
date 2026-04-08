using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.DTOs
{
    public class CreateDoctorDTO
    {
        [Required, MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Qualification { get; set; } = string.Empty;

        public int ExperienceYears { get; set; }

        public decimal ConsultationFee { get; set; }

        public List<int> SpecializationIds { get; set; } = new();
    }

    public class UpdateDoctorDTO
    {
        public string? Phone { get; set; }
        public string? Qualification { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? ConsultationFee { get; set; }
        public bool? IsAvailable { get; set; }
        public List<int>? SpecializationIds { get; set; }
    }

    public class DoctorDTO
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public decimal ConsultationFee { get; set; }
        public bool IsAvailable { get; set; }
        public List<string> Specializations { get; set; } = new();
    }
}