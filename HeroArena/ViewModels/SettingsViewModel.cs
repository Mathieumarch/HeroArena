using HeroArena.Data;
using HeroArena.Services;
using System;
using System.Threading.Tasks;

namespace HeroArena.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private string _connectionString;
        private string _statusMessage = string.Empty;
        private bool _isBusy;

        public string ConnectionString
        {
            get => _connectionString;
            set => SetProperty(ref _connectionString, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand SeedCommand { get; }
        public RelayCommand TestCommand { get; }

        public event Action? SettingsSaved;

        public SettingsViewModel()
        {
            var s = AppSettings.Load();
            _connectionString = s.ConnectionString;

            SaveCommand = new RelayCommand(_ => Save());
            SeedCommand = new RelayCommand(async _ => await SeedAsync());
            TestCommand = new RelayCommand(async _ => await TestConnectionAsync());
        }

        private void Save()
        {
            var s = AppSettings.Load();
            s.ConnectionString = ConnectionString;
            AppSettings.Save(s);
            StatusMessage = "Paramètres sauvegardés.";
            SettingsSaved?.Invoke();
        }

        private async Task TestConnectionAsync()
        {
            IsBusy = true;
            StatusMessage = "Test en cours...";
            try
            {
                using var ctx = new HeroArenaContext(ConnectionString);
                await ctx.Database.CanConnectAsync()
                    .ContinueWith(t =>
                    {
                        StatusMessage = t.Result
                            ? "Connexion réussie !"
                            : "Impossible de se connecter.";
                    });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SeedAsync()
        {
            IsBusy = true;
            StatusMessage = "Initialisation des données...";
            try
            {
                var auth = new AuthService(ConnectionString);
                await auth.SeedDefaultDataAsync();
                StatusMessage = "Données initialisées avec succès !";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}