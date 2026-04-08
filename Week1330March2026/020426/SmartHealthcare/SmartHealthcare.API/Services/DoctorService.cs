using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Helpers;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly ILogger<DoctorService> _logger;
        private readonly CacheHelper _cacheHelper;

        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper,
            AppDbContext context, ILogger<DoctorService> logger, CacheHelper cacheHelper)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
            _context = context;
            _logger = logger;
            _cacheHelper = cacheHelper;
        }

        public async Task<IEnumerable<DoctorDTO>> GetAllDoctorsAsync()
        {
            return await _cacheHelper.GetOrSetAsync(
                CacheHelper.Keys.AllDoctors,
                async () =>
                {
                    var doctors = await _doctorRepository.GetAllDoctorsWithDetailsAsync();
                    return _mapper.Map<IEnumerable<DoctorDTO>>(doctors).ToList();
                },
                slidingExpiration: TimeSpan.FromHours(1)
            ) ?? new List<DoctorDTO>();
        }

        /// <summary>
        /// Get doctors with pagination and filtering
        /// </summary>
        public async Task<PaginatedResult<DoctorDTO>> GetDoctorsWithPaginationAsync(DoctorFilterParams filterParams)
        {
            var query = _context.Doctors.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
            {
                var searchTerm = filterParams.SearchTerm.ToLower();
                query = query.Where(d => d.User!.FullName.ToLower().Contains(searchTerm) ||
                                        d.Qualification!.ToLower().Contains(searchTerm));
            }

            if (filterParams.SpecializationId.HasValue)
            {
                query = query.Where(d => d.DoctorSpecializations!.Any(ds => ds.SpecializationId == filterParams.SpecializationId));
            }

            if (filterParams.IsAvailable.HasValue)
            {
                query = query.Where(d => d.IsAvailable == filterParams.IsAvailable.Value);
            }

            if (filterParams.MinConsultationFee.HasValue)
            {
                query = query.Where(d => d.ConsultationFee >= filterParams.MinConsultationFee.Value);
            }

            if (filterParams.MaxConsultationFee.HasValue)
            {
                query = query.Where(d => d.ConsultationFee <= filterParams.MaxConsultationFee.Value);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(filterParams.SortBy))
            {
                query = filterParams.SortBy.ToLower() switch
                {
                    "name" => filterParams.IsDescending 
                        ? query.OrderByDescending(d => d.User!.FullName)
                        : query.OrderBy(d => d.User!.FullName),
                    "experience" => filterParams.IsDescending
                        ? query.OrderByDescending(d => d.ExperienceYears)
                        : query.OrderBy(d => d.ExperienceYears),
                    "fee" => filterParams.IsDescending
                        ? query.OrderByDescending(d => d.ConsultationFee)
                        : query.OrderBy(d => d.ConsultationFee),
                    _ => query.OrderBy(d => d.DoctorId)
                };
            }
            else
            {
                query = query.OrderBy(d => d.DoctorId);
            }

            // Apply pagination
            var skip = (filterParams.PageNumber - 1) * filterParams.PageSize;
            var doctors = await query
                .Skip(skip)
                .Take(filterParams.PageSize)
                .Include(d => d.User)
                .Include(d => d.DoctorSpecializations)
                .ThenInclude(ds => ds.Specialization)
                .Select(d => _mapper.Map<DoctorDTO>(d))
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} doctors with pagination", doctors.Count);

            return new PaginatedResult<DoctorDTO>(doctors, totalCount, filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<DoctorDTO?> GetDoctorByIdAsync(int id)
        {
            var cacheKey = string.Format(CacheHelper.Keys.Doctor, id);
            return await _cacheHelper.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var doctor = await _doctorRepository.GetDoctorWithDetailsAsync(id);
                    return doctor == null ? null : _mapper.Map<DoctorDTO>(doctor);
                },
                slidingExpiration: TimeSpan.FromHours(1)
            );
        }

        public async Task<IEnumerable<DoctorDTO>> GetDoctorsBySpecializationAsync(int specializationId)
        {
            var cacheKey = string.Format(CacheHelper.Keys.DoctorsBySpecialization, specializationId);
            return await _cacheHelper.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var doctors = await _context.Doctors
                        .Where(d => d.DoctorSpecializations!.Any(ds => ds.SpecializationId == specializationId))
                        .Include(d => d.User)
                        .Include(d => d.DoctorSpecializations)
                        .ThenInclude(ds => ds.Specialization)
                        .ToListAsync();
                    return _mapper.Map<IEnumerable<DoctorDTO>>(doctors).ToList();
                },
                slidingExpiration: TimeSpan.FromHours(1)
            ) ?? new List<DoctorDTO>();
        }

        public async Task<DoctorDTO> CreateDoctorAsync(int userId, CreateDoctorDTO dto)
        {
            var doctor = new Doctor
            {
                UserId = userId,
                Phone = dto.Phone,
                Qualification = dto.Qualification,
                ExperienceYears = dto.ExperienceYears,
                ConsultationFee = dto.ConsultationFee
            };

            var created = await _doctorRepository.CreateAsync(doctor);

            // Add specializations
            foreach (var specId in dto.SpecializationIds)
            {
                _context.DoctorSpecializations.Add(new DoctorSpecialization
                {
                    DoctorId = created.DoctorId,
                    SpecializationId = specId
                });
            }
            await _context.SaveChangesAsync();

            // Clear related caches
            _cacheHelper.Remove(CacheHelper.Keys.AllDoctors);
            _cacheHelper.RemoveByPattern("doctors_spec_");

            var withDetails = await _doctorRepository.GetDoctorWithDetailsAsync(created.DoctorId);
            _logger.LogInformation("Doctor profile created for UserId: {UserId}", userId);
            return _mapper.Map<DoctorDTO>(withDetails!);
        }

        public async Task<DoctorDTO?> UpdateDoctorAsync(int id, UpdateDoctorDTO dto)
        {
            var doctor = await _doctorRepository.GetDoctorWithDetailsAsync(id);
            if (doctor == null) return null;

            if (dto.Phone != null) doctor.Phone = dto.Phone;
            if (dto.Qualification != null) doctor.Qualification = dto.Qualification;
            if (dto.ExperienceYears.HasValue) doctor.ExperienceYears = dto.ExperienceYears.Value;
            if (dto.ConsultationFee.HasValue) doctor.ConsultationFee = dto.ConsultationFee.Value;
            if (dto.IsAvailable.HasValue) doctor.IsAvailable = dto.IsAvailable.Value;

            if (dto.SpecializationIds != null)
            {
                var existing = _context.DoctorSpecializations.Where(ds => ds.DoctorId == id);
                _context.DoctorSpecializations.RemoveRange(existing);
                foreach (var specId in dto.SpecializationIds)
                {
                    _context.DoctorSpecializations.Add(new DoctorSpecialization
                    {
                        DoctorId = id,
                        SpecializationId = specId
                    });
                }
                await _context.SaveChangesAsync();
            }

            await _doctorRepository.UpdateAsync(doctor);
            
            // Clear related caches
            _cacheHelper.Remove(CacheHelper.Keys.AllDoctors);
            _cacheHelper.Remove(string.Format(CacheHelper.Keys.Doctor, id));
            _cacheHelper.RemoveByPattern("doctors_spec_");

            var updated = await _doctorRepository.GetDoctorWithDetailsAsync(id);
            return _mapper.Map<DoctorDTO>(updated);
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var result = await _doctorRepository.DeleteAsync(id);
            if (result)
            {
                // Clear related caches
                _cacheHelper.Remove(CacheHelper.Keys.AllDoctors);
                _cacheHelper.Remove(string.Format(CacheHelper.Keys.Doctor, id));
                _cacheHelper.RemoveByPattern("doctors_spec_");
                _logger.LogInformation("Doctor {DoctorId} deleted and cache cleared", id);
            }
            return result;
        }
    }
}