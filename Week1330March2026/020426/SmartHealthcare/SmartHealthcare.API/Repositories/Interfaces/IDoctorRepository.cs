using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<Doctor?> GetDoctorWithDetailsAsync(int doctorId);
        Task<Doctor?> GetDoctorByUserIdAsync(int userId);
        Task<IEnumerable<Doctor>> GetAllDoctorsWithDetailsAsync();
        Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(int specializationId);
    }
}