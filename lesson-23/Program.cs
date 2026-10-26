using System.Text;
using Lesson23;

Console.OutputEncoding = Encoding.UTF8;

var users = new List<User>
{
    new User("Иванов Иван Иванович", "ivanovii", new DateTime(1990, 5, 18), new List<string> { "Чтение", "Путешествия", "Программирование" }),
    new User("Петров Петр Петрович", "petrovpp", new DateTime(1985, 10, 2), new List<string> { "Футбол", "Кулинария" }),
    new User("Сидорова Анна Михайловна", "sidorovana", new DateTime(2000, 3, 15), new List<string> { "Рисование", "Программирование" }),
    new User("Смирнов Алексей Николаевич", "smirnovan", new DateTime(1995, 7, 30), new List<string> { "Хоккей", "Фотография" }),
    new User("Кузнецова Ольга Валерьевна", "kuznetsovao", new DateTime(1992, 12, 12), new List<string> { "Йога", "Путешествия" })
};

DateTime today = DateTime.Today;

void PrintUsers(IEnumerable<User> list)
{
    foreach (var user in list)
    {
        Console.WriteLine($"{user.FullName} ({user.Login}), возраст: {user.GetAge(today)}");
    }
}

Console.WriteLine("1. Пользователи старше 30 лет:");
var olderThan30 = users.Where(u => u.GetAge(today) > 30).ToList();
PrintUsers(olderThan30);
Console.WriteLine();

Console.WriteLine("2. Логины в алфавитном порядке:");
var sortedLogins = users.Select(u => u.Login).OrderBy(login => login).ToList();
foreach (var login in sortedLogins)
{
    Console.WriteLine(login);
}
Console.WriteLine();

Console.WriteLine("3. Пользователи с хобби \"Программирование\":");
var programmers = users.Where(u => u.Hobbies.Contains("Программирование")).ToList();
PrintUsers(programmers);
Console.WriteLine();

Console.WriteLine("4. Группировка по году рождения (год -> количество):");
var byBirthYear = users.GroupBy(u => u.DateOfBirth.Year).OrderBy(g => g.Key);
foreach (var group in byBirthYear)
{
    Console.WriteLine($"{group.Key} -> {group.Count()}");
}
Console.WriteLine();

Console.WriteLine("5. Словарь login -> FullName:");
Dictionary<string, string> loginToFullName = users.ToDictionary(u => u.Login, u => u.FullName);
foreach (var pair in loginToFullName)
{
    Console.WriteLine($"{pair.Key} -> {pair.Value}");
}
Console.WriteLine();

Console.WriteLine("6. Самый молодой и самый старый пользователь:");
var youngest = users.OrderByDescending(u => u.DateOfBirth).First();
var oldest = users.OrderBy(u => u.DateOfBirth).First();
Console.WriteLine($"Самый молодой: {youngest.FullName}, родился {youngest.DateOfBirth:dd.MM.yyyy}, возраст: {youngest.GetAge(today)}");
Console.WriteLine($"Самый старый: {oldest.FullName}, родился {oldest.DateOfBirth:dd.MM.yyyy}, возраст: {oldest.GetAge(today)}");
Console.WriteLine();

Console.WriteLine("7. Уникальные хобби:");
var uniqueHobbies = users.SelectMany(u => u.Hobbies).Distinct().ToList();
Console.WriteLine(string.Join(", ", uniqueHobbies));
Console.WriteLine();

Console.WriteLine("8. Количество пользователей, увлекающихся \"Путешествиями\":");
int travelersCount = users.Count(u => u.Hobbies.Contains("Путешествия"));
Console.WriteLine(travelersCount);
Console.WriteLine();

Console.WriteLine("9. Пользователи, чьи ФИО начинаются на \"С\":");
var startsWithS = users.Where(u => u.FullName.StartsWith("С", StringComparison.OrdinalIgnoreCase)).ToList();
PrintUsers(startsWithS);
Console.WriteLine();

Console.WriteLine("10. Два пользователя с наибольшим числом хобби:");
var topByHobbies = users
    .OrderByDescending(u => u.Hobbies.Count)
    .ThenBy(u => u.Login)
    .Take(2)
    .ToList();
foreach (var user in topByHobbies)
{
    Console.WriteLine($"{user.FullName} ({user.Login}), хобби: {user.Hobbies.Count}");
}
