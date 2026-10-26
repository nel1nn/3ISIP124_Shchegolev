namespace Lesson30;

public class DrinkFactory
{
    public static Drink? CreateDrink(string type)
    {
        switch (type.Trim().ToLower())
        {
            case "кофе":
                return new Coffee();
            case "чай":
                return new Tea();
            case "смузи":
                return new Smoothie();
            default:
                return null;
        }
    }
}
