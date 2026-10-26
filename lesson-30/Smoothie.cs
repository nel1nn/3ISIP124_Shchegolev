namespace Lesson30;

public class Smoothie : Drink
{
    public override string Name => "Смузи";

    public override decimal Price => 220m;

    public override void Prepare()
    {
        Console.WriteLine("Взбиваем в блендере фрукты, ягоды и йогурт.");
    }
}
