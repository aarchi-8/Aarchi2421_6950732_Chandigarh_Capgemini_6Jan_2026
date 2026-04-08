using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        Task<Patient?> GetPatientWithDetailsAsync(int patientId);
        Task<Patient?> GetPatientByUserIdAsync(int userId);
        Task<IEnumerable<Patient>> GetAllPatientsWithDetailsAsync();
    }
}