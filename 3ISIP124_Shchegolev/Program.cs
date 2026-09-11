using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _3ISIP124_Shchegolev
{
    internal class Program
    {
        static int count = 1;
        static List<string> shoppingListName = new List<string>();
        static List<double> shoppingListPrice = new List<double>();
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
                        double max = 0;
                        double min = double.MaxValue;
                        double sum = 0;
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

                    case 3:
                        Console.Clear();
                        for (int i = 0; i < shoppingListPrice.Count; i++)
                        {
                            for (int j = shoppingListPrice.Count - 1; j > i; j--)
                            {
                                if (shoppingListPrice[j - 1] > shoppingListPrice[j])
                                {
                                    double a = shoppingListPrice[j];
                                    string b = shoppingListName[j];
                                    shoppingListPrice[j] = shoppingListPrice[j - 1];
                                    shoppingListName[j] = shoppingListName[j-1];
                                    shoppingListPrice[j - 1] = a;
                                    shoppingListName[j-1] = b;
                                }
                            }
                        }
                        for (int i = 0; i < shoppingListPrice.Count; i++)
                        {
                            Console.WriteLine($"{shoppingListName[i]} - {shoppingListPrice[i]}");
                        }
                        Wait();
                    break;

                    case 4:
                        
                        while(true)
                        {
                            Console.Clear();
                            Console.WriteLine("Введите курс");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine("( при вводе '100' будет считаться, что указанная раннее 1 валюта (с) равна 100 новой валюты (н) )\n");
                            Console.ResetColor();

                            try
                            {
                                Console.Write("Курс: ");
                                double curse = Convert.ToDouble(Console.ReadLine());

                                if (curse > 0)
                                {
                                    Console.Clear();
                                    Console.WriteLine($"Курс: {curse} н = 1 с");
                                    Console.WriteLine("\nПодтвердить конвертацию?\n1) Да\n2) Нет");
                                    Console.Write("Выбор: ");
                                    int chose1 = Convert.ToInt32(Console.ReadLine());
                                    switch (chose1)
                                    {
                                        case 1:
                                            for (int i = 0;i < shoppingListPrice.Count; i++)
                                            {
                                                shoppingListPrice[i] = shoppingListPrice[i] * curse;
                                            }
                                            break;

                                        default:
                                            continue;
                                    }
                                    break;
                                }

                                if (curse <= 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Курс не может быть отрицательным или нулевым.");
                                    Console.ResetColor();
                                    Wait();
                                    continue;
                                }

                                if (curse == 0)
                                {
                                    break;
                                }

                            }
                            catch (Exception)
                            {
                                Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Некорректный ввод"); Console.ResetColor();

                                Wait();

                                continue;
                            }
                            
                        }
                        


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