using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderProcessing.API.Controllers;
using OrderProcessing.API.Models;
using OrderProcessing.API.Services;

namespace OrderProcessing.Tests.xUnit;

public class OrderControllerTests
{
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly OrderController _controller;

    // Constructor = xUnit's setup (runs before each test)
    public OrderControllerTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _controller = new OrderController(_mockOrderService.Object);
    }

    // ✅ Positive scenario: valid order → 201 Created
    [Fact]
    public async Task PlaceOrder_ValidOrder_ReturnsCreatedResult()
    {
        // Arrange
        var order = new Order { Id = 1, ProductName = "Laptop", Quantity = 2, Price = 999.99m };
        _mockOrderService
            .Setup(s => s.PlaceOrderAsync(order))
            .ReturnsAsync(true);  // mock returns success

        // Act
        var result = await _controller.PlaceOrder(order);

        // Assert
        var statusResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(201, statusResult.StatusCode);
    }

    // ❌ Negative scenario: failed order → 400 Bad Request
    [Fact]
    public async Task PlaceOrder_FailedOrder_ReturnsBadRequest()
    {
        // Arrange
        var order = new Order { Id = 2, ProductName = "Invalid", Quantity = 0, Price = 0 };
        _mockOrderService
            .Setup(s => s.PlaceOrderAsync(order))
            .ReturnsAsync(false);  // mock returns failure

        // Act
        var result = await _controller.PlaceOrder(order);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    // 🔍 Bonus: verify the service was actually called once
    [Fact]
    public async Task PlaceOrder_Always_CallsServiceOnce()
    {
        // Arrange
        var order = new Order { Id = 3, ProductName = "Mouse", Quantity = 1, Price = 29.99m };
        _mockOrderService
            .Setup(s => s.PlaceOrderAsync(It.IsAny<Order>()))
            .ReturnsAsync(true);

        // Act
        await _controller.PlaceOrder(order);

        // Assert — verify service was called exactly once
        _mockOrderService.Verify(s => s.PlaceOrderAsync(order), Times.Once);
    }
}