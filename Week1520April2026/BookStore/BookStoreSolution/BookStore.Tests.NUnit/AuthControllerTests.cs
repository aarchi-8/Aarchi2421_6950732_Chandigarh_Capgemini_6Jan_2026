using BookStore.API.Controllers;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Security.Cryptography;
using System.Text;
namespace BookStore.Tests.NUnit;
[TestFixture] public class AuthControllerTests
{
    private BookStoreDbContext _ctx = null!; private Mock<ITokenService> _tokenMock = null!; private AuthController _ctrl = null!;
    [SetUp] public void Setup() { var o = new DbContextOptionsBuilder<BookStoreDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options; _ctx = new BookStoreDbContext(o); _tokenMock = new Mock<ITokenService>(); _ctrl = new AuthController(_ctx, _tokenMock.Object); _ctx.Roles.Add(new Role { RoleId = 2, RoleName = "Customer" }); _ctx.SaveChanges(); }
    [TearDown] public void TearDown() { _ctx.Database.EnsureDeleted(); _ctx.Dispose(); }
    [Test] public async Task Register_New_ReturnsOk() { var r = await _ctrl.Register(new UserRegisterDto { FullName = "T", Email = "t@t.com", Password = "P1!", Phone = "1234567890" }); Assert.That(r.Result, Is.TypeOf<OkObjectResult>()); }
    [Test] public async Task Register_Duplicate_ReturnsBadRequest() { _ctx.Users.Add(new User { FullName = "E", Email = "d@t.com", PasswordHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("P"))), Phone = "1", RoleId = 2 }); await _ctx.SaveChangesAsync(); var r = await _ctrl.Register(new UserRegisterDto { FullName = "N", Email = "d@t.com", Password = "P", Phone = "2" }); Assert.That(r.Result, Is.TypeOf<BadRequestObjectResult>()); }
    [Test] public async Task Login_Invalid_ReturnsUnauthorized() { var r = await _ctrl.Login(new UserLoginDto { Email = "x@x.com", Password = "w" }); Assert.That(r.Result, Is.TypeOf<UnauthorizedObjectResult>()); }
}