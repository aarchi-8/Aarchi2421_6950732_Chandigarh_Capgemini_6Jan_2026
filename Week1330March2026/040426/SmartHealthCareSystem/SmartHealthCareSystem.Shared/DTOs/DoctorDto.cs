using System.ComponentModel.DataAnnotations;

namespace SmartHealthCareSystem.Shared.DTOs
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int? ExperienceYears { get; set; }
        public string Availability { get; set; } = string.Empty;
    }

    public class CreateDoctorDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        public int? ExperienceYears { get; set; }

        [StringLength(100)]
        public string Availability { get; set; } = string.Empty;
    }

    public class UpdateDoctorDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        public int? ExperienceYears { get; set; }

        [StringLength(100)]
        public string Availability { get; set; } = string.Empty;
    }
}
