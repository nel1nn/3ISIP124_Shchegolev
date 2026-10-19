using ConsoleApp2.Core;
using ConsoleApp2.Entities;

namespace ConsoleApp2.Items
{
    /// <summary>Предмет, который может выпасть из сундука.</summary>
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

        /// <summary>Что происходит, когда игрок получает предмет из сундука.</summary>
        public abstract void Acquire(Player player, ConsoleUi ui);
    }
}
