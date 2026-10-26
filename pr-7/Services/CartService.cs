using Marketplace.Data;
using Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Services;

// Работа с корзиной пользователя
public class CartService
{
    private readonly MarketplaceContext _context;

    public CartService(MarketplaceContext context)
    {
        _context = context;
    }

    // Добавляет товар в корзину. Если он уже там, увеличивает количество
    public string? Add(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
            return "Количество должно быть больше нуля.";

        if (!_context.Product.Any(product => product.Id == productId))
            return "Товар с таким номером не найден.";

        CartItem? existing = _context.CartItem
            .FirstOrDefault(item => item.UserId == userId && item.ProductId == productId);

        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _context.CartItem.Add(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            });
        }

        _context.SaveChanges();
        return null;
    }

    // Возвращает содержимое корзины вместе с товарами
    public List<CartItem> GetItems(int userId)
    {
        return _context.CartItem
            .Include(item => item.Product)
            .Where(item => item.UserId == userId)
            .OrderBy(item => item.Id)
            .ToList();
    }

    // Удаляет товар из корзины. Возвращает false, если такого нет
    public bool Remove(int userId, int productId)
    {
        CartItem? item = _context.CartItem
            .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

        if (item == null)
            return false;

        _context.CartItem.Remove(item);
        _context.SaveChanges();
        return true;
    }
}
