using ConsoleApp2.Core;
using ConsoleApp2.Entities;

namespace ConsoleApp2.Items
{
    public class HealingPotion : Item
    {
        public HealingPotion() : base("Лечебное зелье", Rarity.Common)
        {
        }

        public override string Describe() => $"{Name} (полностью восстанавливает здоровье)";

        public override void Acquire(Player player, ConsoleUi ui)
        {
            int healed = player.MaxHp - player.Hp;
            player.HealFully();
            ui.Line($"Вы выпили зелье и восстановили {healed} HP. Здоровье: {player.Hp}/{player.MaxHp}.", ConsoleColor.Green);
        }
    }
}
