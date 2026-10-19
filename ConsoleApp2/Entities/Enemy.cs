using ConsoleApp2.Core;

namespace ConsoleApp2.Entities
{
    public abstract class Enemy : Character
    {
        public int Attack { get; }
        public int Defense { get; }
        public bool IsBoss { get; }

        public abstract string Race { get; }

        public abstract string AbilityDescription { get; }

        public virtual bool IgnoresArmor => false;

        protected Enemy(string name, int maxHp, int attack, int defense, bool isBoss) : base(name, maxHp)
        {
            Attack = attack;
            Defense = defense;
            IsBoss = isBoss;
        }

        protected static int Scale(int value, double multiplier) => (int)Math.Round(value * multiplier);

        public AttackReport AttackPlayer(Player player, Dice dice)
        {
            var report = new AttackReport();
            int damage = ModifyDamage(Attack, dice, report);
            player.ReceiveHit(damage, IgnoresArmor, dice, report);

            if (!report.Dodged)
            {
                OnHit(player, dice, report);
            }

            return report;
        }

        protected virtual int ModifyDamage(int damage, Dice dice, AttackReport report) => damage;

        protected virtual void OnHit(Player player, Dice dice, AttackReport report)
        {
        }

        protected static void TryFreeze(Player player, double chance, Dice dice, AttackReport report)
        {
            if (dice.Chance(chance))
            {
                player.IsFrozen = true;
                report.Froze = true;
            }
        }
    }
}
