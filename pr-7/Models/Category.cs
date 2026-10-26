namespace Marketplace.Models;

// Категория товара. Вынесена в отдельную таблицу, чтобы название
// категории не повторялось в каждом товаре (3НФ). Таблица: Category
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = new();
}
