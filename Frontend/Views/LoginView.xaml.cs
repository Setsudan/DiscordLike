using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DiscordLikeChatApp.Services;
using Microsoft.Extensions.Configuration;
using System;
using DiscordLikeChatApp.Models;

namespace DiscordLikeChatApp.Views {
    /// <summary>
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl {
        private readonly AuthService _authService;
        private readonly UserSession _userSession;
        private readonly IConfiguration _configuration;

        public LoginView() {
            InitializeComponent();
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _authService = new AuthService(configuration, new HttpClient());
            _userSession = new UserSession();
        }

        private async Task<AccessTokenResponse> LoginAsync(string username, string password) {
            try {
                AccessTokenResponse response = await _authService.LoginAsync(username, password);
                if (response.IsSuccess) {
                    MessageBox.Show("Connexion réussie.");
                    _userSession.Set("Username", username);
                    _userSession.Set("Authorization", response.AccessToken);
                }
                else {
                    MessageBox.Show("Connexion échouée : " + response.ErrorMessage);
                }
                return response;
            }
            catch (Exception ex) {
                MessageBox.Show("Une erreur s'est produite : " + ex.Message);
                return new AccessTokenResponse { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            AccessTokenResponse response = await LoginAsync(username, password);

            if (response.IsSuccess) {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.Content = new DashboardView(_userSession, _configuration);
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
