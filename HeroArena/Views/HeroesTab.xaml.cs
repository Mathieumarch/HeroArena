using HeroArena.ViewModels;
using System.Windows.Controls;

namespace HeroArena.Views
{
    public partial class HeroesTab : UserControl
    {
        public HeroesTab() => InitializeComponent();

        public void SetViewModel(HeroViewModel vm)
        {
            DataContext = vm;
            System.Diagnostics.Debug.WriteLine(
                $"DataContext défini : {vm != null}");
        }
    }
}