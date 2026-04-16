using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login(string username, string password)
    {
        if (username == "admin" && password == "123")
        {
            var token = _tokenService.GenerateToken(username);
            return Ok(token);
        }

        return Unauthorized();
    }
}
