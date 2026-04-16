using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartHealthCareSystem.Shared.Models;
using SmartHealthCareSystem.Shared.DTOs;
using SmartHealthCareSystem.API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartHealthCareSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDoctorService _doctorService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IDoctorService doctorService, IConfiguration configuration)
        {
            _userService = userService;
            _doctorService = doctorService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate department for doctors
            if (createUserDto.Role == "Doctor" && !createUserDto.DepartmentId.HasValue)
            {
                return BadRequest("Department is required for doctor registration");
            }

            var userDto = await _userService.CreateUserAsync(createUserDto);

            // Create doctor record if role is Doctor
            if (createUserDto.Role == "Doctor")
            {
                var createDoctorDto = new CreateDoctorDto
                {
                    UserId = userDto.UserId,
                    DepartmentId = createUserDto.DepartmentId.Value
                };
                await _doctorService.CreateDoctorAsync(createDoctorDto);
            }

            return Ok(new { message = "User registered successfully", user = userDto });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.AuthenticateAsync(loginDto.Email, loginDto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token, user.UserId, user.FullName, user.Role });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var keyString = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
