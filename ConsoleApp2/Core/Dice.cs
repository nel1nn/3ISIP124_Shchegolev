namespace ConsoleApp2.Core
{
    public class Dice
    {
        private readonly Random _random;

        public Dice() : this(new Random()) { }

        public Dice(Random random)
        {
            _random = random;
        }

        public bool Chance(double probability) => _random.NextDouble() < probability;

        public int Next(int min, int max) => _random.Next(min, max + 1);

        public double Next(double min, double max) => min + _random.NextDouble() * (max - min);

        public T Pick<T>(IReadOnlyList<T> items) => items[_random.Next(items.Count)];

        public T PickWeighted<T>(IReadOnlyList<(T Item, int Weight)> items)
        {
            int total = items.Sum(i => i.Weight);
            int roll = _random.Next(total);
            foreach (var (item, weight) in items)
            {
                if (roll < weight)
                {
                    return item;
                }
                roll -= weight;
            }
            return items[^1].Item;
        }
    }
}
