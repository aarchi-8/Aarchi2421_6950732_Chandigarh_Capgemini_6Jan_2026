using System.Security.Cryptography;
using System.Text;
using Asp.Versioning;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using BookStore.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BookStore.API.Controllers;
[ApiVersion("1.0")] [Route("api/v{version:apiVersion}/[controller]")] [ApiController]
public class AuthController : ControllerBase
{
    private readonly BookStoreDbContext _context; private readonly ITokenService _tokenService;
    public AuthController(BookStoreDbContext context, ITokenService tokenService) { _context = context; _tokenService = tokenService; }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse>> Register(UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email)) return BadRequest(ApiResponse.Fail("Email already registered."));
        _context.Users.Add(new User { FullName = dto.FullName, Email = dto.Email, PasswordHash = Hash(dto.Password), Phone = dto.Phone, RoleId = 2 });
        await _context.SaveChangesAsync(); return Ok(ApiResponse.Ok("Registration successful."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(UserLoginDto dto)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || user.PasswordHash != Hash(dto.Password)) return Unauthorized(ApiResponse<AuthResponseDto>.Fail("Invalid credentials."));
        var at = _tokenService.GenerateAccessToken(user); var rt = _tokenService.GenerateRefreshToken();
        user.RefreshToken = rt; user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); await _context.SaveChangesAsync();
        return Ok(ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto { Token = at, RefreshToken = rt, FullName = user.FullName, Role = user.Role.RoleName }));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh(RefreshTokenDto dto)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);
        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow) return Unauthorized(ApiResponse<AuthResponseDto>.Fail("Invalid or expired refresh token."));
        var at = _tokenService.GenerateAccessToken(user); var rt = _tokenService.GenerateRefreshToken();
        user.RefreshToken = rt; user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); await _context.SaveChangesAsync();
        return Ok(ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto { Token = at, RefreshToken = rt, FullName = user.FullName, Role = user.Role.RoleName }));
    }

    private static string Hash(string p) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(p)));
}