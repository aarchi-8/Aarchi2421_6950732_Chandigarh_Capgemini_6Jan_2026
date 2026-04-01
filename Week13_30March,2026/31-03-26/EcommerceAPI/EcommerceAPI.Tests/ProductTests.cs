using EcommerceAPI.Controllers;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace EcommerceAPI.Tests;

public class ProductTests
{
    /// <summary>
    /// Helper: creates an in-memory AppDbContext with a unique DB name per test.
    /// </summary>
    private static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new AppDbContext(options);
    }

    private static ProductsController CreateController(AppDbContext context)
    {
        var logger = new Mock<ILogger<ProductsController>>().Object;
        return new ProductsController(context, logger);
    }

    // ─── GetAll ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ReturnsOk_WithAllProducts()
    {
        using var context = CreateInMemoryContext("GetAll_Test");
        context.Products.AddRange(
            new Product { Id = 1, Name = "Laptop",  Price = 999m, Category = "Electronics" },
            new Product { Id = 2, Name = "Monitor", Price = 399m, Category = "Electronics" }
        );
        await context.SaveChangesAsync();

        var controller = CreateController(context);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
        Assert.Equal(2, products.Count());
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoProducts()
    {
        using var context = CreateInMemoryContext("GetAll_Empty_Test");
        var controller = CreateController(context);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
        Assert.Empty(products);
    }

    // ─── GetById ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_ReturnsOk_WhenProductExists()
    {
        using var context = CreateInMemoryContext("GetById_Exists_Test");
        context.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 999m, Category = "Electronics" });
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var product = Assert.IsType<Product>(ok.Value);
        Assert.Equal("Laptop", product.Name);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductMissing()
    {
        using var context = CreateInMemoryContext("GetById_Missing_Test");
        var controller = CreateController(context);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ─── GetByCategory ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByCategory_ReturnsFilteredProducts()
    {
        using var context = CreateInMemoryContext("GetByCategory_Test");
        context.Products.AddRange(
            new Product { Id = 1, Name = "Laptop",     Price = 999m, Category = "Electronics" },
            new Product { Id = 2, Name = "Desk Chair", Price = 299m, Category = "Furniture"   },
            new Product { Id = 3, Name = "Monitor",    Price = 399m, Category = "Electronics" }
        );
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.GetByCategory("Electronics");

        var ok = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
        Assert.Equal(2, products.Count());
        Assert.All(products, p => Assert.Equal("Electronics", p.Category));
    }

    [Fact]
    public async Task GetByCategory_IsCaseInsensitive()
    {
        using var context = CreateInMemoryContext("GetByCategory_Case_Test");
        context.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 999m, Category = "Electronics" });
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.GetByCategory("ELECTRONICS");

        var ok = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
        Assert.Single(products);
    }

    // ─── Create ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_ReturnsCreated_WithNewProduct()
    {
        using var context = CreateInMemoryContext("Create_Test");
        var controller = CreateController(context);

        var newProduct = new Product { Name = "Keyboard", Price = 79.99m, Category = "Electronics" };
        var result = await controller.Create(newProduct);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var product = Assert.IsType<Product>(created.Value);
        Assert.Equal("Keyboard", product.Name);
        Assert.Equal(79.99m, product.Price);
    }

    // ─── Update ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_ReturnsNoContent_WhenProductExists()
    {
        using var context = CreateInMemoryContext("Update_Exists_Test");
        context.Products.Add(new Product { Id = 1, Name = "Old Name", Price = 100m, Category = "Electronics" });
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var updated = new Product { Name = "New Name", Price = 200m, Category = "Electronics" };
        var result = await controller.Update(1, updated);

        Assert.IsType<NoContentResult>(result);

        var productInDb = await context.Products.FindAsync(1);
        Assert.Equal("New Name", productInDb!.Name);
        Assert.Equal(200m, productInDb.Price);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenProductMissing()
    {
        using var context = CreateInMemoryContext("Update_Missing_Test");
        var controller = CreateController(context);
        var updated = new Product { Name = "Ghost", Price = 1m, Category = "None" };

        var result = await controller.Update(999, updated);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ─── Delete ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenProductExists()
    {
        using var context = CreateInMemoryContext("Delete_Exists_Test");
        context.Products.Add(new Product { Id = 1, Name = "ToDelete", Price = 50m, Category = "Test" });
        await context.SaveChangesAsync();

        var controller = CreateController(context);
        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(await context.Products.FindAsync(1));
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenProductMissing()
    {
        using var context = CreateInMemoryContext("Delete_Missing_Test");
        var controller = CreateController(context);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}