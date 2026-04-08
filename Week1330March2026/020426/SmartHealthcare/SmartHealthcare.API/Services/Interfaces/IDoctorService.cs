using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDTO>> GetAllDoctorsAsync();
        Task<DoctorDTO?> GetDoctorByIdAsync(int id);
        Task<DoctorDTO> CreateDoctorAsync(int userId, CreateDoctorDTO dto);
        Task<DoctorDTO?> UpdateDoctorAsync(int id, UpdateDoctorDTO dto);
        Task<bool> DeleteDoctorAsync(int id);
        Task<IEnumerable<DoctorDTO>> GetDoctorsBySpecializationAsync(int specializationId);
        Task<PaginatedResult<DoctorDTO>> GetDoctorsWithPaginationAsync(DoctorFilterParams filterParams);
    }
}