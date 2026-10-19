namespace ConsoleApp2.Core
{
    /// <summary>Поток ввода закрыт (например, Ctrl+Z / конец файла) — игру нужно завершить.</summary>
    public class InputClosedException : Exception
    {
    }

    /// <summary>Весь ввод-вывод игры. Остальные классы не обращаются к Console напрямую.</summary>
    public class ConsoleUi
    {
        public void Line(string text = "", ConsoleColor? color = null)
        {
            if (color is null)
            {
                Console.WriteLine(text);
                return;
            }

            var previous = Console.ForegroundColor;
            Console.ForegroundColor = color.Value;
            Console.WriteLine(text);
            Console.ForegroundColor = previous;
        }

        public void Header(string title)
        {
            Line();
            Line(new string('=', 60), ConsoleColor.DarkYellow);
            Line("  " + title, ConsoleColor.Yellow);
            Line(new string('=', 60), ConsoleColor.DarkYellow);
        }

        /// <summary>
        /// Показывает пронумерованные варианты и просит выбрать один из них.
        /// Повторяет запрос, пока не будет введено корректное число. Возвращает номер (с 1).
        /// </summary>
        public int AskChoice(string prompt, params string[] options)
        {
            Line(prompt, ConsoleColor.White);
            for (int i = 0; i < options.Length; i++)
            {
                Line($"  {i + 1} — {options[i]}");
            }

            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                if (input is null)
                {
                    throw new InputClosedException();
                }

                if (int.TryParse(input.Trim(), out int choice) && choice >= 1 && choice <= options.Length)
                {
                    return choice;
                }

                Line($"Введите число от 1 до {options.Length}.", ConsoleColor.Red);
            }
        }
    }
}
