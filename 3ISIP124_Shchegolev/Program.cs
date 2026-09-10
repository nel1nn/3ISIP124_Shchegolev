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
                    Wait();
                    continue;
                }

            }

            while (true)
            {
                Console.Clear();
                int chose;
                Console.Write("1. Вывод данных\n2. Статистика (среднее, максимальное, минимальное, сумма)\n3. Сортировка по цене \n4. Конвертация валюты\n5. Поиск по названию \n0. Выход\n\nВыбор: ");
                bool correct = int.TryParse(Console.ReadLine(), out chose);
                
                if (!correct)
                    continue;

                switch(chose)
                {
                    case 0:
                        return;

                    case 1:
                        Console.Clear();
                        for (int i = 0; i < shoppingListName.Count; i++)
                        {
                            Console.WriteLine($"{i+1}) {shoppingListName[i]} - {shoppingListPrice[i]}");
                        }
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("Для продолжения нажмите на кнопку...");
                        Console.ResetColor();
                        Console.ReadKey();
                    break;

                    case 2:
                        Console.Clear();
                        int max = 0;
                        int min = int.MaxValue;
                        int sum = 0;
                        double average;

                        for (int i = 0; i < shoppingListPrice.Count; i++)
                        {
                            if (shoppingListPrice[i] > max)
                                max = shoppingListPrice[i];

                            if (shoppingListPrice[i] < min)
                                min = shoppingListPrice[i];

                            sum += shoppingListPrice[i];
                        }
                        average = sum / shoppingListPrice.Count;

                        Console.WriteLine($"\nМаксимальная сумма: {max}\nМинимальная сумма: {min}\nСредняя сумма: {average}");
                        Wait();
                  break;

                    default:
                        continue;
                }
            }


        }

        static void Wait()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Для продолжения нажмите на кнопку...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}