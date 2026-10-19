using System.Text;
using ConsoleApp2.Core;
using ConsoleApp2.Entities;
using ConsoleApp2.GameLogic;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            var ui = new ConsoleUi();
            var saves = new SaveManager();
            try
            {
                ui.Header("РОГАЛИК: подземелье");
                var game = CreateGame(ui, saves);
                if (game is not null)
                {
                    game.Run();
                }
                ui.Line("Спасибо за игру!");
            }
            catch (InputClosedException)
            {
                ui.Line();
                ui.Line("Ввод закрыт. Игра завершена.");
            }
        }

        private static Game? CreateGame(ConsoleUi ui, SaveManager saves)
        {
            var dice = new Dice();
            while (true)
            {
                int choice = ui.AskChoice("Главное меню:", "Новая игра", "Загрузить игру", "Выход");
                switch (choice)
                {
                    case 1:
                        return new Game(ui, dice, saves, new Player(), 0);
                    case 2:
                        if (!saves.SaveExists)
                        {
                            ui.Line("Сохранение не найдено.", ConsoleColor.Red);
                        }
                        else if (saves.TryLoad(out Player? player, out int turn, out string error) && player is not null)
                        {
                            ui.Line($"Игра загружена. Пройдено ходов: {turn}.", ConsoleColor.Green);
                            return new Game(ui, dice, saves, player, turn);
                        }
                        else
                        {
                            ui.Line($"Не удалось загрузить игру: {error}", ConsoleColor.Red);
                        }
                        break;
                    default:
                        return null;
                }
            }
        }
    }
}
