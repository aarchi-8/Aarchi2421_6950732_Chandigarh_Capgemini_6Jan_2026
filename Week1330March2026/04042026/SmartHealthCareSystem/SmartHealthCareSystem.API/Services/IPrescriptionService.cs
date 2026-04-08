using SmartHealthCareSystem.Shared.Models;
using SmartHealthCareSystem.Shared.DTOs;

namespace SmartHealthCareSystem.API.Services
{
    public interface IPrescriptionService
    {
        Task<IEnumerable<PrescriptionDto>> GetAllPrescriptionsAsync();
        Task<PrescriptionDto?> GetPrescriptionByIdAsync(int id);
        Task<PrescriptionDto> CreatePrescriptionAsync(CreatePrescriptionDto createPrescriptionDto);
        Task<PrescriptionDto?> UpdatePrescriptionAsync(int id, UpdatePrescriptionDto updatePrescriptionDto);
        Task<bool> DeletePrescriptionAsync(int id);
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByAppointmentIdAsync(int appointmentId);
    }
}
