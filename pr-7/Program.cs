using System.Text;
using Marketplace.Data;
using Marketplace.Models;
using Marketplace.Services;

namespace Marketplace;

internal class Program
{
    // Текущий вошедший пользователь. null означает гостя
    private static User? _currentUser;

    private static MarketplaceContext _context = null!;
    private static AuthService _authService = null!;
    private static CartService _cartService = null!;
    private static OrderService _orderService = null!;

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        _context = new MarketplaceContext();
        // Создаёт файл базы и таблицы при первом запуске, заполняет начальными данными
        _context.Database.EnsureCreated();

        _authService = new AuthService(_context);
        _cartService = new CartService(_context);
        _orderService = new OrderService(_context);

        bool running = true;
        while (running)
        {
            running = _currentUser == null ? ShowGuestMenu() : ShowUserMenu();
        }

        Console.WriteLine("До свидания!");
    }

    // Меню гостя. Возвращает false, если нужно выйти из программы
    private static bool ShowGuestMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== GMWOG||GG.MOW||WONGG: маркетплейс ===");
        Console.WriteLine("1. Посмотреть товары");
        Console.WriteLine("2. Регистрация");
        Console.WriteLine("3. Войти");
        Console.WriteLine("0. Выход");

        switch (ReadLine("Выберите пункт: "))
        {
            case "1": ShowProducts(); break;
            case "2": Register(); break;
            case "3": Login(); break;
            case "0": return false;
            default: Console.WriteLine("Нет такого пункта меню."); break;
        }

        return true;
    }

    // Меню авторизованного пользователя
    private static bool ShowUserMenu()
    {
        Console.WriteLine();
        Console.WriteLine($"=== Вы вошли как {_currentUser!.Nickname} ===");
        Console.WriteLine("1. Посмотреть товары");
        Console.WriteLine("2. Добавить товар в корзину");
        Console.WriteLine("3. Моя корзина");
        Console.WriteLine("4. Удалить товар из корзины");
        Console.WriteLine("5. Купить один товар");
        Console.WriteLine("6. Купить одну позицию из корзины");
        Console.WriteLine("7. Купить всю корзину");
        Console.WriteLine("8. Мои заказы");
        Console.WriteLine("9. Выйти из аккаунта");
        Console.WriteLine("0. Выход из программы");

        switch (ReadLine("Выберите пункт: "))
        {
            case "1": ShowProducts(); break;
            case "2": AddToCart(); break;
            case "3": ShowCart(); break;
            case "4": RemoveFromCart(); break;
            case "5": BuySingleProduct(); break;
            case "6": BuyCartItem(); break;
            case "7": BuyWholeCart(); break;
            case "8": ShowOrders(); break;
            case "9":
                _currentUser = null;
                Console.WriteLine("Вы вышли из аккаунта.");
                break;
            case "0": return false;
            default: Console.WriteLine("Нет такого пункта меню."); break;
        }

        return true;
    }

    // ---------- Регистрация и вход ----------

    private static void Register()
    {
        Console.WriteLine("--- Регистрация ---");
        string nickname = ReadLine("Ник: ");
        string password = ReadLine("Пароль: ");
        string passwordConfirm = ReadLine("Повторите пароль: ");

        var (user, error) = _authService.Register(nickname, password, passwordConfirm);

        if (user == null)
        {
            Console.WriteLine($"Ошибка: {error}");
            return;
        }

        _currentUser = user;
        Console.WriteLine("Регистрация прошла успешно, вы вошли в аккаунт.");
    }

    private static void Login()
    {
        Console.WriteLine("--- Вход ---");
        string nickname = ReadLine("Ник: ");
        string password = ReadLine("Пароль: ");

        User? user = _authService.Login(nickname, password);

        if (user == null)
        {
            Console.WriteLine("Неверный ник или пароль.");
            return;
        }

        _currentUser = user;
        Console.WriteLine("Вы успешно вошли в аккаунт.");
    }

    // ---------- Товары и корзина ----------

    private static void ShowProducts()
    {
        Console.WriteLine("--- Каталог товаров ---");

        foreach (Product product in _orderService.GetProducts())
        {
            Console.WriteLine(
                $"{product.Id}. {product.Name} [{product.Category?.Name}] - {product.Price:N2} руб.");
            Console.WriteLine($"   {product.Description}");
        }
    }

    private static void AddToCart()
    {
        ShowProducts();
        int? productId = ReadInt("Номер товара: ");
        int? quantity = ReadInt("Количество: ");

        if (productId == null || quantity == null)
            return;

        string? error = _cartService.Add(_currentUser!.Id, productId.Value, quantity.Value);
        Console.WriteLine(error == null ? "Товар добавлен в корзину." : $"Ошибка: {error}");
    }

    private static void ShowCart()
    {
        List<CartItem> items = _cartService.GetItems(_currentUser!.Id);

        if (items.Count == 0)
        {
            Console.WriteLine("Корзина пуста.");
            return;
        }

        Console.WriteLine("--- Ваша корзина ---");
        decimal total = 0;

        foreach (CartItem item in items)
        {
            decimal sum = item.Product!.Price * item.Quantity;
            total += sum;
            Console.WriteLine(
                $"{item.ProductId}. {item.Product.Name} x{item.Quantity} = {sum:N2} руб.");
        }

        Console.WriteLine($"Итого: {total:N2} руб.");
    }

    private static void RemoveFromCart()
    {
        ShowCart();
        int? productId = ReadInt("Номер товара для удаления: ");

        if (productId == null)
            return;

        bool removed = _cartService.Remove(_currentUser!.Id, productId.Value);
        Console.WriteLine(removed ? "Товар удалён из корзины." : "Этого товара нет в корзине.");
    }

    // ---------- Покупки ----------

    // Показывает ПВЗ и возвращает выбранный номер (null, если ввод неверный)
    private static int? ChoosePickupPoint()
    {
        Console.WriteLine("--- Выберите пункт выдачи ---");

        foreach (PickupPoint point in _orderService.GetPickupPoints())
            Console.WriteLine($"{point.Id}. {point.Name} ({point.Address})");

        return ReadInt("Номер ПВЗ: ");
    }

    private static void BuySingleProduct()
    {
        ShowProducts();
        int? productId = ReadInt("Номер товара: ");
        int? quantity = ReadInt("Количество: ");

        if (productId == null || quantity == null)
            return;

        int? pickupPointId = ChoosePickupPoint();
        if (pickupPointId == null)
            return;

        var (order, error) = _orderService.BuySingleProduct(
            _currentUser!.Id, productId.Value, quantity.Value, pickupPointId.Value);

        PrintOrderResult(order, error);
    }

    private static void BuyCartItem()
    {
        ShowCart();
        int? productId = ReadInt("Номер товара из корзины: ");

        if (productId == null)
            return;

        int? pickupPointId = ChoosePickupPoint();
        if (pickupPointId == null)
            return;

        var (order, error) = _orderService.BuyCartItem(
            _currentUser!.Id, productId.Value, pickupPointId.Value);

        PrintOrderResult(order, error);
    }

    private static void BuyWholeCart()
    {
        ShowCart();

        int? pickupPointId = ChoosePickupPoint();
        if (pickupPointId == null)
            return;

        var (order, error) = _orderService.BuyWholeCart(_currentUser!.Id, pickupPointId.Value);
        PrintOrderResult(order, error);
    }

    private static void PrintOrderResult(Order? order, string? error)
    {
        Console.WriteLine(order != null
            ? $"Заказ №{order.Id} оформлен. Заберите его в выбранном пункте выдачи."
            : $"Ошибка: {error}");
    }

    // ---------- История заказов ----------

    private static void ShowOrders()
    {
        Console.WriteLine("Сортировка по дате: 1 - сначала новые, 2 - сначала старые");
        string choice = ReadLine("Выберите: ");

        if (choice != "1" && choice != "2")
        {
            Console.WriteLine("Нет такого варианта.");
            return;
        }

        List<Order> orders = _orderService.GetOrders(_currentUser!.Id, newestFirst: choice == "1");

        if (orders.Count == 0)
        {
            Console.WriteLine("У вас пока нет заказов.");
            return;
        }

        foreach (Order order in orders)
        {
            Console.WriteLine();
            Console.WriteLine($"Заказ №{order.Id} от {order.CreatedAt:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"ПВЗ: {order.PickupPoint?.Name} ({order.PickupPoint?.Address})");

            decimal total = 0;
            foreach (OrderProduct line in order.OrderProducts)
            {
                decimal sum = line.PriceAtPurchase * line.Quantity;
                total += sum;
                Console.WriteLine($"  {line.Product?.Name} x{line.Quantity} = {sum:N2} руб.");
            }

            Console.WriteLine($"  Итого: {total:N2} руб.");
        }
    }

    // ---------- Вспомогательные методы ввода ----------

    // Читает строку, никогда не возвращает null
    private static string ReadLine(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    // Читает целое число. При неверном вводе пишет сообщение и возвращает null
    private static int? ReadInt(string prompt)
    {
        string input = ReadLine(prompt);

        if (int.TryParse(input, out int value))
            return value;

        Console.WriteLine("Нужно ввести целое число.");
        return null;
    }
}
