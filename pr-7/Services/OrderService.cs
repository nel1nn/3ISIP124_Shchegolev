using Marketplace.Data;
using Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Services;

// Оформление заказов и история покупок
public class OrderService
{
    private readonly MarketplaceContext _context;

    public OrderService(MarketplaceContext context)
    {
        _context = context;
    }

    // Покупка одного товара напрямую, минуя корзину
    public (Order? Order, string? Error) BuySingleProduct(int userId, int productId, int quantity, int pickupPointId)
    {
        if (quantity <= 0)
            return (null, "Количество должно быть больше нуля.");

        Product? product = _context.Product.Find(productId);
        if (product == null)
            return (null, "Товар с таким номером не найден.");

        if (!_context.PickupPoint.Any(point => point.Id == pickupPointId))
            return (null, "Пункт выдачи с таким номером не найден.");

        var order = new Order
        {
            UserId = userId,
            PickupPointId = pickupPointId,
            CreatedAt = DateTime.Now
        };

        order.OrderProducts.Add(new OrderProduct
        {
            ProductId = product.Id,
            Quantity = quantity,
            PriceAtPurchase = product.Price
        });

        _context.Order.Add(order);
        _context.SaveChanges();
        return (order, null);
    }

    // Покупка одной позиции, которая лежит в корзине. После покупки она из корзины удаляется
    public (Order? Order, string? Error) BuyCartItem(int userId, int productId, int pickupPointId)
    {
        CartItem? item = _context.CartItem
            .Include(c => c.Product)
            .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

        if (item == null || item.Product == null)
            return (null, "Этого товара нет в вашей корзине.");

        if (!_context.PickupPoint.Any(point => point.Id == pickupPointId))
            return (null, "Пункт выдачи с таким номером не найден.");

        var order = new Order
        {
            UserId = userId,
            PickupPointId = pickupPointId,
            CreatedAt = DateTime.Now
        };

        order.OrderProducts.Add(new OrderProduct
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            PriceAtPurchase = item.Product.Price
        });

        _context.Order.Add(order);
        _context.CartItem.Remove(item);
        _context.SaveChanges();
        return (order, null);
    }

    // Покупка всей корзины одним заказом. После покупки корзина очищается
    public (Order? Order, string? Error) BuyWholeCart(int userId, int pickupPointId)
    {
        List<CartItem> items = _context.CartItem
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToList();

        if (items.Count == 0)
            return (null, "Корзина пуста.");

        if (!_context.PickupPoint.Any(point => point.Id == pickupPointId))
            return (null, "Пункт выдачи с таким номером не найден.");

        var order = new Order
        {
            UserId = userId,
            PickupPointId = pickupPointId,
            CreatedAt = DateTime.Now
        };

        foreach (CartItem item in items)
        {
            order.OrderProducts.Add(new OrderProduct
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                PriceAtPurchase = item.Product!.Price
            });
        }

        _context.Order.Add(order);
        _context.CartItem.RemoveRange(items);
        _context.SaveChanges();
        return (order, null);
    }

    // Заказы пользователя, отсортированные по дате.
    // newestFirst = true: сначала новые, false: сначала старые
    public List<Order> GetOrders(int userId, bool newestFirst)
    {
        IQueryable<Order> query = _context.Order
            .Include(order => order.PickupPoint)
            .Include(order => order.OrderProducts)
                .ThenInclude(op => op.Product)
            .Where(order => order.UserId == userId);

        query = newestFirst
            ? query.OrderByDescending(order => order.CreatedAt)
            : query.OrderBy(order => order.CreatedAt);

        return query.ToList();
    }

    // Все пункты выдачи для выбора при покупке
    public List<PickupPoint> GetPickupPoints()
    {
        return _context.PickupPoint.OrderBy(point => point.Id).ToList();
    }

    // Каталог доступен всем, в том числе незарегистрированным
    public List<Product> GetProducts()
    {
        return _context.Product
            .Include(product => product.Category)
            .OrderBy(product => product.Id)
            .ToList();
    }
}
