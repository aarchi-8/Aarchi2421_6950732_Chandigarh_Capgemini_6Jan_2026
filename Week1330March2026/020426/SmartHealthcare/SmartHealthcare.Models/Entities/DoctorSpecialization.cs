using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHealthcare.Models.Entities
{
    // Junction table for Many-to-Many: Doctor <-> Specialization
    public class DoctorSpecialization
    {
        public int DoctorId { get; set; }
        public int SpecializationId { get; set; }

        // Navigation
        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        [ForeignKey("SpecializationId")]
        public Specialization? Specialization { get; set; }
    }
}