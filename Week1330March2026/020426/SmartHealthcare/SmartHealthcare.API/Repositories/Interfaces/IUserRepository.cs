using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    }
}