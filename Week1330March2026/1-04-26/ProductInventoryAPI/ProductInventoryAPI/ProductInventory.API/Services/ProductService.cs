using ProductInventory.API.Models;

namespace ProductInventory.API.Services
{
    public class ProductService : IProductService
    {
        // Simulated in-memory data (no DB needed for now)
        private static readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop",   Description = "Gaming Laptop", Price = 999.99m, Stock = 10 },
            new Product { Id = 2, Name = "Mouse",    Description = "Wireless Mouse", Price = 29.99m,  Stock = 50 },
            new Product { Id = 3, Name = "Keyboard", Description = "Mechanical Keyboard", Price = 79.99m, Stock = 30 }
        };

        public Task<Product?> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }
    }
}