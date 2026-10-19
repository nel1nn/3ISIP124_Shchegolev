using ConsoleApp2.Entities;

namespace ConsoleApp2.Items
{
    public class Armor : Equipment
    {
        public Armor(string name, Rarity rarity, int defense) : base(name, rarity, defense)
        {
        }

        public override string StatName => "Защита";

        protected override Equipment GetEquipped(Player player) => player.Armor;

        protected override void EquipTo(Player player) => player.Equip(this);
    }
}
