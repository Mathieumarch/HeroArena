using System;
using System.Threading.Tasks;
using System.Windows;
using HeroArena.Data;
using HeroArena.Models;
using HeroArena.Services;

namespace HeroArena.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _auth;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isBusy;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public RelayCommand LoginCommand { get; }
        public RelayCommand RegisterCommand { get; }
        public RelayCommand OpenSettingsCommand { get; }

        public event Action<Login>? LoginSuccess;
        public event Action? SettingsRequested;

        public LoginViewModel()
        {
            var settings = AppSettings.Load();
            _auth = new AuthService(settings.ConnectionString);

            LoginCommand = new RelayCommand(async _ => await DoLoginAsync());
            RegisterCommand = new RelayCommand(async _ => await DoRegisterAsync());
            OpenSettingsCommand = new RelayCommand(_ => SettingsRequested?.Invoke());
        }

        private async Task DoLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Veuillez remplir tous les champs.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var login = await _auth.LoginAsync(Username, Password);
                if (login is null)
                    ErrorMessage = "Identifiants incorrects.";
                else
                    LoginSuccess?.Invoke(login);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur de connexion à la base : {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DoRegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Veuillez remplir tous les champs.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var ok = await _auth.RegisterAsync(Username, Password);
                ErrorMessage = ok
                    ? "Compte créé ! Vous pouvez vous connecter."
                    : "Ce nom d'utilisateur existe déjà.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}