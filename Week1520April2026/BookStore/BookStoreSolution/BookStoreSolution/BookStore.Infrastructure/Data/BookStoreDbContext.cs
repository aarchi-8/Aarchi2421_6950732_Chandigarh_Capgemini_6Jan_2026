using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace BookStore.Infrastructure.Data;
public class BookStoreDbContext : DbContext
{
    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : base(options) { }
    public DbSet<Role> Roles => Set<Role>(); public DbSet<User> Users => Set<User>(); public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Category> Categories => Set<Category>(); public DbSet<Author> Authors => Set<Author>(); public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Book> Books => Set<Book>(); public DbSet<Order> Orders => Set<Order>(); public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Review> Reviews => Set<Review>(); public DbSet<Wishlist> Wishlists => Set<Wishlist>(); public DbSet<EmailLog> EmailLogs => Set<EmailLog>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e => { e.HasIndex(u => u.Email).IsUnique(); e.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId); });
        modelBuilder.Entity<UserProfile>(e => { e.HasKey(p => p.ProfileId); e.HasOne(p => p.User).WithOne(u => u.Profile).HasForeignKey<UserProfile>(p => p.UserId); e.HasIndex(p => p.UserId).IsUnique(); });
        modelBuilder.Entity<Book>(e => { e.HasIndex(b => b.ISBN).IsUnique(); e.Property(b => b.Price).HasColumnType("decimal(10,2)"); e.HasQueryFilter(b => !b.IsDeleted); e.HasOne(b => b.Category).WithMany(c => c.Books).HasForeignKey(b => b.CategoryId); e.HasOne(b => b.Author).WithMany(a => a.Books).HasForeignKey(b => b.AuthorId); e.HasOne(b => b.Publisher).WithMany(p => p.Books).HasForeignKey(b => b.PublisherId); });
        modelBuilder.Entity<Order>(e => { e.Property(o => o.TotalAmount).HasColumnType("decimal(12,2)"); e.HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId); });
        modelBuilder.Entity<OrderItem>(e => { e.Property(oi => oi.Price).HasColumnType("decimal(10,2)"); e.HasOne(oi => oi.Order).WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId); e.HasOne(oi => oi.Book).WithMany(b => b.OrderItems).HasForeignKey(oi => oi.BookId).OnDelete(DeleteBehavior.Restrict); });
        modelBuilder.Entity<Review>(e => { e.HasOne(r => r.User).WithMany(u => u.Reviews).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict); e.HasOne(r => r.Book).WithMany(b => b.Reviews).HasForeignKey(r => r.BookId).OnDelete(DeleteBehavior.Restrict); });
        modelBuilder.Entity<Wishlist>(e => { e.HasKey(w => new { w.UserId, w.BookId }); e.HasOne(w => w.User).WithMany(u => u.Wishlists).HasForeignKey(w => w.UserId); e.HasOne(w => w.Book).WithMany(b => b.Wishlists).HasForeignKey(w => w.BookId); });
        modelBuilder.Entity<Role>().HasData(new Role { RoleId = 1, RoleName = "Admin" }, new Role { RoleId = 2, RoleName = "Customer" });
        modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 1, Name = "Fiction" }, new Category { CategoryId = 2, Name = "Science" }, new Category { CategoryId = 3, Name = "Technology" }, new Category { CategoryId = 4, Name = "History" }, new Category { CategoryId = 5, Name = "Self-Help" });
        modelBuilder.Entity<Author>().HasData(new Author { AuthorId = 1, Name = "R.K. Narayan" }, new Author { AuthorId = 2, Name = "A.P.J. Abdul Kalam" }, new Author { AuthorId = 3, Name = "Chetan Bhagat" });
        modelBuilder.Entity<Publisher>().HasData(new Publisher { PublisherId = 1, Name = "Penguin India" }, new Publisher { PublisherId = 2, Name = "HarperCollins India" }, new Publisher { PublisherId = 3, Name = "Rupa Publications" });
    }
}