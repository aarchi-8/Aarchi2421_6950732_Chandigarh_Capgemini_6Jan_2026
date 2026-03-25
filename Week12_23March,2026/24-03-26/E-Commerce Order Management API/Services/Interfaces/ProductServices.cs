using E_Commerce_Order_Management_API.Models;
using System.Collections.Generic;

namespace E_Commerce_Order_Management_API.Services.Interfaces
{
    public interface IProductService
    {
        List<Product> GetAll();
        Product Add(Product product);
    }
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public Product Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        public List<Product> Search(string name)
        {
            return _context.Products
                .Where(p => p.Name.Contains(name))
                .ToList();
        }

        public List<Product> GetPaged(int page, int pageSize)
        {
            return _context.Products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Product Update(int id, Product product)
        {
            var existing = _context.Products.Find(id);
            if (existing == null) return null;

            existing.Name = product.Name;
            existing.Price = product.Price;

            _context.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            _context.SaveChanges();
            return true;
        }
    }
}
