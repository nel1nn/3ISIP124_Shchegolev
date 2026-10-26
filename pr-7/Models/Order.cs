namespace Marketplace.Models;

// Заказ. Итоговая сумма здесь не хранится: она вычисляется
// из позиций заказа, иначе данные дублировались бы (3НФ). Таблица: Order
public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int PickupPointId { get; set; }
    public PickupPoint? PickupPoint { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<OrderProduct> OrderProducts { get; set; } = new();
}
