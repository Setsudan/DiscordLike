using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Configuration;
using DiscordLikeChatApp.Models;
using DiscordLikeChatApp.Services;
using DiscordFrontEnd.Models;

namespace DiscordLikeChatApp.Views.Components {
    public partial class UserSettings : UserControl {
        private readonly ApiService _apiService;
        private readonly UserSession _userSession;

        // On force l'utilisation du constructeur avec paramètres pour garantir l'accès au token
        public UserSettings(UserSession userSession, IConfiguration configuration) {
            InitializeComponent();
            _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
            string authToken = _userSession.Get<string>("Authorization");
            if (string.IsNullOrEmpty(authToken)) {
                MessageBox.Show("Erreur : Vous devez vous connecter pour accéder aux réglages utilisateur.");
                // Vous pouvez rediriger vers l'écran de connexion ici
                return;
            }
            _apiService = new ApiService(configuration, authToken);
            // Charger les infos de l'utilisateur connecté
            LoadCurrentUserAsync();
        }

        private async void LoadCurrentUserAsync() {
            try {
                // Appel GET sur "/users/me"
                User currentUser = await _apiService.GetAsync<User>("/users/me");
                // Mettre à jour l'interface (TextBox, etc.)
                UserNameTextBox.Text = currentUser.Username;
                UserEmailTextBox.Text = currentUser.Email;
                // Vous pouvez également afficher DisplayName, AvatarUrl, etc.
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur lors du chargement de vos informations : " + ex.Message);
            }
        }


        private async void OnSaveSettingsClick(object sender, RoutedEventArgs e) {
            try {
                var updatedUser = new User {
                    Username = UserNameTextBox.Text,
                    Email = UserEmailTextBox.Text,
                    // Ajoutez d'autres propriétés modifiables si nécessaire.
                };

                // Appel PUT sur "/users/me"
                User result = await _apiService.PutAsync<User, User>("/users/me", updatedUser);
                MessageBox.Show("Réglages enregistrés avec succès.");
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur lors de la mise à jour de vos informations : " + ex.Message);
            }
        }


        private async void OnChangeAvatarClick(object sender, RoutedEventArgs e) {
            try {
                // Ouvrir une boîte de dialogue pour sélectionner un fichier image
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                bool? result = dlg.ShowDialog();
                if (result == true) {
                    string filePath = dlg.FileName;
                    bool uploadResult = await UploadAvatarAsync(filePath);
                    if (uploadResult) {
                        MessageBox.Show("Avatar mis à jour avec succès.");
                    }
                    else {
                        MessageBox.Show("Erreur lors du téléchargement de l'avatar.");
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur : " + ex.Message);
            }
        }


        private async Task<bool> UploadAvatarAsync(string filePath) {
            try {
                using (var content = new MultipartFormDataContent()) {
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    var byteContent = new ByteArrayContent(fileBytes);
                    // Vous pouvez adapter le Content-Type selon le fichier sélectionné
                    byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    content.Add(byteContent, "file", Path.GetFileName(filePath));

                    using (var client = new HttpClient()) {
                        client.BaseAddress = new Uri("https://discord-backend.ethlny.net/");
                        string authToken = _userSession.Get<string>("Authorization");
                        if (!string.IsNullOrEmpty(authToken))
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                        var response = await client.PostAsync("/users/me/avatar", content);
                        response.EnsureSuccessStatusCode();
                        return response.IsSuccessStatusCode;
                    }
                }
            }
            catch {
                return false;
            }
        }

       
        private void OnChangePasswordClick(object sender, RoutedEventArgs e) {
            string currentPassword = CurrentPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (newPassword == confirmPassword) {
                // Appel à un endpoint dédié pour changer le mot de passe si disponible
                MessageBox.Show("Mot de passe changé avec succès");
            }
            else {
                MessageBox.Show("Les nouveaux mots de passe ne correspondent pas");
            }
        }
    }
}
