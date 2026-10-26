using System.Globalization;
using System.Text;
using Lesson35;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Core.Context.Database.EnsureCreated();

bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("=== Учёт покупок ===");
    Console.WriteLine("1. Показать все покупки");
    Console.WriteLine("2. Добавить покупку");
    Console.WriteLine("3. Изменить покупку");
    Console.WriteLine("4. Удалить покупку");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите пункт: ");

    switch (Console.ReadLine())
    {
        case "1":
            ShowPurchases();
            break;
        case "2":
            AddPurchase();
            break;
        case "3":
            EditPurchase();
            break;
        case "4":
            RemovePurchase();
            break;
        case "0":
            running = false;
            break;
        default:
            Console.WriteLine("Нет такого пункта меню.");
            break;
    }
}

void ShowPurchases()
{
    List<Purchase> purchases = Core.Context.Purchase.ToList();

    if (purchases.Count == 0)
    {
        Console.WriteLine("Список покупок пуст.");
        return;
    }

    foreach (Purchase purchase in purchases)
    {
        Console.WriteLine($"{purchase.Id}. {purchase.Name} - {purchase.Price:N2} руб. x {purchase.Count} шт.");
    }
}

void AddPurchase()
{
    Console.Write("Название: ");
    string name = (Console.ReadLine() ?? string.Empty).Trim();

    if (name.Length == 0)
    {
        Console.WriteLine("Название не может быть пустым.");
        return;
    }

    decimal? price = ReadPrice("Цена: ");
    int? count = ReadCount("Количество: ");

    if (price == null || count == null)
    {
        return;
    }

    Purchase newPurchase = new Purchase
    {
        Name = name,
        Price = price.Value,
        Count = count.Value
    };

    Core.Context.Purchase.Add(newPurchase);
    Core.Context.SaveChanges();
    Console.WriteLine($"Покупка добавлена, номер: {newPurchase.Id}.");
}

void EditPurchase()
{
    ShowPurchases();
    Purchase? editPurchase = FindPurchase("Номер покупки для изменения: ");

    if (editPurchase == null)
    {
        return;
    }

    Console.WriteLine("Оставьте поле пустым, чтобы не менять значение.");

    Console.Write($"Название ({editPurchase.Name}): ");
    string name = (Console.ReadLine() ?? string.Empty).Trim();
    if (name.Length > 0)
    {
        editPurchase.Name = name;
    }

    Console.Write($"Цена ({editPurchase.Price:N2}): ");
    string priceInput = Console.ReadLine() ?? string.Empty;
    if (priceInput.Trim().Length > 0)
    {
        decimal? price = ParsePrice(priceInput);
        if (price == null)
        {
            return;
        }
        editPurchase.Price = price.Value;
    }

    Console.Write($"Количество ({editPurchase.Count}): ");
    string countInput = Console.ReadLine() ?? string.Empty;
    if (countInput.Trim().Length > 0)
    {
        int? count = ParseCount(countInput);
        if (count == null)
        {
            return;
        }
        editPurchase.Count = count.Value;
    }

    Core.Context.SaveChanges();
    Console.WriteLine("Покупка изменена.");
}

void RemovePurchase()
{
    ShowPurchases();
    Purchase? removePurchase = FindPurchase("Номер покупки для удаления: ");

    if (removePurchase == null)
    {
        return;
    }

    Core.Context.Purchase.Remove(removePurchase);
    Core.Context.SaveChanges();
    Console.WriteLine("Покупка удалена.");
}

Purchase? FindPurchase(string prompt)
{
    Console.Write(prompt);

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Нужно ввести целое число.");
        return null;
    }

    Purchase? purchase = Core.Context.Purchase.FirstOrDefault(p => p.Id == id);

    if (purchase == null)
    {
        Console.WriteLine("Покупка с таким номером не найдена.");
    }

    return purchase;
}

decimal? ReadPrice(string prompt)
{
    Console.Write(prompt);
    return ParsePrice(Console.ReadLine() ?? string.Empty);
}

int? ReadCount(string prompt)
{
    Console.Write(prompt);
    return ParseCount(Console.ReadLine() ?? string.Empty);
}

decimal? ParsePrice(string input)
{
    string normalized = input.Trim().Replace(',', '.');

    if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price) && price >= 0)
    {
        return price;
    }

    Console.WriteLine("Цена должна быть неотрицательным числом.");
    return null;
}

int? ParseCount(string input)
{
    if (int.TryParse(input.Trim(), out int count) && count >= 0)
    {
        return count;
    }

    Console.WriteLine("Количество должно быть целым неотрицательным числом.");
    return null;
}
