using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HeroArena.Data;
using HeroArena.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroArena.ViewModels
{
    public class HeroViewModel : BaseViewModel
    {
        private readonly string _connectionString;
        private Hero? _selectedHero;
        private Hero? _chosenHero;
        private string _filterHeroName = "Tous";

        public ObservableCollection<Hero> Heroes { get; } = new();
        public ObservableCollection<Spell> FilteredSpells { get; } = new();
        public ObservableCollection<string> HeroNames { get; } = new();

        public Hero? SelectedHero
        {
            get => _selectedHero;
            set
            {
                SetProperty(ref _selectedHero, value);
                OnPropertyChanged(nameof(SelectedSpells));
            }
        }

        public Hero? ChosenHero
        {
            get => _chosenHero;
            set => SetProperty(ref _chosenHero, value);
        }

        public IEnumerable<Spell> SelectedSpells =>
            _selectedHero?.Spells ?? Enumerable.Empty<Spell>();

        public string FilterHeroName
        {
            get => _filterHeroName;
            set
            {
                SetProperty(ref _filterHeroName, value);
                ApplySpellFilter();
            }
        }

        public RelayCommand ChooseHeroCommand { get; }

        public HeroViewModel(string connectionString)
        {
            _connectionString = connectionString;
            ChooseHeroCommand = new RelayCommand(_ =>
            {
                if (SelectedHero is not null)
                    ChosenHero = SelectedHero;
            });
        }

        public async Task LoadAsync()
        {
            try
            {
                using var ctx = new HeroArenaContext(_connectionString);
                var heroes = await ctx.Heroes
                    .Include(h => h.Spells)
                    .ToListAsync();

                Heroes.Clear();
                foreach (var h in heroes)
                    Heroes.Add(h);

                HeroNames.Clear();
                HeroNames.Add("Tous");
                foreach (var h in heroes)
                    HeroNames.Add(h.Name);

                if (Heroes.Any())
                    SelectedHero = Heroes[0];

                ApplySpellFilter();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Erreur : {ex.Message}\nInner: {ex.InnerException?.Message}",
                    "Erreur chargement");
            }
        }

        private void ApplySpellFilter()
        {
            FilteredSpells.Clear();

            var spells = FilterHeroName == "Tous"
                ? Heroes.SelectMany(h => h.Spells)
                : Heroes.FirstOrDefault(h => h.Name == FilterHeroName)?.Spells
                  ?? Enumerable.Empty<Spell>();

            foreach (var s in spells)
                FilteredSpells.Add(s);
        }
    }
}