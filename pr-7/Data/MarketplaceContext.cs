using Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Data;

// Контекст базы данных. База хранится в файле MarketplaceDatabase.db (SQLite)
public class MarketplaceContext : DbContext
{
    // Имена DbSet в единственном числе, как и таблицы
    public DbSet<User> User => Set<User>();
    public DbSet<Category> Category => Set<Category>();
    public DbSet<Product> Product => Set<Product>();
    public DbSet<CartItem> CartItem => Set<CartItem>();
    public DbSet<PickupPoint> PickupPoint => Set<PickupPoint>();
    public DbSet<Order> Order => Set<Order>();
    public DbSet<OrderProduct> OrderProduct => Set<OrderProduct>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=MarketplaceDatabase.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Явно задаём имена таблиц в единственном числе
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<Product>().ToTable("Product");
        modelBuilder.Entity<CartItem>().ToTable("CartItem");
        modelBuilder.Entity<PickupPoint>().ToTable("PickupPoint");
        modelBuilder.Entity<Order>().ToTable("Order");
        modelBuilder.Entity<OrderProduct>().ToTable("OrderProduct");

        // Ник пользователя уникален
        modelBuilder.Entity<User>()
            .HasIndex(user => user.Nickname)
            .IsUnique();

        // Один и тот же товар в корзине пользователя только одной строкой
        modelBuilder.Entity<CartItem>()
            .HasIndex(cartItem => new { cartItem.UserId, cartItem.ProductId })
            .IsUnique();

        // SQLite не умеет хранить decimal нативно, храним как число с точностью
        modelBuilder.Entity<Product>().Property(product => product.Price).HasPrecision(18, 2);
        modelBuilder.Entity<OrderProduct>().Property(op => op.PriceAtPurchase).HasPrecision(18, 2);

        // Начальные данные: категории, товары и пункты выдачи
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Books" },
            new Category { Id = 3, Name = "Clothes" });

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Смартфон X10", Description = "6.5 дюймов, 128 ГБ", Price = 24990m, CategoryId = 1 },
            new Product { Id = 2, Name = "Наушники Air", Description = "Беспроводные, шумоподавление", Price = 4990m, CategoryId = 1 },
            new Product { Id = 3, Name = "Паттерны проектирования", Description = "Классическая книга по ООП", Price = 1890m, CategoryId = 2 },
            new Product { Id = 4, Name = "C# 12 в двух словах", Description = "Справочник по языку", Price = 1590m, CategoryId = 2 },
            new Product { Id = 5, Name = "Худи Basic", Description = "Хлопок, размер M", Price = 2990m, CategoryId = 3 },
            new Product { Id = 6, Name = "Кроссовки Run", Description = "Лёгкие, для бега", Price = 5490m, CategoryId = 3 });

        modelBuilder.Entity<PickupPoint>().HasData(
            new PickupPoint { Id = 1, Name = "ПВЗ Центральный", Address = "ул. Ленина, 10" },
            new PickupPoint { Id = 2, Name = "ПВЗ Северный", Address = "пр. Мира, 45" },
            new PickupPoint { Id = 3, Name = "ПВЗ Южный", Address = "ул. Гагарина, 7" });
    }
}
