﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DiscordLikeChatApp.Services;
using static DiscordFrontEnd.ViewModels.LoginViewModel;


namespace DiscordLikeChatApp.Views {
    /// <summary>
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl {
        private readonly ApiService _apiService;

        public LoginView() {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async Task<bool> LoginAsync(string username, string password) {
            try {
                bool success = await _apiService.LoginAsync(username, password);
                if (success) {
                    MessageBox.Show("Connexion réussie.");
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
                mainWindow.Content = new DashboardView();
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
