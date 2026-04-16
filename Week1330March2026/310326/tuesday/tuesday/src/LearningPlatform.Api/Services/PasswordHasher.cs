using System.Security.Cryptography;
using System.Text;

namespace LearningPlatform.Api.Services;

public static class PasswordHasher
{
    public static string HashPassword(string value)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }

    public static bool VerifyPassword(string value, string hashed) => HashPassword(value) == hashed;
}
