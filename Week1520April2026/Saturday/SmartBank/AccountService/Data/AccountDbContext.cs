using AccountService.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Data
{
  public class AccountDbContext : DbContext
  {
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Account>(entity =>
      {
        entity.ToTable("Accounts");
        entity.HasKey(e => e.AccountId);
        entity.Property(e => e.AccountNo).IsRequired().HasMaxLength(12);
        entity.HasIndex(e => e.AccountNo).IsUnique();
        entity.Property(e => e.AccountType).IsRequired();
        entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
        entity.Property(e => e.Status).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
      });

      modelBuilder.Entity<Transaction>(entity =>
      {
        entity.ToTable("Transactions");
        entity.HasKey(e => e.TransactionId);
        entity.Property(e => e.AccountNo).IsRequired().HasMaxLength(12);
        entity.Property(e => e.Type).IsRequired();
        entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        entity.Property(e => e.Description).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
      });
    }
  }
}
