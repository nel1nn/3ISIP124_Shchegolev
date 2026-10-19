namespace ConsoleApp2.Entities
{
    public class AttackReport
    {
        public int RawDamage { get; set; }
        public int FinalDamage { get; set; }
        public int Blocked { get; set; }
        public bool Dodged { get; set; }
        public bool Critical { get; set; }
        public bool ArmorIgnored { get; set; }
        public bool Froze { get; set; }
    }
}
