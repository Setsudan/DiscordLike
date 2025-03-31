using System.Windows;
using System.Windows.Controls;

namespace DiscordLikeChatApp.Views.Components {
    /// <summary>
    /// Logique d'interaction pour UserSettings.xaml
    /// </summary>
    public partial class UserSettings : UserControl {
        public UserSettings() {
            InitializeComponent();
        }

        private void OnChangeAvatarClick(object sender, RoutedEventArgs e) {
            // Logique pour changer l'avatar
            MessageBox.Show("Changer Avatar");
        }

        private void OnChangePasswordClick(object sender, RoutedEventArgs e) {
            // Logique pour changer le mot de passe
            string currentPassword = CurrentPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (newPassword == confirmPassword) {
                MessageBox.Show("Mot de passe changé avec succès");
            }
            else {
                MessageBox.Show("Les nouveaux mots de passe ne correspondent pas");
            }
        }

        private void OnSaveSettingsClick(object sender, RoutedEventArgs e) {
            // Logique pour enregistrer les réglages de notification
            bool emailNotifications = EmailNotificationsCheckBox.IsChecked ?? false;
            bool smsNotifications = SmsNotificationsCheckBox.IsChecked ?? false;

            MessageBox.Show("Réglages enregistrés");
        }
    }
}