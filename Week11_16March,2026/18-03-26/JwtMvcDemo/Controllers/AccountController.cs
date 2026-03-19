using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtMvcDemo.Models;

public class AccountController : Controller
{
    private readonly IConfiguration _config;

    public AccountController(IConfiguration config)
    {
        _config = config;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(User user)
    {
        if (user.Username == "admin" && user.Password == "1234")
        {
            var token = GenerateToken(user);

            // Store token in session (simple way)
            HttpContext.Session.SetString("JWToken", token);

            return RedirectToAction("Dashboard");
        }

        ViewBag.Message = "Invalid Login!";
        return View();
    }

    public IActionResult Dashboard()
    {
        var token = HttpContext.Session.GetString("JWToken");

        if (token == null)
        {
            return RedirectToAction("Login");
        }

        return View();
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
