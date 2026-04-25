using System.Windows.Controls;
using HeroArena.ViewModels;

namespace HeroArena.Views
{
    public partial class CombatView : UserControl
    {
        public CombatView() => InitializeComponent();

        public void SetViewModel(CombatViewModel vm) => DataContext = vm;
    }
}