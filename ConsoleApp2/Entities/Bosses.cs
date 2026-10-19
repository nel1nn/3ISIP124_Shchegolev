using ConsoleApp2.Core;

namespace ConsoleApp2.Entities
{
    public class VvgBoss : Goblin
    {
        public VvgBoss() : base(
            "ВВГ",
            Scale(BaseHp, 2.0),
            Scale(BaseAttack, 1.5),
            Scale(BaseDefense, 1.2),
            BaseCritChance + 0.10,
            true)
        {
        }
    }

    public class KovalskyBoss : Skeleton
    {
        public KovalskyBoss() : base(
            "Ковальский",
            Scale(BaseHp, 2.5),
            Scale(BaseAttack, 1.3),
            Scale(BaseDefense, 1.4),
            true)
        {
        }
    }

    public class ArchMageCppBoss : Mage
    {
        public ArchMageCppBoss() : base(
            "Архимаг C++",
            Scale(BaseHp, 1.8),
            Scale(BaseAttack, 1.6),
            Scale(BaseDefense, 1.1),
            BaseFreezeChance + 0.10,
            true)
        {
        }
    }

    public class PestovBoss : Skeleton
    {
        private const double FreezeChance = Mage.BaseFreezeChance + 0.15;

        public PestovBoss() : base(
            "Пестов С--",
            Scale(BaseHp, 1.3),
            Scale(BaseAttack, 1.8),
            Scale(BaseDefense, 0.6),
            true)
        {
        }

        public override string AbilityDescription =>
            $"{base.AbilityDescription}, шанс заморозки {FreezeChance:P0}";

        protected override void OnHit(Player player, Dice dice, AttackReport report) =>
            TryFreeze(player, FreezeChance, dice, report);
    }
}
