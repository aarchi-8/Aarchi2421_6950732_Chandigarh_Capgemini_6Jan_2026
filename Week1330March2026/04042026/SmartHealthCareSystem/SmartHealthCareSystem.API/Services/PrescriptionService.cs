using AutoMapper;
using SmartHealthCareSystem.Shared.Models;
using SmartHealthCareSystem.API.Repositories;
using SmartHealthCareSystem.Shared.DTOs;

namespace SmartHealthCareSystem.API.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IRepository<Prescription> _prescriptionRepository;
        private readonly IMapper _mapper;

        public PrescriptionService(IRepository<Prescription> prescriptionRepository, IMapper mapper)
        {
            _prescriptionRepository = prescriptionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PrescriptionDto>> GetAllPrescriptionsAsync()
        {
            var prescriptions = await _prescriptionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
        }

        public async Task<PrescriptionDto?> GetPrescriptionByIdAsync(int id)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);
            return _mapper.Map<PrescriptionDto?>(prescription);
        }

        public async Task<PrescriptionDto> CreatePrescriptionAsync(CreatePrescriptionDto createPrescriptionDto)
        {
            var prescription = _mapper.Map<Prescription>(createPrescriptionDto);
            await _prescriptionRepository.AddAsync(prescription);
            await _prescriptionRepository.SaveChangesAsync();
            return _mapper.Map<PrescriptionDto>(prescription);
        }

        public async Task<PrescriptionDto?> UpdatePrescriptionAsync(int id, UpdatePrescriptionDto updatePrescriptionDto)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);
            if (prescription == null) return null;

            _mapper.Map(updatePrescriptionDto, prescription);
            await _prescriptionRepository.UpdateAsync(prescription);
            await _prescriptionRepository.SaveChangesAsync();
            return _mapper.Map<PrescriptionDto?>(prescription);
        }

        public async Task<bool> DeletePrescriptionAsync(int id)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);
            if (prescription == null) return false;

            await _prescriptionRepository.DeleteAsync(id);
            await _prescriptionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByAppointmentIdAsync(int appointmentId)
        {
            var prescriptions = await _prescriptionRepository.GetAllAsync();
            var appointmentPrescriptions = prescriptions.Where(p => p.AppointmentId == appointmentId);
            return _mapper.Map<IEnumerable<PrescriptionDto>>(appointmentPrescriptions);
        }
    }
}
