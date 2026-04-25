using System;
using System.Collections.Generic;
using System.Linq;
using HeroArena.Models;
namespace HeroArena.Services
{
    public class CombatResult
    {
        public string AttackerName { get; set; } = string.Empty;
        public string SpellName { get; set; } = string.Empty;
        public int Damage { get; set; }
        public string Log => $"{AttackerName} utilise {SpellName} et inflige {Damage} dégâts !";
    }

    public class CombatService
    {
        private readonly Random _rng = new();

        public (int hp, int maxHp, List<Spell> spells) CreateEnemy(Hero baseHero)
        {
            int boostedHp = (int)(baseHero.Health * 1.10);
            var boostedSpells = baseHero.Spells.Select(s => new Spell
            {
                ID = s.ID,
                Name = s.Name,
                Damage = (int)(s.Damage * 1.05),
                Description = s.Description
            }).ToList();

            return (boostedHp, boostedHp, boostedSpells);
        }

        public CombatResult PlayerAttack(string playerName, Spell spell)
        {
            return new CombatResult
            {
                AttackerName = playerName,
                SpellName = spell.Name,
                Damage = spell.Damage
            };
        }

        public CombatResult EnemyAttack(string enemyName, List<Spell> enemySpells)
        {
            var spell = enemySpells[_rng.Next(enemySpells.Count)];
            return new CombatResult
            {
                AttackerName = enemyName,
                SpellName = spell.Name,
                Damage = spell.Damage
            };
        }

        public bool IsDead(int currentHp) => currentHp <= 0;
    }
}