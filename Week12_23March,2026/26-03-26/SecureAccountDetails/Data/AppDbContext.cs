using Microsoft.EntityFrameworkCore;
using SecureAccountDetails.Models;

namespace SecureAccountDetails.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
    }
}
