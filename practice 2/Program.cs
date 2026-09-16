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

        // Метод генерации следующего ID (гарантирует старт с 1 и уникальность)
        private int GenerateId() => _nextId++;

        private void SeedInitialData()
        {
            _products.Add(new Product(GenerateId(), "Наушники Sony", 8990.00m, 12, ProductCategory.Electronics));
            _products.Add(new Product(GenerateId(), "Кофе в зернах 1кг", 1450.50m, 25, ProductCategory.Groceries));
            _products.Add(new Product(GenerateId(), "Худи оверсайз", 3200.00m, 0, ProductCategory.Clothing));
            _products.Add(new Product(GenerateId(), "Чистый код (Р. Мартин)", 1850.00m, 7, ProductCategory.Books));
            _products.Add(new Product(GenerateId(), "Механическая клавиатура", 6490.99m, 4, ProductCategory.Electronics));
        }

        // Возвращаем копию списка для безопасного чтения
        public IReadOnlyList<Product> GetAllProducts() => _products.AsReadOnly();
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }


}
