using LearningPlatform.Api.Models;

namespace LearningPlatform.Api.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}
