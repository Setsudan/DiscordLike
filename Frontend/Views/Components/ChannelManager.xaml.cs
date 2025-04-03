using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DiscordFrontEnd.Models;
using DiscordLikeChatApp.Models;
using DiscordLikeChatApp.Services;

namespace DiscordLikeChatApp.Views.Components {
    /// <summary>
    /// Logique d'interaction pour ChannelManager.xaml
    /// </summary>
    public partial class ChannelManager : UserControl {
        private readonly ApiService _apiService;
        public string _channelId;
        private readonly UserSession _userSession;
        private readonly string _authToken;

        public ChannelManager(string channelId, ApiService apiService, UserSession userSession) {
            InitializeComponent();
            _channelId = channelId;
            _apiService = apiService;
            _userSession = userSession;
            _authToken = _userSession.Get<string>("Authorization");
            LoadMembers();
        }
        private async void LoadMembers() {
            try {
                var members = await _apiService.GetAsync<List<User>>($"/users");
                MembersListBox.Items.Clear();
                foreach (var member in members) {
                    MembersListBox.Items.Add(new ListBoxItem { Content = member.Username });
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur lors du chargement des membres: " + ex.Message);
            }
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

        private async void OnDeleteChannelClick(object sender, RoutedEventArgs e) {
            var channelId = await RetrieveChannelId(_channelId);
            if (channelId != null) {
                await _apiService.DeleteChannelAsync(channelId);
            }
        }
        private async Task<string> RetrieveChannelId(string channelName) {
            try {
                var channels = await _apiService.GetAsync<List<Channel>>("/channels");
                var channelObj = channels?.Find(ch => ch.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));
                if (channelObj != null) {
                    return channelObj.Id.ToString();
                }
                else {
                    MessageBox.Show($"Channel '{channelName}' non trouvé.");
                    return null;
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur lors de la récupération de l'identifiant du canal : " + ex.Message);
                return null;
            }
        }
    }
}

