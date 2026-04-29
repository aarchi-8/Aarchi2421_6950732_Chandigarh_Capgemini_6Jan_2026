using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AccountService.Controllers
{
  [ApiController]
  [Route("api/dev")]
  public class DevTokenController : ControllerBase
  {
    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
      "Admin",
      "Customer",
      "LoanOfficer",
      "SupportStaff"
    };

    private readonly IConfiguration _configuration;

    public DevTokenController(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    [HttpGet("token")]
    public IActionResult GetToken([FromQuery] string role = "Admin")
    {
      if (!AllowedRoles.Contains(role))
      {
        return BadRequest(new
        {
          Message = "Invalid role. Allowed values: Admin, Customer, LoanOfficer, SupportStaff"
        });
      }

      var jwtKey = _configuration["JwtSettings:Key"]
        ?? throw new InvalidOperationException("JwtSettings:Key is missing in configuration.");
      var jwtIssuer = _configuration["JwtSettings:Issuer"]
        ?? throw new InvalidOperationException("JwtSettings:Issuer is missing in configuration.");
      var jwtAudience = _configuration["JwtSettings:Audience"]
        ?? throw new InvalidOperationException("JwtSettings:Audience is missing in configuration.");

      var claims = new List<Claim>
      {
        new(ClaimTypes.NameIdentifier, "1"),
        new(ClaimTypes.Name, "TestUser"),
        new(ClaimTypes.Role, role),
        new("CustomerId", "101")
      };

      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
      var expires = DateTime.UtcNow.AddHours(8);

      var tokenDescriptor = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: expires,
        signingCredentials: credentials);

      var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

      return Ok(new
      {
        token,
        usage = "Copy Token value. In Swagger click Authorize and paste: Bearer <token>",
        role,
        expiresIn = "8 hours",
        note = "REMOVE THIS ENDPOINT BEFORE PRODUCTION"
      });
    }
  }
}
