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

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }


}
