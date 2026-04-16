using System.ComponentModel.DataAnnotations;

namespace SmartHealthCareSystem.Shared.DTOs
{
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CreateDepartmentDto
    {
        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(255)]
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateDepartmentDto
    {
        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(255)]
        public string Description { get; set; } = string.Empty;
    }
}
