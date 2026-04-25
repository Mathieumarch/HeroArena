using System.Windows;
using HeroArena.ViewModels;

namespace HeroArena.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
            var vm = new SettingsViewModel();
            DataContext = vm;
            vm.SettingsSaved += Close;
        }
    }
}