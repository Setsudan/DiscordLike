using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DiscordLikeChatApp.Services;
using Microsoft.Extensions.Configuration;
using System;

namespace DiscordLikeChatApp.Views {
    /// <summary>
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl {
        private readonly AuthService _authService;
        private readonly UserSession _userSession;

        public LoginView() {
            InitializeComponent();
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _authService = new AuthService(configuration, new HttpClient());
            _userSession = new UserSession();
        }

        private async Task<bool> LoginAsync(string username, string password) {
            try {
                bool success = await _authService.LoginAsync(username, password);
                if (success) {
                    MessageBox.Show("Connexion réussie.");
                    _userSession.Set("Username", username);
                }
                else {
                    MessageBox.Show("Connexion échouée. Veuillez réessayer.");
                }
                return success;
            }
            catch (Exception ex) {
                MessageBox.Show("Une erreur s'est produite : " + ex.Message);
                return false;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            bool success = await LoginAsync(username, password);

            if (success) {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.Content = new DashboardView(_userSession);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            if (e.Uri.ToString() == "register") {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.Content = new RegisterView();
            }
            else {
                MessageBox.Show("Navigating to: " + e.Uri.ToString());
            }
            e.Handled = true;
        }
    }
}
