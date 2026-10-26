namespace Marketplace.Models;

// Позиция заказа (связь многие-ко-многим между Order и Product).
// Таблица: OrderProduct
public class OrderProduct
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int Quantity { get; set; }

    // Цена на момент покупки. Хранится отдельно, потому что цена товара
    // в каталоге может измениться, а старые заказы должны остаться прежними
    public decimal PriceAtPurchase { get; set; }
}
