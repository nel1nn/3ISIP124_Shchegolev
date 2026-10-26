using Microsoft.EntityFrameworkCore;

namespace Lesson37;

public class UserContext : DbContext
{
    public DbSet<User> User => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=UserDatabase.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
    }
}
