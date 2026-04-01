using ProductInventory.API.Models;

namespace ProductInventory.API.Services
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int id);
    }
}