using System;
using System.Collections.Generic;
using System.Linq;

namespace practice_2
{
    public enum ProductCategory
    {
        Electronics,
        Groceries,
        Clothing,
        Books
    }

    public class Product
    {
        public int Id { get; init; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public ProductCategory Category { get; set; }

        public bool IsInStock => Quantity > 0;

        public Product(int id, string name, decimal price, int quantity, ProductCategory category)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public override string ToString()
        {
            string stockStatus = IsInStock ? $"Да ({Quantity} шт.)" : "Нет в наличии";
            return $"[ID: {Id}] {Name} | Категория: {Category} | Цена: {Price:C2} | Остаток: {stockStatus}";
        }
    }

    public class Store
    {
        private readonly List<Product> _products = new();
        private int _nextId = 1;

        public Store()
        {
            SeedInitialData();
        }

        private int GenerateId() => _nextId++;

        private void SeedInitialData()
        {
            _products.Add(new Product(GenerateId(), "Наушники Sony", 8990.00m, 12, ProductCategory.Electronics));
            _products.Add(new Product(GenerateId(), "Кофе в зернах 1кг", 1450.50m, 25, ProductCategory.Groceries));
            _products.Add(new Product(GenerateId(), "Худи оверсайз", 3200.00m, 0, ProductCategory.Clothing));
            _products.Add(new Product(GenerateId(), "Чистый код (Р. Мартин)", 1850.00m, 7, ProductCategory.Books));
            _products.Add(new Product(GenerateId(), "Механическая клавиатура", 6490.99m, 4, ProductCategory.Electronics));
        }

        public IReadOnlyList<Product> GetAllProducts() => _products.AsReadOnly();

        public Product AddProduct(string name, decimal price, int quantity, ProductCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название товара не может быть пустым.");

            if (price <= 0)
                throw new ArgumentException("Цена должна быть строго больше нуля.");

            if (quantity < 0)
                throw new ArgumentException("Количество не может быть отрицательным.");

            var product = new Product(GenerateId(), name.Trim(), price, quantity, category);
            _products.Add(product);
            return product;
        }

        public bool RemoveProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is null)
                return false;

            return _products.Remove(product);
        }

        public bool RestockProduct(int id, int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Количество поставки должно быть больше нуля.");

            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is null)
                return false;

            product.Quantity += amount;
            return true;
        }

        public bool SellProduct(int id, int amount, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (amount <= 0)
            {
                errorMessage = "Количество для продажи должно быть больше нуля.";
                return false;
            }

            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                errorMessage = $"Товар с кодом {id} не найден.";
                return false;
            }

            if (product.Quantity < amount)
            {
                errorMessage = $"Недостаточно товара на складе. В наличии: {product.Quantity} шт., запрошено: {amount} шт.";
                return false;
            }

            product.Quantity -= amount;
            return true;
        }

        public Product? FindById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public List<Product> FindByName(string namePart)
        {
            if (string.IsNullOrWhiteSpace(namePart))
                return new List<Product>();

            return _products
                .Where(p => p.Name.Contains(namePart.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Product> FindByCategory(ProductCategory category)
        {
            return _products
                .Where(p => p.Category == category)
                .ToList();
        }
    }

    internal class Program
    {
        private static readonly Store Store = new();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== УЧЁТ ТОВАРОВ В МАГАЗИНЕ ===");
                Console.WriteLine("1. Показать все товары");
                Console.WriteLine("2. Добавить товар");
                Console.WriteLine("3. Удалить товар");
                Console.WriteLine("4. Заказать поставку товара");
                Console.WriteLine("5. Продать товар");
                Console.WriteLine("6. Поиск товара");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите действие: ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowAllProducts();
                        break;
                    case "2":
                        AddProductView();
                        break;
                    case "3":
                        RemoveProductView();
                        break;
                    case "4":
                        RestockProductView();
                        break;
                    case "5":
                        SellProductView();
                        break;
                    case "6":
                        SearchProductView();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Работа программы завершена.");
                        continue;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Неизвестный пункт меню. Попробуйте снова.");
                        Console.ResetColor();
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        private static void ShowAllProducts()
        {
            var products = Store.GetAllProducts();
            PrintProductTable(products);
        }

        private static void AddProductView()
        {
            Console.WriteLine("--- Добавление нового товара ---");

            string name;
            while (true)
            {
                Console.Write("Введите название товара: ");
                name = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(name))
                    break;
                Console.WriteLine("Ошибка: название не может быть пустым.");
            }

            decimal price = ReadPositiveDecimal("Введите цену (руб): ");
            int quantity = ReadNonNegativeInt("Введите начальное количество: ");
            ProductCategory category = ChooseCategory();

            try
            {
                var created = Store.AddProduct(name, price, quantity, category);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nТовар успешно добавлен! Присвоен код (ID): {created.Id}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка при добавлении: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void RemoveProductView()
        {
            Console.WriteLine("--- Удаление товара ---");
            int id = ReadPositiveInt("Введите код (ID) товара для удаления: ");

            bool removed = Store.RemoveProduct(id);
            if (removed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Товар с кодом {id} успешно удалён.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Товар с кодом {id} не найден.");
            }
            Console.ResetColor();
        }

        private static void RestockProductView()
        {
            Console.WriteLine("--- Заказ поставки ---");
            int id = ReadPositiveInt("Введите код (ID) товара: ");
            int amount = ReadPositiveInt("Введите количество для поставки: ");

            bool updated = Store.RestockProduct(id, amount);
            if (updated)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Поставка принята. Остаток обновлён.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Товар с кодом {id} не найден.");
            }
            Console.ResetColor();
        }

        private static void SellProductView()
        {
            Console.WriteLine("--- Продажа товара ---");
            int id = ReadPositiveInt("Введите код (ID) товара: ");
            int amount = ReadPositiveInt("Введите количество для продажи: ");

            if (Store.SellProduct(id, amount, out string errorMessage))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Продажа оформлена. Списано {amount} шт.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка продажи: {errorMessage}");
            }
            Console.ResetColor();
        }

        private static void SearchProductView()
        {
            Console.WriteLine("--- Поиск товаров ---");
            Console.WriteLine("1. По коду (ID)");
            Console.WriteLine("2. По названию");
            Console.WriteLine("3. По категории");
            Console.Write("Выберите критерий поиска: ");

            string? criteria = Console.ReadLine();
            List<Product> results = new();

            switch (criteria)
            {
                case "1":
                    int id = ReadPositiveInt("Введите код товара: ");
                    var product = Store.FindById(id);
                    if (product is not null)
                        results.Add(product);
                    break;
                case "2":
                    Console.Write("Введите фрагмент названия: ");
                    string term = Console.ReadLine() ?? string.Empty;
                    results = Store.FindByName(term);
                    break;
                case "3":
                    var category = ChooseCategory();
                    results = Store.FindByCategory(category);
                    break;
                default:
                    Console.WriteLine("Неверный критерий поиска.");
                    return;
            }

            Console.WriteLine($"\nРезультаты поиска (найдено: {results.Count}):");
            PrintProductTable(results);
        }

        private static void PrintProductTable(IReadOnlyList<Product> products)
        {
            if (products.Count == 0)
            {
                Console.WriteLine("Список пуст.");
                return;
            }

            Console.WriteLine(new string('-', 85));
            Console.WriteLine($"{"ID",-5} | {"Название",-28} | {"Категория",-15} | {"Цена",-12} | {"На складе",-15}");
            Console.WriteLine(new string('-', 85));

            foreach (var p in products)
            {
                string stockText = p.IsInStock ? $"{p.Quantity} шт." : "Нет";
                Console.WriteLine($"{p.Id,-5} | {p.Name,-28} | {p.Category,-15} | {p.Price,10:F2} ₽ | {stockText,-15}");
            }
            Console.WriteLine(new string('-', 85));
        }

        private static decimal ReadPositiveDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out decimal value) && value > 0)
                    return value;
                Console.WriteLine("Ошибка: введите положительное число.");
            }
        }

        private static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value) && value > 0)
                    return value;
                Console.WriteLine("Ошибка: число должно быть целым и больше нуля.");
            }
        }

        private static int ReadNonNegativeInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value) && value >= 0)
                    return value;
                Console.WriteLine("Ошибка: количество не может быть отрицательным.");
            }
        }

        private static ProductCategory ChooseCategory()
        {
            var categories = (ProductCategory[])Enum.GetValues(typeof(ProductCategory));
            Console.WriteLine("Выберите категорию:");
            for (int i = 0; i < categories.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }

            while (true)
            {
                Console.Write("Номер категории: ");
                if (int.TryParse(Console.ReadLine(), out int num) && num >= 1 && num <= categories.Length)
                {
                    return categories[num - 1];
                }
                Console.WriteLine("Ошибка: выберите существующий номер категории.");
            }
        }
    }
}