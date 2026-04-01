using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventBooking.API.Controllers;

/// <summary>
/// Development-only endpoint that issues a JWT for testing.
/// Remove or protect this in production.
/// </summary>
[ApiController]
[Route("api/token")]
public class TokenController : ControllerBase
{
    private readonly IConfiguration _config;

    public TokenController(IConfiguration config) => _config = config;

    public record TokenRequest(string UserId, string UserName);

    // POST api/token
    [HttpPost]
    public IActionResult GenerateToken([FromBody] TokenRequest request)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey   = jwtSettings["SecretKey"]!;
        var issuer      = jwtSettings["Issuer"]!;
        var audience    = jwtSettings["Audience"]!;
        var expMinutes  = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, request.UserId),
            new Claim(ClaimTypes.Name, request.UserName),
            new Claim("sub", request.UserId),
            new Claim("name", request.UserName),
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(expMinutes),
            signingCredentials: creds
        );

        return Ok(new
        {
            token   = new JwtSecurityTokenHandler().WriteToken(token),
            expires = token.ValidTo
        });
    }
}
