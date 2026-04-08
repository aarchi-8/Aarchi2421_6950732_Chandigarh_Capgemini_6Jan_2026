using Moq;
using Microsoft.AspNetCore.Mvc;
using ProductInventory.API.Controllers;
using ProductInventory.API.Models;
using ProductInventory.API.Services;
using Xunit;

namespace ProductInventory.Tests.xUnit
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductController _controller;

        // ✅ xUnit uses Constructor for setup (runs before each test automatically)
        public ProductControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductController(_mockService.Object);
        }

        // Test 1: Product EXISTS → should return 200 OK
        [Fact]
        public async Task GetProduct_ReturnsOk_WhenProductExists()
        {
            // Arrange — prepare fake data
            var fakeProduct = new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "Gaming Laptop",
                Price = 999.99m,
                Stock = 10
            };

            // Tell mock: when GetProductByIdAsync(1) is called → return fakeProduct
            _mockService
                .Setup(s => s.GetProductByIdAsync(1))
                .ReturnsAsync(fakeProduct);

            // Act — call the controller method
            var result = await _controller.GetProduct(1);

            // Assert — verify the result
            var okResult = Assert.IsType<OkObjectResult>(result);       // must be 200 OK
            var returnedProduct = Assert.IsType<Product>(okResult.Value); // must be Product
            Assert.Equal(1, returnedProduct.Id);                         // ID must be 1
            Assert.Equal("Laptop", returnedProduct.Name);                // Name must be Laptop
        }

        // ✅ Test 2: Product DOES NOT EXIST → should return 404 Not Found
        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange — tell mock: when ID 99 is called → return null
            _mockService
                .Setup(s => s.GetProductByIdAsync(99))
                .ReturnsAsync((Product?)null);

            // Act — call the controller method
            var result = await _controller.GetProduct(99);

            // Assert — must be 404 Not Found
            Assert.IsType<NotFoundResult>(result);
        }
    }
}