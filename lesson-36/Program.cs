using System.Text;
using Lesson36;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("=== Блокнот ===");
    Console.WriteLine("1. Открыть файл");
    Console.WriteLine("2. Изменить файл");
    Console.WriteLine("3. Создать файл");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите пункт: ");

    string? choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                OpenFile();
                break;
            case "2":
                EditFile();
                break;
            case "3":
                CreateFile();
                break;
            case "0":
                running = false;
                break;
            default:
                Console.WriteLine("Нет такого пункта меню.");
                break;
        }
    }
    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

void OpenFile()
{
    string path = ReadPath("Путь к файлу: ");
    string content = FileManager.Read(path);

    Console.WriteLine();
    Console.WriteLine($"--- {Path.GetFileName(path)} ---");
    Console.WriteLine(content);
    Console.WriteLine("--- конец файла ---");
}

void CreateFile()
{
    Console.Write("Имя файла: ");
    string fileName = (Console.ReadLine() ?? string.Empty).Trim();

    if (fileName.Length == 0)
    {
        Console.WriteLine("Имя файла не может быть пустым.");
        return;
    }

    Console.Write($"Папка (Enter - {FileManager.DefaultFolder}): ");
    string folder = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');

    if (folder.Length == 0)
    {
        folder = FileManager.DefaultFolder;
    }

    string content = ReadText();
    string fullPath = FileManager.Create(folder, fileName, content);
    Console.WriteLine($"Файл создан: {fullPath}");
}

void EditFile()
{
    string path = ReadPath("Путь к файлу: ");
    Console.WriteLine();
    Console.WriteLine(FileManager.Read(path));

    bool editing = true;

    while (editing)
    {
        Console.WriteLine();
        Console.WriteLine($"Файл: {path}");
        Console.WriteLine("1. Дописать текст в конец");
        Console.WriteLine("2. Заменить весь текст");
        Console.WriteLine("3. Переименовать файл");
        Console.WriteLine("4. Переместить файл в другую папку");
        Console.WriteLine("0. Назад");
        Console.Write("Выберите пункт: ");

        switch (Console.ReadLine())
        {
            case "1":
                FileManager.Append(path, ReadText());
                Console.WriteLine("Текст дописан.");
                break;
            case "2":
                FileManager.Overwrite(path, ReadText());
                Console.WriteLine("Текст заменён.");
                break;
            case "3":
                Console.Write("Новое имя файла: ");
                string newName = (Console.ReadLine() ?? string.Empty).Trim();
                if (newName.Length == 0)
                {
                    Console.WriteLine("Имя файла не может быть пустым.");
                    break;
                }
                path = FileManager.Rename(path, newName);
                Console.WriteLine($"Файл переименован: {path}");
                break;
            case "4":
                Console.Write("Новая папка: ");
                string newFolder = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');
                if (newFolder.Length == 0)
                {
                    Console.WriteLine("Папка не может быть пустой.");
                    break;
                }
                path = FileManager.Move(path, newFolder);
                Console.WriteLine($"Файл перемещён: {path}");
                break;
            case "0":
                editing = false;
                break;
            default:
                Console.WriteLine("Нет такого пункта меню.");
                break;
        }
    }
}

string ReadPath(string prompt)
{
    Console.Write(prompt);
    return (Console.ReadLine() ?? string.Empty).Trim().Trim('"');
}

string ReadText()
{
    Console.WriteLine("Введите текст. Строка /end завершает ввод.");
    List<string> lines = new List<string>();

    while (true)
    {
        string line = Console.ReadLine() ?? string.Empty;

        if (line.Trim() == "/end")
        {
            break;
        }

        lines.Add(line);
    }

    return string.Join(Environment.NewLine, lines);
}
