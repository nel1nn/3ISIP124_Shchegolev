using ConsoleApp2.Core;
using ConsoleApp2.Items;

namespace ConsoleApp2.Entities
{
    public class Player : Character
    {
        public const int StartMaxHp = 100;
        public const double DodgeChance = 0.40;
        public const double BlockMinShare = 0.70;
        public const double BlockMaxShare = 1.00;

        public Weapon Weapon { get; private set; }
        public Armor Armor { get; private set; }
        public int Kills { get; set; }

        /// <summary>Игрок выбрал «Защиту» в этом раунде.</summary>
        public bool IsDefending { get; private set; }

        /// <summary>Игрок заморожен и пропустит следующий ход.</summary>
        public bool IsFrozen { get; set; }

        public Player() : this(
            new Weapon("Ржавый кинжал", Rarity.Common, 12),
            new Armor("Старая роба", Rarity.Common, 3),
            StartMaxHp,
            0)
        {
        }

        public Player(Weapon weapon, Armor armor, int hp, int kills) : base("Герой", StartMaxHp)
        {
            Weapon = weapon;
            Armor = armor;
            Hp = Math.Clamp(hp, 1, MaxHp);
            Kills = kills;
        }

        public void Equip(Weapon weapon) => Weapon = weapon;

        public void Equip(Armor armor) => Armor = armor;

        public void HealFully() => Hp = MaxHp;

        public void StartDefending() => IsDefending = true;

        public void StopDefending() => IsDefending = false;

        /// <summary>Урон по врагу: атака оружия минус защита врага, но не меньше 1.</summary>
        public int CalculateDamageTo(Enemy enemy) => Math.Max(1, Weapon.Power - enemy.Defense);

        /// <summary>
        /// Принимает удар врага. Если игрок защищается — 40% полного уклонения,
        /// иначе блок на 70–100% значения защиты доспехов (если враг не игнорирует броню).
        /// </summary>
        public void ReceiveHit(int rawDamage, bool ignoreArmor, Dice dice, AttackReport report)
        {
            report.RawDamage = rawDamage;
            report.ArmorIgnored = ignoreArmor;

            if (IsDefending && dice.Chance(DodgeChance))
            {
                report.Dodged = true;
                return;
            }

            int damage = rawDamage;
            if (IsDefending && !ignoreArmor)
            {
                double share = dice.Next(BlockMinShare, BlockMaxShare);
                report.Blocked = (int)Math.Round(Armor.Power * share);
                damage = Math.Max(0, rawDamage - report.Blocked);
            }

            report.FinalDamage = damage;
            TakeDamage(damage);
        }
    }
}
