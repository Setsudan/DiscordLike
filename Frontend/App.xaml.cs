using System.Windows;
using DiscordHeticWpf.Views;

namespace DiscordHeticWpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // For now, since session management isn't fully implemented,
            // we assume there's no saved session and always open the LoginView.
            // Later you can check for a token from user settings or a session manager.
            LoginView loginView = new LoginView();
            loginView.Show();
        }
    }
}
