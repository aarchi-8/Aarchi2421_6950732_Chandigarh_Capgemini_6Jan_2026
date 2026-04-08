using System.ComponentModel.DataAnnotations;
using SmartHealthcare.Models.Enums;

namespace SmartHealthcare.Models.DTOs
{
    public class CreatePatientDTO
    {
        [Required, MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        [MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        public string? MedicalHistory { get; set; }
    }

    public class UpdatePatientDTO
    {
        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }

        public string? MedicalHistory { get; set; }
    }

    public class PatientDTO
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? MedicalHistory { get; set; }
    }
}