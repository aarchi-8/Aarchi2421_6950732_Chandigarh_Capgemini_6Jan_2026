using SmartHealthcare.Models.DTOs;

namespace SmartHealthcare.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDTO);
        Task<UserDTO?> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthResponseDTO?> RefreshTokenAsync(string refreshToken);
    }
}