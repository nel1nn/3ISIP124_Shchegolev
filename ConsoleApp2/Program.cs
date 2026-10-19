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
            try
            {
                ui.Header("РОГАЛИК: подземелье");
                var game = new Game(ui, new Dice(), new Player(), 0);
                game.Run();
                ui.Line("Спасибо за игру!");
            }
            catch (InputClosedException)
            {
                ui.Line();
                ui.Line("Ввод закрыт. Игра завершена.");
            }
        }
    }
}
