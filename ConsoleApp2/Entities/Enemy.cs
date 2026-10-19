using ConsoleApp2.Core;

namespace ConsoleApp2.Entities
{
    /// <summary>Враг. Подклассы переопределяют особенности атаки.</summary>
    public abstract class Enemy : Character
    {
        public int Attack { get; }
        public int Defense { get; }
        public bool IsBoss { get; }

        /// <summary>Раса врага для вывода («Гоблин», «Скелет», «Маг»).</summary>
        public abstract string Race { get; }

        /// <summary>Краткое описание особенности для вывода.</summary>
        public abstract string AbilityDescription { get; }

        /// <summary>Игнорирует ли атака защиту (блок) игрока.</summary>
        public virtual bool IgnoresArmor => false;

        protected Enemy(string name, int maxHp, int attack, int defense, bool isBoss) : base(name, maxHp)
        {
            Attack = attack;
            Defense = defense;
            IsBoss = isBoss;
        }

        /// <summary>Умножает базовую характеристику на коэффициент босса.</summary>
        protected static int Scale(int value, double multiplier) => (int)Math.Round(value * multiplier);

        /// <summary>Враг всегда атакует игрока; особенности применяются через переопределяемые методы.</summary>
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

        /// <summary>Изменяет исходящий урон (например, критический удар).</summary>
        protected virtual int ModifyDamage(int damage, Dice dice, AttackReport report) => damage;

        /// <summary>Дополнительный эффект после успешного попадания (например, заморозка).</summary>
        protected virtual void OnHit(Player player, Dice dice, AttackReport report)
        {
        }

        /// <summary>Пытается заморозить игрока с заданным шансом.</summary>
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
