using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DiscordFrontEnd.Models;
using Newtonsoft.Json;

namespace DiscordLikeChatApp.Views {
    public partial class RegisterView : UserControl {
        private static readonly HttpClient client = new HttpClient();

        public RegisterView() {
            InitializeComponent();
        }

        private async void SignupButton_Click(object sender, RoutedEventArgs e) {
            var username = UsernameTextBox.Text.Trim();
            var email = EmailTextBox.Text.Trim();
            var password = PasswordBox.Password;
            var rolesText = RolesTextBox.Text.Trim();

            // Split roles by commas; if empty, default to USER.
            string[] roles;
            if (!string.IsNullOrWhiteSpace(rolesText)) {
                roles = rolesText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < roles.Length; i++) {
                    roles[i] = roles[i].Trim();
                }
            }
            else {
                roles = new string[] { "USER" };
            }

            var signupData = new {
                username = username,
                email = email,
                password = password,
                roles = roles
            };

            var json = JsonConvert.SerializeObject(signupData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try {
                // Post to the signup endpoint.
                var response = await client.PostAsync("http://localhost:8080/auth/signup", content);
                if (response.IsSuccessStatusCode) {
                    MessageBox.Show("Inscription réussie !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur lors de l'inscription : {responseBody}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Exception: " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            // Navigation logic to go to the login view
            // For example, if you're using a Frame:
            // NavigationService.GetNavigationService(this)?.Navigate(new LoginView());
            e.Handled = true;
        }
    }
}
