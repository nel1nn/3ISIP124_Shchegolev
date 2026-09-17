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

        public string GetCategoryDisplayName()
        {
            return Category switch
            {
                ProductCategory.Electronics => "Электроника",
                ProductCategory.Groceries => "Продукты",
                ProductCategory.Clothing => "Одежда",
                ProductCategory.Books => "Книги",
                _ => Category.ToString()
            };
        }

        public override string ToString()
        {
            string stock = IsInStock ? "да" : "нет";
            return $"{Id}) {Name} {Quantity}шт - {Price:0.##}р | {GetCategoryDisplayName()} | На складе: {stock}";
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
            _products.Add(new Product(GenerateId(), "Наушники Sony", 8990m, 12, ProductCategory.Electronics));
            _products.Add(new Product(GenerateId(), "Кофе в зернах 1кг", 1450m, 25, ProductCategory.Groceries));
            _products.Add(new Product(GenerateId(), "Худи оверсайз", 3200m, 0, ProductCategory.Clothing));
            _products.Add(new Product(GenerateId(), "Чистый код (Р. Мартин)", 1850m, 7, ProductCategory.Books));
            _products.Add(new Product(GenerateId(), "Механическая клавиатура", 6490m, 4, ProductCategory.Electronics));
        }

        public IReadOnlyList<Product> GetAllProducts() => _products.AsReadOnly();

        public Product AddProduct(string name, decimal price, int quantity, ProductCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название товара не может быть пустым.");

            if (price <= 0)
                throw new ArgumentException("Цена должна быть больше нуля.");

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
                errorMessage = $"Недостаточно на складе. В наличии: {product.Quantity}шт, запрошено: {amount}шт.";
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
                Console.WriteLine("Учет товаров в магазине");
                Console.WriteLine("-----------------------");
                Console.WriteLine("1. Показать все товары");
                Console.WriteLine("2. Добавить товар");
                Console.WriteLine("3. Удалить товар");
                Console.WriteLine("4. Заказать поставку");
                Console.WriteLine("5. Продать товар");
                Console.WriteLine("6. Поиск товара");
                Console.WriteLine("0. Выход");
                Console.WriteLine("-----------------------");
                Console.Write("Действие: ");

                string? choice = Console.ReadLine();

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
                        continue;
                    default:
                        Console.WriteLine("\nНеверный пункт.");
                        break;
                }

                Pause();
            }
        }

        private static void ShowAllProducts()
        {
            Console.Clear();
            Console.WriteLine("Список товаров:");
            Console.WriteLine();
            PrintProducts(Store.GetAllProducts());
        }

        private static void AddProductView()
        {
            Console.Clear();
            Console.WriteLine("Добавление товара");
            Console.WriteLine("-----------------");

            string name;
            while (true)
            {
                Console.Write("Название: ");
                name = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(name))
                    break;
                Console.WriteLine("Название не может быть пустым.");
            }

            decimal price = ReadPositiveDecimal("Цена (руб): ");
            int quantity = ReadNonNegativeInt("Количество: ");
            ProductCategory category = ChooseCategory();

            try
            {
                var created = Store.AddProduct(name, price, quantity, category);
                Console.WriteLine($"\nУспешно добавлен:\n{created}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка: {ex.Message}");
            }
        }

        private static void RemoveProductView()
        {
            Console.Clear();
            Console.WriteLine("Удаление товара");
            Console.WriteLine("---------------");

            int id = ReadPositiveInt("Код товара: ");
            if (Store.RemoveProduct(id))
                Console.WriteLine($"\nТовар {id} удален.");
            else
                Console.WriteLine($"\nТовар {id} не найден.");
        }

        private static void RestockProductView()
        {
            Console.Clear();
            Console.WriteLine("Поставка товара");
            Console.WriteLine("---------------");

            int id = ReadPositiveInt("Код товара: ");
            int amount = ReadPositiveInt("Количество поставки: ");

            if (Store.RestockProduct(id, amount))
                Console.WriteLine($"\nПоставка принята. Остаток обновлен.");
            else
                Console.WriteLine($"\nТовар с кодом {id} не найден.");
        }

        private static void SellProductView()
        {
            Console.Clear();
            Console.WriteLine("Продажа товара");
            Console.WriteLine("--------------");

            int id = ReadPositiveInt("Код товара: ");
            int amount = ReadPositiveInt("Количество для продажи: ");

            if (Store.SellProduct(id, amount, out string errorMessage))
                Console.WriteLine($"\nПродажа выполнена: списано {amount}шт.");
            else
                Console.WriteLine($"\nОшибка: {errorMessage}");
        }

        private static void SearchProductView()
        {
            Console.Clear();
            Console.WriteLine("Поиск товара");
            Console.WriteLine("------------");
            Console.WriteLine("1. По коду");
            Console.WriteLine("2. По названию");
            Console.WriteLine("3. По категории");
            Console.Write("Критерий: ");

            string? criteria = Console.ReadLine();
            List<Product> results = new();

            switch (criteria)
            {
                case "1":
                    int id = ReadPositiveInt("Код: ");
                    var product = Store.FindById(id);
                    if (product is not null)
                        results.Add(product);
                    break;
                case "2":
                    Console.Write("Фрагмент названия: ");
                    string term = Console.ReadLine() ?? string.Empty;
                    results = Store.FindByName(term);
                    break;
                case "3":
                    var category = ChooseCategory();
                    results = Store.FindByCategory(category);
                    break;
                default:
                    Console.WriteLine("\nНеверный пункт.");
                    return;
            }

            Console.WriteLine("\nРезультаты поиска:");
            PrintProducts(results);
        }

        private static void PrintProducts(IReadOnlyList<Product> products)
        {
            if (products.Count == 0)
            {
                Console.WriteLine("Товары не найдены.");
                return;
            }

            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        private static void Pause()
        {
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey(true);
        }

        private static decimal ReadPositiveDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out decimal value) && value > 0)
                    return value;
                Console.WriteLine("Введите число больше 0.");
            }
        }

        private static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value) && value > 0)
                    return value;
                Console.WriteLine("Введите целое число больше 0.");
            }
        }

        private static int ReadNonNegativeInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value) && value >= 0)
                    return value;
                Console.WriteLine("Количество не может быть меньше 0.");
            }
        }

        private static ProductCategory ChooseCategory()
        {
            var categories = (ProductCategory[])Enum.GetValues(typeof(ProductCategory));
            Console.WriteLine("Категории:");
            for (int i = 0; i < categories.Length; i++)
            {
                string name = categories[i] switch
                {
                    ProductCategory.Electronics => "Электроника",
                    ProductCategory.Groceries => "Продукты",
                    ProductCategory.Clothing => "Одежда",
                    ProductCategory.Books => "Книги",
                    _ => categories[i].ToString()
                };
                Console.WriteLine($"{i + 1}. {name}");
            }

            while (true)
            {
                Console.Write("Выберите категорию (номер): ");
                if (int.TryParse(Console.ReadLine(), out int num) && num >= 1 && num <= categories.Length)
                {
                    return categories[num - 1];
                }
                Console.WriteLine("Неверный номер.");
            }
        }
    }
}