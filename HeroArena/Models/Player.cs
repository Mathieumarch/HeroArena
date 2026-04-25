namespace HeroArena.Models
{
    public class Player
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? LoginID { get; set; }
        public Login? Login { get; set; }
        public ICollection<Hero> Heroes { get; set; } = new List<Hero>();
    }
}