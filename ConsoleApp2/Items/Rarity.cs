namespace ConsoleApp2.Items
{
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic
    }

    public static class RarityExtensions
    {
        public static string Title(this Rarity rarity) => rarity switch
        {
            Rarity.Common => "Обычная",
            Rarity.Uncommon => "Необычная",
            Rarity.Rare => "Редкая",
            Rarity.Epic => "Эпическая",
            _ => "Неизвестная"
        };

        public static double Multiplier(this Rarity rarity) => rarity switch
        {
            Rarity.Common => 1.0,
            Rarity.Uncommon => 1.25,
            Rarity.Rare => 1.5,
            Rarity.Epic => 2.0,
            _ => 1.0
        };

        public static int DropWeight(this Rarity rarity) => rarity switch
        {
            Rarity.Common => 60,
            Rarity.Uncommon => 25,
            Rarity.Rare => 11,
            Rarity.Epic => 4,
            _ => 0
        };

        public static ConsoleColor Color(this Rarity rarity) => rarity switch
        {
            Rarity.Common => ConsoleColor.Gray,
            Rarity.Uncommon => ConsoleColor.Green,
            Rarity.Rare => ConsoleColor.Cyan,
            Rarity.Epic => ConsoleColor.Magenta,
            _ => ConsoleColor.Gray
        };
    }
}
