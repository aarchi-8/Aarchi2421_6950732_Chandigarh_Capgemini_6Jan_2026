using AutoMapper;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IMapper mapper, ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<AppointmentDTO?> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithDetailsAsync(id);
            return appointment == null ? null : _mapper.Map<AppointmentDTO>(appointment);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByPatientAsync(int patientId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByPatientAsync(patientId);
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId);
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDateAsync(DateTime date)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDateAsync(date);
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<AppointmentDTO> CreateAppointmentAsync(int patientUserId, CreateAppointmentDTO dto)
        {
            try
            {
                var patient = await _patientRepository.GetPatientByUserIdAsync(patientUserId);
                if (patient == null) 
                    throw new Exception("Patient profile not found for this user");

                _logger.LogInformation("Found patient: {PatientId} for userId: {UserId}", patient.PatientId, patientUserId);

                var appointment = new Appointment
                {
                    PatientId = patient.PatientId,
                    DoctorId = dto.DoctorId,
                    AppointmentDate = dto.AppointmentDate,
                    Reason = dto.Reason,
                    Status = Models.Enums.AppointmentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Creating appointment: PatientId={PatientId}, DoctorId={DoctorId}, Date={Date}", 
                    appointment.PatientId, appointment.DoctorId, appointment.AppointmentDate);

                var created = await _appointmentRepository.CreateAsync(appointment);
                _logger.LogInformation("Appointment created with ID: {AppointmentId}", created.AppointmentId);

                var withDetails = await _appointmentRepository.GetAppointmentWithDetailsAsync(created.AppointmentId);
                _logger.LogInformation("Appointment booked successfully for PatientId: {PatientId}", patient.PatientId);
                return _mapper.Map<AppointmentDTO>(withDetails);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error: {Message}", dbEx.InnerException?.Message ?? dbEx.Message);
                throw new Exception($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<AppointmentDTO?> UpdateAppointmentAsync(int id, UpdateAppointmentDTO dto)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithDetailsAsync(id);
            if (appointment == null) return null;

            if (dto.AppointmentDate.HasValue) appointment.AppointmentDate = dto.AppointmentDate.Value;
            if (dto.Reason != null) appointment.Reason = dto.Reason;
            if (dto.Status.HasValue) appointment.Status = dto.Status.Value;
            if (dto.Notes != null) appointment.Notes = dto.Notes;

            await _appointmentRepository.UpdateAsync(appointment);
            var updated = await _appointmentRepository.GetAppointmentWithDetailsAsync(id);
            return _mapper.Map<AppointmentDTO>(updated);
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
            => await _appointmentRepository.DeleteAsync(id);
    }
}