using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDTO>> GetAllAppointmentsAsync();
        Task<AppointmentDTO?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDateAsync(DateTime date);
        Task<AppointmentDTO> CreateAppointmentAsync(int patientUserId, CreateAppointmentDTO dto);
        Task<AppointmentDTO?> UpdateAppointmentAsync(int id, UpdateAppointmentDTO dto);
        Task<bool> DeleteAppointmentAsync(int id);
    }
}