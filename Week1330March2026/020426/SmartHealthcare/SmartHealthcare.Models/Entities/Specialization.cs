using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Models.Entities
{
    public class Specialization
    {
        [Key]
        public int SpecializationId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        // Many-to-Many
        public ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
    }
}