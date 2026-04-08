using Microsoft.EntityFrameworkCore;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task<bool> EmailExistsAsync(string email)
            => await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
            => await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}