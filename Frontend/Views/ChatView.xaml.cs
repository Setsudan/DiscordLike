using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.Threading.Tasks;
using DiscordLikeChatApp.Services;
using DiscordLikeChatApp.Models;
using DiscordFrontEnd.Models;

namespace DiscordLikeChatApp.Views {
    public partial class ChatView : UserControl {
        private StompWebSocketClientService _webSocketClientService;
        private ApiService _apiService;
        private UserSession _userSession; 

    
        public string ChannelName {
            get; set;
        }
 
        public string ChannelId {
            get; set;
        }

    
        public ChatView() {
            InitializeComponent();
        }

        public ChatView(ApiService apiService, UserSession userSession, string channelName) : this() {
            _apiService = apiService;
            _userSession = userSession; 
            ChannelName = channelName;
            string authToken = _userSession.Get<string>("Authorization");
            this.Loaded += async (s, e) => {
                await RetrieveChannelIdAsync();
                // Définir la destination du WebSocket une fois le ChannelId récupéré
                if (!string.IsNullOrEmpty(ChannelId)) {
                    _webSocketClientService.Destination = $"/app/channel/{ChannelId}/send";
                }
            };
            InitializeWebSocket(authToken);
        }

        private async void InitializeWebSocket(string authToken) {
            _webSocketClientService = new StompWebSocketClientService();
            _webSocketClientService.MessageReceived += OnWebSocketMessageReceived;
            try {
                await _webSocketClientService.ConnectAsync("wss://discord-backend.ethlny.net/ws");
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur de connexion WebSocket : " + ex.Message);
            }
        }

        private async Task RetrieveChannelIdAsync() {
            try {
                var channels = await _apiService.GetAsync<List<Channel>>("/channels");
                // Chercher le canal dont le nom correspond (insensible à la casse)
                var channelObj = channels?.Find(ch => ch.Name.Equals(ChannelName, StringComparison.OrdinalIgnoreCase));
                if (channelObj != null) {
                    ChannelId = channelObj.Id.ToString();
                }
                else {
                    MessageBox.Show($"Channel '{ChannelName}' non trouvé.");
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur lors de la récupération de l'identifiant du canal : " + ex.Message);
            }
        }

        private void OnWebSocketMessageReceived(string message) {
            Dispatcher.Invoke(() => {
                MessagesListBox.Items.Add(message);
            });
        }

        // Envoi d'un message via WebSocket
        private async void OnSendButtonClick(object sender, RoutedEventArgs e) {
            var username = _userSession.Get<string>("Username");
            var userId = await RetrieveUserIdAsync(username);
            string messageText = MessageTextBox.Text;
            if (!string.IsNullOrEmpty(messageText) && !string.IsNullOrEmpty(userId)) {
                try {
                    // Vérifier que la destination est bien définie
                    if (string.IsNullOrEmpty(_webSocketClientService.Destination) && !string.IsNullOrEmpty(ChannelId)) {
                        _webSocketClientService.Destination = $"/app/channel/{ChannelId}/send";
                    }
                    var messagePayload = new {
                        content = messageText,
                        timestamp = DateTime.UtcNow,
                        channelId = ChannelId,
                        sender = userId,
                    };
                    string jsonMessage = JsonSerializer.Serialize(messagePayload);
                    string authToken = _userSession.Get<string>("Authorization");
                    await _webSocketClientService.SendJsonMessageAsync(jsonMessage, authToken);
                    MessageTextBox.Clear();
                }
                catch (Exception ex) {
                    MessageBox.Show("Erreur lors de l'envoi du message : " + ex.Message);
                }
            }
        }
        private async Task<string> RetrieveUserIdAsync(string userName) {
            try {
                var users = await _apiService.GetAsync<List<User>>("/users");
                var userObj = users?.FirstOrDefault(u => u.Username.Equals(userName, StringComparison.OrdinalIgnoreCase));
                if (userObj != null) {
                    return userObj.Id.ToString();
                }
                else {
                    MessageBox.Show($"Utilisateur '{userName}' non trouvé.");
                    return null;
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur lors de la récupération de l'identifiant de l'utilisateur : " + ex.Message);
                return null;
            }
        }
    }
}
