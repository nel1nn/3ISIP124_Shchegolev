using ConsoleApp2.Core;
using ConsoleApp2.Entities;

namespace ConsoleApp2.Items
{
    public abstract class Item
    {
        public string Name { get; }
        public Rarity Rarity { get; }

        protected Item(string name, Rarity rarity)
        {
            Name = name;
            Rarity = rarity;
        }

        public virtual string Describe() => $"{Name} [{Rarity.Title()}]";

        public abstract void Acquire(Player player, ConsoleUi ui);
    }
}
