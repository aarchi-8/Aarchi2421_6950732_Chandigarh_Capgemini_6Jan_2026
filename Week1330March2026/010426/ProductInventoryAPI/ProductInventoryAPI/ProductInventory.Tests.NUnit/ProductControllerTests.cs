using Moq;
using Microsoft.AspNetCore.Mvc;
using ProductInventory.API.Controllers;
using ProductInventory.API.Models;
using ProductInventory.API.Services;
using NUnit.Framework;

namespace ProductInventory.Tests.NUnit
{
    [TestFixture]  // Marks this class as a test class in NUnit
    public class ProductControllerTests
    {
        private Mock<IProductService> _mockService;
        private ProductController _controller;

        // NUnit uses [SetUp] attribute instead of constructor
        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductController(_mockService.Object);
        }

        [Test]  // NUnit uses [Test] instead of [Fact]
        public async Task GetProduct_ReturnsOk_WhenProductExists()
        {
            // Arrange
            var fakeProduct = new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "Gaming Laptop",
                Price = 999.99m,
                Stock = 10
            };

            _mockService
                .Setup(s => s.GetProductByIdAsync(1))
                .ReturnsAsync(fakeProduct);

            // Act
            var result = await _controller.GetProduct(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            var returnedProduct = okResult.Value as Product;
            Assert.That(returnedProduct, Is.Not.Null);
            Assert.That(returnedProduct!.Id, Is.EqualTo(1));
            Assert.That(returnedProduct.Name, Is.EqualTo("Laptop"));
        }

        [Test]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetProductByIdAsync(99))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _controller.GetProduct(99);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}