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

        // Поиск по точному коду (ID)
        public Product? FindById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        // Поиск по названию (частичное совпадение без учета регистра)
        public List<Product> FindByName(string namePart)
        {
            if (string.IsNullOrWhiteSpace(namePart))
                return new List<Product>();

            return _products
                .Where(p => p.Name.Contains(namePart.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Поиск по категории
        public List<Product> FindByCategory(ProductCategory category)
        {
            return _products
                .Where(p => p.Category == category)
                .ToList();
        }

    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }


}
