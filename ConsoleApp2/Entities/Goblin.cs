using ConsoleApp2.Core;

namespace ConsoleApp2.Entities
{
    public class Goblin : Enemy
    {
        public const int BaseHp = 30;
        public const int BaseAttack = 6;
        public const int BaseDefense = 3;
        public const double BaseCritChance = 0.20;
        public const double CritMultiplier = 2.0;

        public double CritChance { get; }

        public Goblin() : this("Гоблин", BaseHp, BaseAttack, BaseDefense, BaseCritChance, false)
        {
        }

        protected Goblin(string name, int hp, int attack, int defense, double critChance, bool isBoss)
            : base(name, hp, attack, defense, isBoss)
        {
            CritChance = critChance;
        }

        public override string Race => "Гоблин";

        public override string AbilityDescription => $"шанс критического урона {CritChance:P0}";

        protected override int ModifyDamage(int damage, Dice dice, AttackReport report)
        {
            if (dice.Chance(CritChance))
            {
                report.Critical = true;
                return (int)Math.Round(damage * CritMultiplier);
            }

            return damage;
        }
    }
}
