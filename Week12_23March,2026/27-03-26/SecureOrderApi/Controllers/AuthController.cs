using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecureOrderApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecureOrderApi.Controllers
{


    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthController));

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            log.Info($"Login attempt: {dto.Email}");

            if (dto.Password != "1234")
            {
                log.Warn("Failed login attempt");
                return Unauthorized();
            }

            try
            {
                var key = Encoding.UTF8.GetBytes("MySuperSecretKey1234567890123456");

                var token = new JwtSecurityToken(
                    claims: new[] { new Claim(ClaimTypes.Name, dto.Email) },
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256)
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                log.Info($"Token generated for user: {dto.Email}");

                return Ok(jwt);
            }
            catch (Exception ex)
            {
                log.Error("Login exception", ex);
                return StatusCode(500, ex.ToString()); // 👈 VERY IMPORTANT
            }
        }
    }
}
