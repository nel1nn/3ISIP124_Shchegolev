using ConsoleApp2.Core;

namespace ConsoleApp2.Entities
{
    public class EnemyFactory
    {
        private static readonly Func<Enemy>[] EnemyCreators =
        {
            () => new Goblin(),
            () => new Skeleton(),
            () => new Mage()
        };

        private static readonly Func<Enemy>[] BossCreators =
        {
            () => new VvgBoss(),
            () => new KovalskyBoss(),
            () => new ArchMageCppBoss(),
            () => new PestovBoss()
        };

        private readonly Dice _dice;

        public EnemyFactory(Dice dice)
        {
            _dice = dice;
        }

        public Enemy CreateRandomEnemy() => _dice.Pick(EnemyCreators)();

        public Enemy CreateRandomBoss() => _dice.Pick(BossCreators)();
    }
}
