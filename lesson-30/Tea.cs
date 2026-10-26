namespace Lesson30;

public class Tea : Drink
{
    public override string Name => "Чай";

    public override decimal Price => 100m;

    public override void Prepare()
    {
        Console.WriteLine("Кипятим воду и заливаем ею чайный пакетик.");
    }
}
