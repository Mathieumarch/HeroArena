using System.Windows;
using HeroArena.Data;
using HeroArena.Models;
using HeroArena.ViewModels;

namespace HeroArena.Views
{
    public partial class MainView : Window
    {
        private readonly HeroViewModel _heroVm;
        private readonly CombatViewModel _combatVm;

        public MainView(Login login)
        {
            InitializeComponent();

            WelcomeText.Text = $"Connecté : {login.Username}";

            var connStr = AppSettings.Load().ConnectionString;
            _heroVm = new HeroViewModel(connStr);
            _combatVm = new CombatViewModel();

            // Lier les ViewModels AVANT le chargement
            HeroesTabControl.SetViewModel(_heroVm);
            SpellsTabControl.SetViewModel(_heroVm);
            CombatTabControl.SetViewModel(_combatVm);

            // Quand un héros est choisi -> combat
            _heroVm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(HeroViewModel.ChosenHero)
                    && _heroVm.ChosenHero is not null)
                {
                    _combatVm.SetHero(_heroVm.ChosenHero);
                    MainTab.SelectedIndex = 2;
                }
            };

            Loaded += async (_, _) => await _heroVm.LoadAsync();
        }
    }
}