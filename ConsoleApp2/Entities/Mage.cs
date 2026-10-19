using ConsoleApp2.Core;

namespace ConsoleApp2.Entities
{
    public class Mage : Enemy
    {
        public const int BaseHp = 25;
        public const int BaseAttack = 8;
        public const int BaseDefense = 2;
        public const double BaseFreezeChance = 0.25;

        public double FreezeChance { get; }

        public Mage() : this("Маг", BaseHp, BaseAttack, BaseDefense, BaseFreezeChance, false)
        {
        }

        protected Mage(string name, int hp, int attack, int defense, double freezeChance, bool isBoss)
            : base(name, hp, attack, defense, isBoss)
        {
            FreezeChance = freezeChance;
        }

        public override string Race => "Маг";

        public override string AbilityDescription => $"шанс заморозки {FreezeChance:P0}";

        protected override void OnHit(Player player, Dice dice, AttackReport report) =>
            TryFreeze(player, FreezeChance, dice, report);
    }
}
