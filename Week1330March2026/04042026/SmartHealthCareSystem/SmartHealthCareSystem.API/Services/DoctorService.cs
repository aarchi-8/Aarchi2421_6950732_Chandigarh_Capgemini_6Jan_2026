using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartHealthCareSystem.API.Repositories;
using SmartHealthCareSystem.Shared.Data;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.Shared.Models;

namespace SmartHealthCareSystem.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public DoctorService(IRepository<Doctor> doctorRepository, ApplicationDbContext dbContext, IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _dbContext.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(int id)
        {
            var doctor = await _dbContext.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            return _mapper.Map<DoctorDto?>(doctor);
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
        {
            var doctor = _mapper.Map<Doctor>(createDoctorDto);
            await _doctorRepository.AddAsync(doctor);
            await _doctorRepository.SaveChangesAsync();
            await _dbContext.Entry(doctor).Reference(d => d.User).LoadAsync();
            await _dbContext.Entry(doctor).Reference(d => d.Department).LoadAsync();
            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto?> UpdateDoctorAsync(int id, UpdateDoctorDto updateDoctorDto)
        {
            var doctor = await _dbContext.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null) return null;

            _mapper.Map(updateDoctorDto, doctor);
            await _doctorRepository.UpdateAsync(doctor);
            await _doctorRepository.SaveChangesAsync();
            return _mapper.Map<DoctorDto?>(doctor);
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null) return false;

            await _doctorRepository.DeleteAsync(id);
            await _doctorRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DoctorDto>> GetDoctorsByDepartmentIdAsync(int departmentId)
        {
            var doctors = await _dbContext.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .Where(d => d.DepartmentId == departmentId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
    }
}
