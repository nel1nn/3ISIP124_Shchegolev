namespace ConsoleApp2.Core
{
    /// <summary>
    /// Единая точка работы с генератором случайных чисел.
    /// Все шансы и случайные величины в игре проходят через этот класс.
    /// </summary>
    public class Dice
    {
        private readonly Random _random;

        public Dice() : this(new Random()) { }

        public Dice(Random random)
        {
            _random = random;
        }

        /// <summary>Срабатывает ли событие с вероятностью probability (0..1).</summary>
        public bool Chance(double probability) => _random.NextDouble() < probability;

        /// <summary>Целое число из диапазона [min, max] включительно.</summary>
        public int Next(int min, int max) => _random.Next(min, max + 1);

        /// <summary>Вещественное число из диапазона [min, max).</summary>
        public double Next(double min, double max) => min + _random.NextDouble() * (max - min);

        /// <summary>Случайный элемент списка.</summary>
        public T Pick<T>(IReadOnlyList<T> items) => items[_random.Next(items.Count)];

        /// <summary>Случайный элемент с учётом весов (чем больше вес, тем вероятнее).</summary>
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
