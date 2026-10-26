namespace Lesson30;

public class Coffee : Drink
{
    public override string Name => "Кофе";

    public override decimal Price => 150m;

    public override void Prepare()
    {
        Console.WriteLine("Мелем зёрна, завариваем эспрессо и добавляем горячую воду.");
    }
}
