using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Windows;

namespace HeroArena
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var login = new Views.LoginView();
            login.Show();
        }
    }
}