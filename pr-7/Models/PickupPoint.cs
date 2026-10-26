namespace Marketplace.Models;

// Пункт выдачи заказов (ПВЗ). Таблица: PickupPoint
public class PickupPoint
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
