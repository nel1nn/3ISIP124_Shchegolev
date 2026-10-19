namespace ConsoleApp2.Entities
{
    /// <summary>Итог одной атаки врага по игроку — для вывода в бою.</summary>
    public class AttackReport
    {
        /// <summary>Урон до учёта защиты (с критом, если он сработал).</summary>
        public int RawDamage { get; set; }
        public int FinalDamage { get; set; }
        public int Blocked { get; set; }
        public bool Dodged { get; set; }
        public bool Critical { get; set; }
        public bool ArmorIgnored { get; set; }
        public bool Froze { get; set; }
    }
}
