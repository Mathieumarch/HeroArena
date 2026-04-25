using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using HeroArena.Models;
using HeroArena.Services;

namespace HeroArena.ViewModels
{
    public class CombatViewModel : BaseViewModel
    {
        private readonly CombatService _combat = new();
        private readonly DispatcherTimer _timer = new();

        private Hero? _playerHero;
        private int _playerHp;
        private int _playerMaxHp;
        private int _enemyHp;
        private int _enemyMaxHp;
        private List<Spell> _enemySpells = new();
        private int _timeLeft = 60;
        private int _score;
        private string _statusMessage = string.Empty;
        private bool _isPlayerTurn = true;
        private bool _combatActive;

        public Hero? PlayerHero
        {
            get => _playerHero;
            set => SetProperty(ref _playerHero, value);
        }

        public int PlayerHp
        {
            get => _playerHp;
            set { SetProperty(ref _playerHp, value); OnPropertyChanged(nameof(PlayerHpPercent)); }
        }

        public int PlayerMaxHp
        {
            get => _playerMaxHp;
            set { SetProperty(ref _playerMaxHp, value); OnPropertyChanged(nameof(PlayerHpPercent)); }
        }

        public int EnemyHp
        {
            get => _enemyHp;
            set { SetProperty(ref _enemyHp, value); OnPropertyChanged(nameof(EnemyHpPercent)); }
        }

        public int EnemyMaxHp
        {
            get => _enemyMaxHp;
            set { SetProperty(ref _enemyMaxHp, value); OnPropertyChanged(nameof(EnemyHpPercent)); }
        }

        public double PlayerHpPercent =>
            PlayerMaxHp == 0 ? 0 : (double)PlayerHp / PlayerMaxHp * 100;

        public double EnemyHpPercent =>
            EnemyMaxHp == 0 ? 0 : (double)EnemyHp / EnemyMaxHp * 100;

        public int TimeLeft
        {
            get => _timeLeft;
            set => SetProperty(ref _timeLeft, value);
        }

        public int Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsPlayerTurn
        {
            get => _isPlayerTurn;
            set => SetProperty(ref _isPlayerTurn, value);
        }

        public bool CombatActive
        {
            get => _combatActive;
            set => SetProperty(ref _combatActive, value);
        }

        public ObservableCollection<string> CombatLog { get; } = new();

        public RelayCommand UseSpellCommand { get; }
        public RelayCommand NewCombatCommand { get; }

        public CombatViewModel()
        {
            UseSpellCommand = new RelayCommand(p =>
            {
                if (p is Spell spell) PlayerAttack(spell);
            }, _ => IsPlayerTurn && CombatActive);

            NewCombatCommand = new RelayCommand(_ => StartCombat());

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
        }

        public void SetHero(Hero hero)
        {
            PlayerHero = hero;
            StartCombat();
        }

        public void StartCombat()
        {
            if (PlayerHero is null) return;

            _timer.Stop();

            PlayerHp = PlayerHero.Health;
            PlayerMaxHp = PlayerHero.Health;

            var (eHp, eMax, eSpells) = _combat.CreateEnemy(PlayerHero);
            EnemyHp = eHp;
            EnemyMaxHp = eMax;
            _enemySpells = eSpells;

            CombatLog.Clear();
            IsPlayerTurn = true;
            CombatActive = true;
            TimeLeft = 60;
            StatusMessage = "À vous de jouer ! Choisissez un sort.";

            _timer.Start();
        }

        private void OnTimerTick(object? s, EventArgs e)
        {
            TimeLeft--;
            if (TimeLeft <= 0)
            {
                // Temps écoulé : sort aléatoire automatique
                if (PlayerHero?.Spells?.Any() == true)
                {
                    var rnd = new Random();
                    var spell = PlayerHero.Spells.ElementAt(
                        rnd.Next(PlayerHero.Spells.Count));
                    PlayerAttack(spell);
                }
            }
        }

        private void PlayerAttack(Spell spell)
        {
            if (!CombatActive || !IsPlayerTurn) return;

            _timer.Stop();
            IsPlayerTurn = false;

            var result = _combat.PlayerAttack(PlayerHero!.Name, spell);
            EnemyHp = Math.Max(0, EnemyHp - result.Damage);
            CombatLog.Insert(0, result.Log);

            if (_combat.IsDead(EnemyHp))
            {
                CombatActive = false;
                Score++;
                StatusMessage = "Victoire ! L'ennemi est vaincu.";
                CombatLog.Insert(0, $"--- VICTOIRE ! Score : {Score} ---");
                return;
            }

            StatusMessage = "L'ennemi attaque...";

            // Délai 1 seconde avant l'attaque IA
            var delay = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            delay.Tick += (_, _) =>
            {
                delay.Stop();
                EnemyTurn();
            };
            delay.Start();
        }

        private void EnemyTurn()
        {
            var result = _combat.EnemyAttack(
                $"Ennemi ({PlayerHero!.Name})", _enemySpells);
            PlayerHp = Math.Max(0, PlayerHp - result.Damage);
            CombatLog.Insert(0, result.Log);

            if (_combat.IsDead(PlayerHp))
            {
                CombatActive = false;
                StatusMessage = "Défaite... Relancez un combat !";
                CombatLog.Insert(0, "--- DÉFAITE ---");
                return;
            }

            TimeLeft = 60;
            IsPlayerTurn = true;
            StatusMessage = "À vous de jouer ! Choisissez un sort.";
            _timer.Start();
        }
    }
}