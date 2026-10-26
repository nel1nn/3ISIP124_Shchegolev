namespace Lesson30;

public class OrderManager
{
    private static readonly OrderManager _instance = new OrderManager();

    private OrderManager()
    {
    }

    public static OrderManager Instance
    {
        get { return _instance; }
    }

    public decimal TotalRevenue { get; private set; }

    public int ServedClientsCount { get; private set; }

    public void AddOrder(decimal amount)
    {
        TotalRevenue += amount;
        ServedClientsCount++;
    }

    public void PrintStatistics()
    {
        Console.WriteLine($"Общая выручка: {TotalRevenue:N2} руб.");
        Console.WriteLine($"Обслужено клиентов: {ServedClientsCount}");
    }
}
