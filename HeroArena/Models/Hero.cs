namespace HeroArena.Models
{
    public class Hero
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Health { get; set; }
        public string? ImageURL { get; set; }
        public ICollection<Spell> Spells { get; set; } = new List<Spell>();
        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}