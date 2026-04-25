namespace HeroArena.Models
{
    public class Spell
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Damage { get; set; }
        public string? Description { get; set; }
        public ICollection<Hero> Heroes { get; set; } = new List<Hero>();
    }
}