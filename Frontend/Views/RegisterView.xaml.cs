using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DiscordLikeChatApp.Services;
using Microsoft.Extensions.Configuration;

namespace DiscordLikeChatApp.Views {
    public partial class RegisterView : UserControl {
        private readonly AuthService _authService;

        public RegisterView() {
            InitializeComponent();
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _authService = new AuthService(configuration, new HttpClient());
        }

        private async void SignupButton_Click(object sender, RoutedEventArgs e) {
            var username = UsernameTextBox.Text.Trim();
            var email = EmailTextBox.Text.Trim();
            var password = PasswordBox.Password;

            try {
                bool success = await _authService.RegisterAsync(username, email, password);
                if (success) {
                    MessageBox.Show("Inscription réussie !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else {
                    MessageBox.Show("Erreur lors de l'inscription.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Exception: " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            if (e.Uri.ToString() == "login") {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.Content = new LoginView();
            }
            else {
                MessageBox.Show("Navigating to: " + e.Uri.ToString());
            }
            e.Handled = true;
        }
    }
}
