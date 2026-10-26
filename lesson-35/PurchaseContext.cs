using Microsoft.EntityFrameworkCore;

namespace Lesson35;

public class PurchaseContext : DbContext
{
    public DbSet<Purchase> Purchase => Set<Purchase>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=PurchaseDatabase.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Purchase>().ToTable("Purchase");
        modelBuilder.Entity<Purchase>().Property(purchase => purchase.Price).HasPrecision(18, 2);
    }
}
