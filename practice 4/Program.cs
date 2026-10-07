using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement
{
    public enum Genre
    {
        Classic = 1,
        ScienceFiction = 2,
        Fantasy = 3,
        Detective = 4,
        Historical = 5
    }

    public class Book
    {
        public int Id { get; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Genre Genre { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }

        public Book(int id, string title, string author, Genre genre, int year, decimal price)
        {
            Id = id;
            Title = title;
            Author = author;
            Genre = genre;
            Year = year;
            Price = price;
        }

        public override string ToString()
        {
            return $"[ID: {Id}] \"{Title}\" | Автор: {Author} | Жанр: {GetGenreName(Genre)} | Год: {Year} | Цена: {Price}";
        }

        public static string GetGenreName(Genre genre) => genre switch
        {
            Genre.Classic => "Классика",
            Genre.ScienceFiction => "Научная фантастика",
            Genre.Fantasy => "Фэнтези",
            Genre.Detective => "Детектив",
            Genre.Historical => "Исторический роман",
            _ => genre.ToString()
        };
    }

    public class LibraryManager
    {
        private List<Book> booksList = new();
        private int _currentIdCounter = 1;

        public LibraryManager()
        {
            SeedInitialData();
        }

        private void SeedInitialData()
        {
            AddBookInternal("Война и мир", "Лев Толстой", Genre.Classic, 1869, 1250.00m);
            AddBookInternal("Преступление и наказание", "Фёдор Достоевский", Genre.Classic, 1866, 890.50m);
            AddBookInternal("Дюна", "Фрэнк Герберт", Genre.ScienceFiction, 1965, 1450.00m);
            AddBookInternal("Властелин колец", "Дж. Р. Р. Толкин", Genre.Fantasy, 1954, 2199.90m);
            AddBookInternal("Убийство в Восточном экспрессе", "Агата Кристи", Genre.Detective, 1934, 670.00m);
        }

        private void AddBookInternal(string title, string author, Genre genre, int year, decimal price)
        {
            var book = new Book(_currentIdCounter++, title, author, genre, year, price);
            booksList.Add(book);
        }

        public Book AddBook(string title, string author, Genre genre, int year, decimal price)
        {
            var book = new Book(_currentIdCounter++, title, author, genre, year, price);
            booksList.Add(book);
            return book;
        }

        public bool RemoveBookById(int id)
        {
            var bookToRemove = booksList.FirstOrDefault(b => b.Id == id);
            if (bookToRemove == null) return false;

            return booksList.Remove(bookToRemove);
        }

        public IReadOnlyList<Book> GetAll() => booksList.AsReadOnly();

        public List<Book> FindByTitle(string titleQuery) =>
            booksList.Where(b => b.Title.Contains(titleQuery, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<Book> FindByAuthor(string authorQuery) =>
            booksList.Where(b => b.Author.Contains(authorQuery, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<Book> FindByGenre(Genre genre) =>
            booksList.Where(b => b.Genre == genre).ToList();

        public List<Book> SortByTitle() =>
            booksList.OrderBy(b => b.Title).ToList();

        public List<Book> SortByYear() =>
            booksList.OrderBy(b => b.Year).ToList();

        public (Book? Cheapest, Book? MostExpensive) GetPriceExtremes()
        {
            if (!booksList.Any()) return (null, null);

            var cheapest = booksList.MinBy(b => b.Price);
            var mostExpensive = booksList.MaxBy(b => b.Price);
            return (cheapest, mostExpensive);
        }

        public IEnumerable<(string Author, int Count)> GroupByAuthor() =>
            booksList.GroupBy(b => b.Author)
                  .Select(g => (Author: g.Key, Count: g.Count()))
                  .OrderByDescending(g => g.Count);
    }

    public static class Program
    {
        private static LibraryManager Library = new();

        public static void Main()
        {
            while (true)
            {
                DisplayMainMenu();
                string choice = Console.ReadLine()?.Trim() ?? "";

                Console.Clear();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            ShowAllBooks();
                            break;
                        case "2":
                            AddNewBookCommand();
                            break;
                        case "3":
                            DeleteBookCommand();
                            break;
                        case "4":
                            SearchBooksCommand();
                            break;
                        case "5":
                            SortBooksCommand();
                            break;
                        case "6":
                            ShowPriceExtremesCommand();
                            break;
                        case "7":
                            ShowAuthorGroupStatsCommand();
                            break;
                        case "0":
                            Console.WriteLine("Работа программы завершена.");
                            return;
                        default:
                            PrintWarning("Неверный пункт меню. Попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    PrintError($"Произошла непредвиденная ошибка: {ex.Message}");
                }

                Console.WriteLine("\nНажмите любую клавишу для возврата в главное меню...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void DisplayMainMenu()
        {
            PrintHeader("Главное меню");
            Console.ResetColor();
            Console.WriteLine("1. Показать все книги");
            Console.WriteLine("2. Добавить книгу");
            Console.WriteLine("3. Удалить книгу по ID");
            Console.WriteLine("4. Найти книги (по названию, автору или жанру)");
            Console.WriteLine("5. Отсортировать книги (по названию или году)");
            Console.WriteLine("6. Вывести самую дорогую и самую дешёвую книгу");
            Console.WriteLine("7. Сгруппировать книги по авторам (статистика)");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие (0-7): ");
        }

        private static void ShowAllBooks()
        {
            PrintHeader("Список всех книг");
            var books = Library.GetAll();
            PrintBookList(books);
        }

        private static void AddNewBookCommand()
        {
            PrintHeader("ДОБАВЛЕНИЕ НОВОЙ КНИГИ");

            string title = ReadNonEmptyString("Введите название книги: ");
            string author = ReadNonEmptyString("Введите автора: ");
            Genre genre = ReadGenre();
            int year = ReadInt("Введите год издания (от 800 до текущего): ", 800, DateTime.Now.Year);
            decimal price = ReadDecimal("Введите цену (руб., больше 0): ", 0.01m, 1_000_000m);

            var createdBook = Library.AddBook(title, author, genre, year, price);
            PrintSuccess($"\nКнига успешно добавлена с ID: {createdBook.Id}");
        }

        private static void DeleteBookCommand()
        {
            PrintHeader("УДАЛЕНИЕ КНИГИ");
            int id = ReadInt("Введите ID книги для удаления: ", 1, int.MaxValue);

            bool deleted = Library.RemoveBookById(id);
            if (deleted)
                PrintSuccess($"Книга с ID {id} успешно удалена.");
            else
                PrintWarning($"Книга с ID {id} не найдена в базе данных.");
        }

        private static void SearchBooksCommand()
        {
            PrintHeader("ПОИСК КНИГ");
            Console.WriteLine("1. По названию");
            Console.WriteLine("2. По автору");
            Console.WriteLine("3. По жанру");
            Console.Write("Выберите критерий поиска (1-3): ");

            string choice = Console.ReadLine()?.Trim() ?? "";
            List<Book> results = new();

            switch (choice)
            {
                case "1":
                    string titleQuery = ReadNonEmptyString("Введите название или его часть: ");
                    results = Library.FindByTitle(titleQuery);
                    break;
                case "2":
                    string authorQuery = ReadNonEmptyString("Введите имя автора или его часть: ");
                    results = Library.FindByAuthor(authorQuery);
                    break;
                case "3":
                    Genre genre = ReadGenre();
                    results = Library.FindByGenre(genre);
                    break;
                default:
                    PrintWarning("Неверный выбор критерия поиска.");
                    return;
            }

            Console.WriteLine("\nРезультаты поиска:");
            PrintBookList(results);
        }

        private static void SortBooksCommand()
        {
            PrintHeader("СОРТИРОВКА КНИГ");
            Console.WriteLine("1. По названию (в алфавитном порядке)");
            Console.WriteLine("2. По году издания (по возрастанию)");
            Console.Write("Выберите вариант сортировки (1-2): ");

            string choice = Console.ReadLine()?.Trim() ?? "";
            List<Book> sortedList;

            switch (choice)
            {
                case "1":
                    sortedList = Library.SortByTitle();
                    Console.WriteLine("\nКниги, отсортированные по названию:");
                    break;
                case "2":
                    sortedList = Library.SortByYear();
                    Console.WriteLine("\nКниги, отсортированные по году издания:");
                    break;
                default:
                    PrintWarning("Неверный выбор варианта сортировки.");
                    return;
            }

            PrintBookList(sortedList);
        }

        private static void ShowPriceExtremesCommand()
        {
            PrintHeader("САМАЯ ДОРОГАЯ И САМАЯ ДЕШЁВАЯ КНИГА");
            var (cheapest, mostExpensive) = Library.GetPriceExtremes();

            if (cheapest == null || mostExpensive == null)
            {
                PrintWarning("Библиотека пуста. Невозможно рассчитать ценовые показатели.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Самая дешёвая книга:");
            Console.ResetColor();
            Console.WriteLine(cheapest);

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Самая дорогая книга:");
            Console.ResetColor();
            Console.WriteLine(mostExpensive);
        }

        private static void ShowAuthorGroupStatsCommand()
        {
            PrintHeader("ГРУППИРОВКА КНИГ ПО АВТОРАМ");
            var stats = Library.GroupByAuthor().ToList();

            if (!stats.Any())
            {
                PrintWarning("В библиотеке нет книг для группировки.");
                return;
            }

            Console.WriteLine("{0,-35} | {1,15}", "Автор", "Количество книг");
            Console.WriteLine(new string('-', 55));

            foreach (var item in stats)
            {
                Console.WriteLine("{0,-35} | {1,15}", item.Author, item.Count);
            }
        }

        private static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim() ?? "";
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                PrintWarning("Поле не может быть пустым. Пожалуйста, введите значение.");
            }
        }

        private static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(input, out int result) && result >= min && result <= max)
                {
                    return result;
                }
                PrintWarning($"Некорректный ввод. Введите целое число в диапазоне от {min} до {max}.");
            }
        }

        private static decimal ReadDecimal(string prompt, decimal min, decimal max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim()?.Replace(',', '.') ?? "";
                if (decimal.TryParse(input, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal result)
                    && result >= min && result <= max)
                {
                    return Math.Round(result, 2);
                }
                PrintWarning($"Некорректная цена. Введите положительное число от {min} до {max}.");
            }
        }

        private static Genre ReadGenre()
        {
            Console.WriteLine("\nДоступные жанры:");
            foreach (Genre g in Enum.GetValues<Genre>())
            {
                Console.WriteLine($"{(int)g}. {Book.GetGenreName(g)}");
            }

            int minGenre = (int)Enum.GetValues<Genre>().Min();
            int maxGenre = (int)Enum.GetValues<Genre>().Max();

            int selected = ReadInt($"Выберите номер жанра ({minGenre}-{maxGenre}): ", minGenre, maxGenre);
            return (Genre)selected;
        }

        private static void PrintBookList(IEnumerable<Book> books)
        {
            var list = books.ToList();
            if (!list.Any())
            {
                PrintError("Книги не найдены.");
                return;
            }

            Console.WriteLine(new string('-', 95));
            foreach (var book in list)
            {
                Console.WriteLine(book);
            }
            Console.WriteLine(new string('-', 95));
            Console.WriteLine($"Всего записей: {list.Count}");
        }

        private static void PrintHeader(string text)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n--- {text} ---");
            Console.ResetColor();
        }

        private static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void PrintWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}