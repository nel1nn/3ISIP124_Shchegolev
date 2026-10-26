namespace Lesson30;

public abstract class Drink
{
    public abstract string Name { get; }

    public abstract decimal Price { get; }

    public abstract void Prepare();

    public void Serve()
    {
        Console.WriteLine($"Напиток {Name} подан!");
    }
}
