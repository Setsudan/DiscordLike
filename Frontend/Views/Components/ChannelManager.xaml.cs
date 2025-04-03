using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DiscordLikeChatApp.Models;
using DiscordLikeChatApp.Services;

namespace DiscordLikeChatApp.Views.Components {
    /// <summary>
    /// Logique d'interaction pour ChannelManager.xaml
    /// </summary>
    public partial class ChannelManager : UserControl {
        private readonly ApiService _apiService;
        public string _channelId;
        public ChannelManager(string channelId, ApiService apiService) {
            InitializeComponent();
            _channelId = channelId;
            _apiService = apiService;
        }
        private void OnPromoteToModeratorClick(object sender, RoutedEventArgs e) {
            
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

        private async void OnSaveChannelSettingsClick(object sender, RoutedEventArgs e) {
            string channelName = ChannelNameTextBox.Text;
            string channelDescription = ChannelDescriptionTextBox.Text;
            await _apiService.PutAsync<Channel, Channel>("/channels", new Channel {
                Name = channelName,
            });
        }

        private async Task OnDeleteChannelClick(object sender, RoutedEventArgs e) {
            await _apiService.DeleteAsync($"/channels{_channelId}");
        }
    }
}

