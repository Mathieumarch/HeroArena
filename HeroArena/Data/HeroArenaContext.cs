using HeroArena.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroArena.Data
{
    public class HeroArenaContext : DbContext
    {
        private readonly string _connectionString;

        public HeroArenaContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Login> Logins { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Spell> Spells { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Login>().ToTable("Login");
            modelBuilder.Entity<Player>().ToTable("Player");
            modelBuilder.Entity<Hero>().ToTable("Hero");
            modelBuilder.Entity<Spell>().ToTable("Spell");

            // Relation Player <-> Hero
            modelBuilder.Entity<Player>()
                .HasMany(p => p.Heroes)
                .WithMany(h => h.Players)
                .UsingEntity(j =>
                {
                    j.ToTable("PlayerHero");
                    j.Property<int>("PlayersID").HasColumnName("PlayerID");
                    j.Property<int>("HeroesID").HasColumnName("HeroID");
                });

            // Relation Hero <-> Spell — colonnes exactes de ta BDD
            modelBuilder.Entity<Hero>()
                .HasMany(h => h.Spells)
                .WithMany(s => s.Heroes)
                .UsingEntity(j =>
                {
                    j.ToTable("HeroSpell");
                    j.Property<int>("HeroesID").HasColumnName("HeroID");
                    j.Property<int>("SpellsID").HasColumnName("SpellID");
                });

            // Relation Login -> Player
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Login)
                .WithOne(l => l.Player)
                .HasForeignKey<Player>(p => p.LoginID);
        }
    }
}