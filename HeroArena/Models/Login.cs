using System.Collections.Generic;

namespace HeroArena.Models
{
    public class Login
    {
        public int ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Player? Player { get; set; }
    }
}