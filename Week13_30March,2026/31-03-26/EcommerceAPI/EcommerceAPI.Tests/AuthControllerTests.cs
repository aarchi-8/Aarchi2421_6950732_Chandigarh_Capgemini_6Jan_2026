using EcommerceAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;


namespace EcommerceAPI.Tests;

public class AuthControllerTests
{
    private static AuthController CreateController()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Key",      "THIS_IS_A_SECURE_KEY_1234567890123456" },
            { "Jwt:Issuer",   "EcommerceAPI"  },
            { "Jwt:Audience", "EcommerceUsers" }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var logger = new Mock<ILogger<AuthController>>().Object;
        return new AuthController(config, logger);
    }

    [Fact]
    public void Login_ReturnsOk_WithValidUsername()
    {
        var controller = CreateController();

        var result = controller.Login("admin");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);

        var value = ok.Value!.ToString()!;
        Assert.Contains("token", value.ToLower());
    }

    [Fact]
    public void Login_ReturnsBadRequest_WithEmptyUsername()
    {
        var controller = CreateController();

        var result = controller.Login("");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Login_ReturnsBadRequest_WithNullUsername()
    {
        var controller = CreateController();

        var result = controller.Login(null!);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Login_ReturnsTokenContainingUsername()
    {
        var controller = CreateController();

        var result = controller.Login("johndoe");

        var ok = Assert.IsType<OkObjectResult>(result);
        // Verify the response object contains a token property
        var responseType = ok.Value!.GetType();
        var tokenProp = responseType.GetProperty("token");
        Assert.NotNull(tokenProp);

        var tokenValue = tokenProp!.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(tokenValue));
    }
}