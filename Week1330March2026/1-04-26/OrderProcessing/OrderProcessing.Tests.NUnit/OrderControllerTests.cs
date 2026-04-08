using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OrderProcessing.API.Controllers;
using OrderProcessing.API.Models;
using OrderProcessing.API.Services;

namespace OrderProcessing.Tests.NUnit;

[TestFixture]   // ← NUnit marks a test class with [TestFixture]
public class OrderControllerTests
{
    private Mock<IOrderService> _mockOrderService;
    private OrderController _controller;

    [SetUp]     // ← NUnit's setup hook, runs before each test
    public void SetUp()
    {
        _mockOrderService = new Mock<IOrderService>();
        _controller = new OrderController(_mockOrderService.Object);
    }

    // ✅ Positive scenario: valid order → 201 Created
    [Test]      // ← NUnit uses [Test] instead of xUnit's [Fact]
    public async Task PlaceOrder_ValidOrder_ReturnsCreatedResult()
    {
        // Arrange
        var order = new Order { Id = 1, ProductName = "Laptop", Quantity = 2, Price = 999.99m };
        _mockOrderService
            .Setup(s => s.PlaceOrderAsync(order))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.PlaceOrder(order);

        // Assert — NUnit uses Assert.That with Is.* constraints
        Assert.That(result, Is.InstanceOf<StatusCodeResult>());
        var statusResult = (StatusCodeResult)result;
        Assert.That(statusResult.StatusCode, Is.EqualTo(201));
    }

    // ❌ Negative scenario: failed order → 400 Bad Request
    [Test]
    public async Task PlaceOrder_FailedOrder_ReturnsBadRequest()
    {
        // Arrange
        var order = new Order { Id = 2, ProductName = "Invalid", Quantity = 0, Price = 0 };
        _mockOrderService
            .Setup(s => s.PlaceOrderAsync(order))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.PlaceOrder(order);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }

    // 🔍 Bonus: verify service was called once
    [Test]
    public async Task PlaceOrder_Always_CallsServiceOnce()
    {
        // Arrange
        var order = new Order { Id = 3, ProductName = "Mouse", Quantity = 1, Price = 29.99m };
        _mockOrderService
            .Setup(s => s.PlaceOrderAsync(It.IsAny<Order>()))
            .ReturnsAsync(true);

        // Act
        await _controller.PlaceOrder(order);

        // Assert
        _mockOrderService.Verify(s => s.PlaceOrderAsync(order), Times.Once);
    }

    [TearDown]  // ← NUnit's cleanup hook (optional here, useful for disposal)
    public void TearDown()
    {
        // Clean up if needed
    }
}
