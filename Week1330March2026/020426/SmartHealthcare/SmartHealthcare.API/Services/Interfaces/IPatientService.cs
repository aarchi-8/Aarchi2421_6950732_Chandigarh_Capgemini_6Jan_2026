using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDTO>> GetAllPatientsAsync();
        Task<PatientDTO?> GetPatientByIdAsync(int id);
        Task<PatientDTO?> GetPatientByUserIdAsync(int userId);
        Task<PatientDTO> CreatePatientAsync(int userId, CreatePatientDTO dto);
        Task<PatientDTO?> UpdatePatientAsync(int id, UpdatePatientDTO dto);
        Task<bool> DeletePatientAsync(int id);
    }
}