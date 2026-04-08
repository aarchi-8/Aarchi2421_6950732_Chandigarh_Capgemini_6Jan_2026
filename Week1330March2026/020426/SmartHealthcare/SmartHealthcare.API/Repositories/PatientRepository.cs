using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(AppDbContext context) : base(context) { }

        public async Task<Patient?> GetPatientWithDetailsAsync(int patientId)
            => await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

        public async Task<Patient?> GetPatientByUserIdAsync(int userId)
            => await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

        public async Task<IEnumerable<Patient>> GetAllPatientsWithDetailsAsync()
            => await _context.Patients
                .Include(p => p.User)
                .ToListAsync();
    }
}