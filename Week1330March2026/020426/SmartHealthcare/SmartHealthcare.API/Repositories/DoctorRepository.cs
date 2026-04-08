using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(AppDbContext context) : base(context) { }

        public async Task<Doctor?> GetDoctorWithDetailsAsync(int doctorId)
            => await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.DoctorSpecializations)
                    .ThenInclude(ds => ds.Specialization)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

        public async Task<Doctor?> GetDoctorByUserIdAsync(int userId)
            => await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.DoctorSpecializations)
                    .ThenInclude(ds => ds.Specialization)
                .FirstOrDefaultAsync(d => d.UserId == userId);

        public async Task<IEnumerable<Doctor>> GetAllDoctorsWithDetailsAsync()
            => await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.DoctorSpecializations)
                    .ThenInclude(ds => ds.Specialization)
                .ToListAsync();

        public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(int specializationId)
            => await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.DoctorSpecializations)
                    .ThenInclude(ds => ds.Specialization)
                .Where(d => d.DoctorSpecializations
                    .Any(ds => ds.SpecializationId == specializationId))
                .ToListAsync();
    }
}