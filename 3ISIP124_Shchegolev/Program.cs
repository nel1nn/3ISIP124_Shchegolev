namespace _3ISIP124_Shchegolev
{
    internal class Program
    {
        static int count = 1;
        static List<string> shoppingListName = new List<string>();
        static List<int> shoppingListPrice = new List<int>();
        static void Main(string[] args)
        {
            Console.WriteLine("Введите траты в формате (название товара или услуги; цена).");
            while (count < 41)
            {
                Console.Write($"\n{count}) ");
                string enter = Console.ReadLine();
                try
                {
                    if (string.IsNullOrEmpty(enter))
                        break;

                    string[] multiEnter = enter.Split(";");
                    string product = multiEnter[0];
                    
                    int price = int.Parse(multiEnter[1]);

                    shoppingListName.Add(product);
                    shoppingListPrice.Add(price);

                    count++;
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Неккоректный ввод!");
                    Console.ResetColor();
                    continue;
                }

            }
        }
    }
}