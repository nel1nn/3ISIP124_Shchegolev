namespace ConsoleApp2.Entities
{
    /// <summary>Базовый класс для всех существ, имеющих здоровье.</summary>
    public abstract class Character
    {
        public string Name { get; }
        public int MaxHp { get; }
        public int Hp { get; protected set; }
        public bool IsAlive => Hp > 0;

        protected Character(string name, int maxHp)
        {
            Name = name;
            MaxHp = maxHp;
            Hp = maxHp;
        }

        public void TakeDamage(int damage)
        {
            Hp = Math.Max(0, Hp - Math.Max(0, damage));
        }
    }
}
