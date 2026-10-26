namespace Lesson42;

public static class Calculator
{
    public const string Division = "Деление";
    public const string Multiplication = "Умножение";
    public const string Addition = "Сложение";
    public const string Subtraction = "Вычитание";
    public const string Remainder = "Остаток от деления";

    public static readonly List<string> Operations = new List<string>
    {
        Division,
        Multiplication,
        Addition,
        Subtraction,
        Remainder
    };

    public static decimal Calculate(decimal first, decimal second, string operation)
    {
        switch (operation)
        {
            case Division:
                return first / second;
            case Multiplication:
                return first * second;
            case Addition:
                return first + second;
            case Subtraction:
                return first - second;
            case Remainder:
                return first % second;
            default:
                throw new ArgumentException("Неизвестное действие.");
        }
    }
}
