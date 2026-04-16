using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartHealthCareSystem.Shared.Models;
using SmartHealthCareSystem.API.Repositories;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.Shared.Data;

namespace SmartHealthCareSystem.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public AppointmentService(IRepository<Appointment> appointmentRepository, IMapper mapper, ApplicationDbContext context)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Prescription)
                .ToListAsync();
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Prescription)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);
            return _mapper.Map<AppointmentDto?>(appointment);
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto)
        {
            var appointment = _mapper.Map<Appointment>(createAppointmentDto);
            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<AppointmentDto?> UpdateAppointmentAsync(int id, UpdateAppointmentDto updateAppointmentDto)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null) return null;

            _mapper.Map(updateAppointmentDto, appointment);
            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
            return _mapper.Map<AppointmentDto?>(appointment);
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null) return false;

            await _appointmentRepository.DeleteAsync(id);
            await _appointmentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Prescription)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Prescription)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
    }
}
