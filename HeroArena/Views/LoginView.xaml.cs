using System.Windows;
using System.Windows.Controls;
using HeroArena.ViewModels;

namespace HeroArena.Views
{
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _vm;

        public LoginView()
        {
            InitializeComponent();
            _vm = new LoginViewModel();
            DataContext = _vm;

            _vm.LoginSuccess += login =>
            {
                var main = new MainView(login);
                main.Show();
                Close();
            };

            _vm.SettingsRequested += () =>
            {
                var settings = new SettingsView();
                settings.ShowDialog();
            };
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            _vm.Password = PasswordBox.Password;
            _vm.LoginCommand.Execute(null);
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            _vm.Password = PasswordBox.Password;
            _vm.RegisterCommand.Execute(null);
        }
    }
}