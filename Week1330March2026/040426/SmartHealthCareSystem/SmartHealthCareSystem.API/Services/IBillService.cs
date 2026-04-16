using SmartHealthCareSystem.Shared.Models;
using SmartHealthCareSystem.Shared.DTOs;

namespace SmartHealthCareSystem.API.Services
{
    public interface IBillService
    {
        Task<IEnumerable<BillDto>> GetAllBillsAsync();
        Task<BillDto?> GetBillByIdAsync(int id);
        Task<BillDto> CreateBillAsync(CreateBillDto createBillDto);
        Task<BillDto?> UpdateBillAsync(int id, UpdateBillDto updateBillDto);
        Task<bool> DeleteBillAsync(int id);
        Task<IEnumerable<BillDto>> GetBillsByAppointmentIdAsync(int appointmentId);
        Task<IEnumerable<BillDto>> GetBillsByPatientIdAsync(int patientId);
        Task<bool> MarkBillsAsPaidAsync(int patientId);
    }
}
