namespace ConsoleApp2.Entities
{
    /// <summary>Скелет: игнорирует защиту игрока.</summary>
    public class Skeleton : Enemy
    {
        public const int BaseHp = 35;
        public const int BaseAttack = 7;
        public const int BaseDefense = 4;

        public Skeleton() : this("Скелет", BaseHp, BaseAttack, BaseDefense, false)
        {
        }

        protected Skeleton(string name, int hp, int attack, int defense, bool isBoss)
            : base(name, hp, attack, defense, isBoss)
        {
        }

        public override string Race => "Скелет";

        public override string AbilityDescription => "игнорирует защиту игрока";

        public override bool IgnoresArmor => true;
    }
}
