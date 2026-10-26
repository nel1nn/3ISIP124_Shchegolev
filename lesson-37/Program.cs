using System.Text;
using Lesson37;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string filePath = Path.GetFullPath("AsyncData.txt");

using UserContext context = new UserContext();
await context.Database.EnsureCreatedAsync();
UserService userService = new UserService(context);

bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("=== Асинхронная работа с файлом и БД ===");
    Console.WriteLine("1. Записать текст в файл");
    Console.WriteLine("2. Прочитать файл");
    Console.WriteLine("3. Показать пользователей из БД");
    Console.WriteLine("4. Добавить пользователя в БД");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите пункт: ");

    try
    {
        switch (Console.ReadLine())
        {
            case "1":
                await WriteFileAsync();
                break;
            case "2":
                await ReadFileAsync();
                break;
            case "3":
                await ShowUsersAsync();
                break;
            case "4":
                await AddUserAsync();
                break;
            case "0":
                running = false;
                break;
            default:
                Console.WriteLine("Нет такого пункта меню.");
                break;
        }
    }
    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
    {
        Console.WriteLine($"Ошибка работы с файлом: {ex.Message}");
    }
}

async Task WriteFileAsync()
{
    Console.Write("Введите текст для записи: ");
    string text = Console.ReadLine() ?? string.Empty;

    Console.WriteLine("Запись начата...");
    await FileService.WriteAsync(filePath, text);
    Console.WriteLine($"Текст записан в файл: {filePath}");
}

async Task ReadFileAsync()
{
    Console.WriteLine("Чтение начато...");
    string text = await FileService.ReadAsync(filePath);
    Console.WriteLine("Содержимое файла:");
    Console.WriteLine(text);
}

async Task ShowUsersAsync()
{
    Console.WriteLine("Получение данных из БД...");
    List<User> users = await userService.GetAllAsync();

    if (users.Count == 0)
    {
        Console.WriteLine("В базе пока нет пользователей.");
        return;
    }

    foreach (User user in users)
    {
        Console.WriteLine($"{user.Id}. {user.FullName}, {user.Age} лет");
    }
}

async Task AddUserAsync()
{
    Console.Write("ФИО: ");
    string fullName = (Console.ReadLine() ?? string.Empty).Trim();

    if (fullName.Length == 0)
    {
        Console.WriteLine("ФИО не может быть пустым.");
        return;
    }

    Console.Write("Возраст: ");

    if (!int.TryParse(Console.ReadLine(), out int age) || age < 0 || age > 150)
    {
        Console.WriteLine("Возраст должен быть целым числом от 0 до 150.");
        return;
    }

    Console.WriteLine("Сохранение в БД...");
    User newUser = await userService.AddAsync(fullName, age);
    Console.WriteLine($"Пользователь сохранён, номер: {newUser.Id}.");
}
