using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Data;

/// <summary>
/// Application database context for the EcommerceAPI.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Index on Category for faster filtering
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Category)
            .HasDatabaseName("IX_Products_Category");

        // Seed data (optional demo products)
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop Pro",      Price = 1299.99m, Category = "Electronics",  Description = "High-performance laptop", CreatedAt = new DateTime(2025,1,1) },
            new Product { Id = 2, Name = "Wireless Mouse",  Price = 49.99m,   Category = "Electronics",  Description = "Ergonomic wireless mouse",  CreatedAt = new DateTime(2025,1,1) },
            new Product { Id = 3, Name = "Desk Chair",      Price = 399.00m,  Category = "Furniture",    Description = "Ergonomic office chair",    CreatedAt = new DateTime(2025,1,1) },
            new Product { Id = 4, Name = "Coffee Maker",    Price = 89.95m,   Category = "Appliances",   Description = "12-cup programmable",       CreatedAt = new DateTime(2025,1,1) }
        );
    }
}