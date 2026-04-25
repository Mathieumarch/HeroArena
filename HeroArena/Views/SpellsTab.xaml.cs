using System.Windows.Controls;
using HeroArena.ViewModels;

namespace HeroArena.Views
{
    public partial class SpellsTab : UserControl
    {
        public SpellsTab() => InitializeComponent();

        public void SetViewModel(HeroViewModel vm) => DataContext = vm;
    }
}