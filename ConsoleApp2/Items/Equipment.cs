using ConsoleApp2.Core;
using ConsoleApp2.Entities;

namespace ConsoleApp2.Items
{
    public abstract class Equipment : Item
    {
        public int Power { get; }

        public abstract string StatName { get; }

        protected Equipment(string name, Rarity rarity, int power) : base(name, rarity)
        {
            Power = power;
        }

        public override string Describe() => $"{base.Describe()} — {StatName}: {Power}";

        protected abstract Equipment GetEquipped(Player player);

        protected abstract void EquipTo(Player player);

        public override void Acquire(Player player, ConsoleUi ui)
        {
            var current = GetEquipped(player);
            ui.Line("Новый предмет:   " + Describe(), Rarity.Color());
            ui.Line("Сейчас надето:   " + current.Describe(), current.Rarity.Color());

            int diff = Power - current.Power;
            string verdict = diff > 0 ? $"лучше на {diff}" : diff < 0 ? $"хуже на {-diff}" : "такой же";
            ui.Line($"({StatName} нового предмета {verdict})");

            int choice = ui.AskChoice("Что сделать с предметом?", "Взять (заменит текущий)", "Выбросить");
            if (choice == 1)
            {
                EquipTo(player);
                ui.Line($"Вы экипировали: {Name}.", ConsoleColor.Green);
            }
            else
            {
                ui.Line($"Вы выбросили: {Name}.");
            }
        }
    }
}
