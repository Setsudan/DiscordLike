using System.Windows;
using System.Windows.Controls;

namespace DiscordLikeChatApp.Views.Components {
    /// <summary>
    /// Logique d'interaction pour ChannelManager.xaml
    /// </summary>
    public partial class ChannelManager : UserControl {
        public ChannelManager() {
            InitializeComponent();
        }

        private void OnPromoteToModeratorClick(object sender, RoutedEventArgs e) {
            // Logique pour nommer un modérateur
            MessageBox.Show("Nommer Modérateur");
        }

        private void OnDemoteFromModeratorClick(object sender, RoutedEventArgs e) {
            // Logique pour retirer un modérateur
            MessageBox.Show("Retirer Modérateur");
        }

        private void OnBanUserClick(object sender, RoutedEventArgs e) {
            // Logique pour bloquer/bannir un utilisateur
            MessageBox.Show("Bloquer/Bannir Utilisateur");
        }

        private void OnSaveChannelSettingsClick(object sender, RoutedEventArgs e) {
            // Logique pour enregistrer les modifications des paramètres du salon
            string channelName = ChannelNameTextBox.Text;
            string channelDescription = ChannelDescriptionTextBox.Text;
            MessageBox.Show($"Enregistrer les Modifications: {channelName}, {channelDescription}");
        }
    }
}

