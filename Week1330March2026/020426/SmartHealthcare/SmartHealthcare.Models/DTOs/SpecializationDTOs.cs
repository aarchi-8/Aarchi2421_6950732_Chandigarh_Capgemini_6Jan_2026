using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.DTOs
{
    public class SpecializationDTO
    {
        public int SpecializationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CreateSpecializationDTO
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;
    }
}