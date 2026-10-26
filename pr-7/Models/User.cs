namespace Marketplace.Models;

// Пользователь маркетплейса. Таблица: User
public class User
{
    public int Id { get; set; }
    public string Nickname { get; set; } = string.Empty;

    // Пароль в открытом виде не хранится: только хеш и соль
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<CartItem> CartItems { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
}
