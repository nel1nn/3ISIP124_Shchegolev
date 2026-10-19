using ConsoleApp2.Core;

namespace ConsoleApp2.Items
{
    /// <summary>Создаёт случайные предметы для сундуков и наград.</summary>
    public class ItemFactory
    {
        private static readonly string[] WeaponNames = { "Меч", "Топор", "Кинжал", "Булава", "Копьё", "Секира" };
        private static readonly string[] ArmorNames = { "Кольчуга", "Кираса", "Кожаная броня", "Латы", "Роба мага" };

        private const int WeaponBaseMin = 8;
        private const int WeaponBaseMax = 14;
        private const int ArmorBaseMin = 2;
        private const int ArmorBaseMax = 6;

        private readonly Dice _dice;

        public ItemFactory(Dice dice)
        {
            _dice = dice;
        }

        /// <summary>Зелье, оружие или доспех — с равной вероятностью.</summary>
        public Item CreateRandom()
        {
            return _dice.Next(0, 2) switch
            {
                0 => new HealingPotion(),
                1 => CreateWeapon(RollRarity(Rarity.Common)),
                _ => CreateArmor(RollRarity(Rarity.Common))
            };
        }

        /// <summary>Случайное оружие или доспех не ниже указанной редкости (награда за босса).</summary>
        public Equipment CreateEquipment(Rarity minRarity)
        {
            var rarity = RollRarity(minRarity);
            return _dice.Chance(0.5) ? CreateWeapon(rarity) : CreateArmor(rarity);
        }

        private Weapon CreateWeapon(Rarity rarity)
        {
            int attack = Scale(_dice.Next(WeaponBaseMin, WeaponBaseMax), rarity);
            return new Weapon(_dice.Pick(WeaponNames), rarity, attack);
        }

        private Armor CreateArmor(Rarity rarity)
        {
            int defense = Scale(_dice.Next(ArmorBaseMin, ArmorBaseMax), rarity);
            return new Armor(_dice.Pick(ArmorNames), rarity, defense);
        }

        private static int Scale(int baseValue, Rarity rarity) =>
            Math.Max(1, (int)Math.Round(baseValue * rarity.Multiplier()));

        private Rarity RollRarity(Rarity minRarity)
        {
            var candidates = Enum.GetValues<Rarity>()
                .Where(r => r >= minRarity)
                .Select(r => (r, r.DropWeight()))
                .ToList();
            return _dice.PickWeighted(candidates);
        }
    }
}
