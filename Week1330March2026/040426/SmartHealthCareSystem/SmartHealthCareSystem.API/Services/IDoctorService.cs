using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.Shared.Models;

namespace SmartHealthCareSystem.API.Services
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto?> GetDoctorByIdAsync(int id);
        Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto);
        Task<DoctorDto?> UpdateDoctorAsync(int id, UpdateDoctorDto updateDoctorDto);
        Task<bool> DeleteDoctorAsync(int id);
        Task<IEnumerable<DoctorDto>> GetDoctorsByDepartmentIdAsync(int departmentId);
    }
}
