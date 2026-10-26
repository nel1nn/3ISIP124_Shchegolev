using System.Text;
using Lesson30;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

OrderManager manager = OrderManager.Instance;
Console.WriteLine($"Единственный экземпляр OrderManager: {ReferenceEquals(manager, OrderManager.Instance)}");

while (true)
{
    Console.WriteLine();
    Console.Write("Какой напиток приготовить? Кофе, Чай, Смузи (или \"выход\"): ");
    string choice = Console.ReadLine() ?? string.Empty;

    if (choice.Trim().ToLower() == "выход")
    {
        break;
    }

    Drink? drink = DrinkFactory.CreateDrink(choice);

    if (drink == null)
    {
        Console.WriteLine("Неизвестный напиток!");
        continue;
    }

    drink.Prepare();
    drink.Serve();
    Console.WriteLine($"К оплате: {drink.Price:N2} руб.");
    manager.AddOrder(drink.Price);
}

Console.WriteLine();
OrderManager.Instance.PrintStatistics();
