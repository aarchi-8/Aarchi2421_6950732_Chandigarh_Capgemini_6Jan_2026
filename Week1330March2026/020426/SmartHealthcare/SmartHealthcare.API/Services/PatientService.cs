using AutoMapper;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IPatientRepository patientRepository,
            IMapper mapper, ILogger<PatientService> logger)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PatientDTO>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllPatientsWithDetailsAsync();
            return _mapper.Map<IEnumerable<PatientDTO>>(patients);
        }

        public async Task<PatientDTO?> GetPatientByIdAsync(int id)
        {
            var patient = await _patientRepository.GetPatientWithDetailsAsync(id);
            return patient == null ? null : _mapper.Map<PatientDTO>(patient);
        }

        public async Task<PatientDTO?> GetPatientByUserIdAsync(int userId)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
            return patient == null ? null : _mapper.Map<PatientDTO>(patient);
        }

        public async Task<PatientDTO> CreatePatientAsync(int userId, CreatePatientDTO dto)
        {
            var patient = _mapper.Map<Patient>(dto);
            patient.UserId = userId;
            var created = await _patientRepository.CreateAsync(patient);
            var withDetails = await _patientRepository.GetPatientWithDetailsAsync(created.PatientId);
            _logger.LogInformation("Patient profile created for UserId: {UserId}", userId);
            return _mapper.Map<PatientDTO>(withDetails);
        }

        public async Task<PatientDTO?> UpdatePatientAsync(int id, UpdatePatientDTO dto)
        {
            var patient = await _patientRepository.GetPatientWithDetailsAsync(id);
            if (patient == null) return null;

            if (dto.Phone != null) patient.Phone = dto.Phone;
            if (dto.Address != null) patient.Address = dto.Address;
            if (dto.MedicalHistory != null) patient.MedicalHistory = dto.MedicalHistory;

            await _patientRepository.UpdateAsync(patient);
            return _mapper.Map<PatientDTO>(patient);
        }

        public async Task<bool> DeletePatientAsync(int id)
            => await _patientRepository.DeleteAsync(id);
    }
}