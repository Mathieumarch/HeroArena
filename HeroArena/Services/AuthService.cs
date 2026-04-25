using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HeroArena.Data;
using HeroArena.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroArena.Services
{
    public class AuthService
    {
        private readonly string _connectionString;

        public AuthService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }

        public async Task<Login?> LoginAsync(string username, string password)
        {
            var hash = HashPassword(password);
            using var ctx = new HeroArenaContext(_connectionString);
            return await ctx.Logins
                .Include(l => l.Player)
                .FirstOrDefaultAsync(l =>
                    l.Username == username &&
                    l.PasswordHash == hash);
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            using var ctx = new HeroArenaContext(_connectionString);
            if (await ctx.Logins.AnyAsync(l => l.Username == username))
                return false;

            var login = new Login
            {
                Username = username,
                PasswordHash = HashPassword(password)
            };
            ctx.Logins.Add(login);

            var player = new Player { Name = username, Login = login };
            ctx.Players.Add(player);

            await ctx.SaveChangesAsync();
            return true;
        }

        public async Task SeedDefaultDataAsync()
        {
            using var ctx = new HeroArenaContext(_connectionString);

            if (await ctx.Heroes.AnyAsync()) return;

            var fireball = new Spell
            {
                Name = "Boule de feu",
                Damage = 45,
                Description = "Lance une boule de feu dévastatrice."
            };
            var frostbolt = new Spell
            {
                Name = "Givre",
                Damage = 35,
                Description = "Ralentit et gèle l'ennemi."
            };
            var arcane = new Spell
            {
                Name = "Éclat arcane",
                Damage = 55,
                Description = "Énergie pure qui traverse les défenses."
            };
            var meteor = new Spell
            {
                Name = "Météore",
                Damage = 70,
                Description = "Fait tomber un météore sur l'ennemi."
            };

            var slash = new Spell
            {
                Name = "Taille",
                Damage = 50,
                Description = "Coup d'épée puissant."
            };
            var shield = new Spell
            {
                Name = "Bouclier",
                Damage = 20,
                Description = "Pare et contre-attaque."
            };
            var charge = new Spell
            {
                Name = "Charge",
                Damage = 60,
                Description = "Fonce sur l'ennemi de plein fouet."
            };
            var warcry = new Spell
            {
                Name = "Cri de guerre",
                Damage = 40,
                Description = "Frappe en renforçant sa propre attaque."
            };

            var backstab = new Spell
            {
                Name = "Coup dans le dos",
                Damage = 65,
                Description = "Frappe en traître par derrière."
            };
            var poison = new Spell
            {
                Name = "Poison",
                Damage = 30,
                Description = "Empoisonne l'ennemi sur la durée."
            };
            var smoke = new Spell
            {
                Name = "Bombe fumigène",
                Damage = 25,
                Description = "Désorie l'ennemi et frappe."
            };
            var shadowstrike = new Spell
            {
                Name = "Frappe de l'ombre",
                Damage = 75,
                Description = "Sort de l'ombre pour un coup fatal."
            };

            var mage = new Hero
            {
                Name = "Mage",
                Health = 800,
                ImageURL = null,
                Spells = new List<Spell> { fireball, frostbolt, arcane, meteor }
            };

            var warrior = new Hero
            {
                Name = "Guerrier",
                Health = 1200,
                ImageURL = null,
                Spells = new List<Spell> { slash, shield, charge, warcry }
            };

            var assassin = new Hero
            {
                Name = "Assassin",
                Health = 900,
                ImageURL = null,
                Spells = new List<Spell> { backstab, poison, smoke, shadowstrike }
            };

            ctx.Heroes.AddRange(mage, warrior, assassin);

            if (!await ctx.Logins.AnyAsync(l => l.Username == "admin"))
            {
                var adminLogin = new Login
                {
                    Username = "admin",
                    PasswordHash = HashPassword("admin123")
                };
                var adminPlayer = new Player { Name = "Admin", Login = adminLogin };
                ctx.Logins.Add(adminLogin);
                ctx.Players.Add(adminPlayer);
            }

            await ctx.SaveChangesAsync();
        }
    }
}