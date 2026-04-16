using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.Shared.Models;

namespace SmartHealthCareSystem.API.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<User?> AuthenticateAsync(string email, string password);
    }
}
