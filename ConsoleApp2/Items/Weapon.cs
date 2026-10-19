using ConsoleApp2.Entities;

namespace ConsoleApp2.Items
{
    public class Weapon : Equipment
    {
        public Weapon(string name, Rarity rarity, int attack) : base(name, rarity, attack)
        {
        }

        public override string StatName => "Атака";

        protected override Equipment GetEquipped(Player player) => player.Weapon;

        protected override void EquipTo(Player player) => player.Equip(this);
    }
}
