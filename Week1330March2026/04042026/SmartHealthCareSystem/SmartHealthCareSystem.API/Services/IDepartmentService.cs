using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.Shared.Models;

namespace SmartHealthCareSystem.API.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
        Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
        Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto);
        Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto updateDepartmentDto);
        Task<bool> DeleteDepartmentAsync(int id);
    }
}
