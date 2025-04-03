using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.Threading.Tasks;
using DiscordLikeChatApp.Services;
using DiscordLikeChatApp.Models;

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
            DisplayUserSessionProperties(); // Afficher les propriétés de la session utilisateur
            string messageText = MessageTextBox.Text;
            if (!string.IsNullOrEmpty(messageText)) {
                try {
                    // Vérifier que la destination est bien définie
                    if (string.IsNullOrEmpty(_webSocketClientService.Destination) && !string.IsNullOrEmpty(ChannelId)) {
                        _webSocketClientService.Destination = $"/app/channel/{ChannelId}/send";
                    }
                    var messagePayload = new {
                        content = messageText,
                        timestamp = DateTime.UtcNow,
                        channelId = ChannelId,
                        sender = _userSession.Get<string>("Username"),
                    };
                    string jsonMessage = JsonSerializer.Serialize(messagePayload);
                    string authToken = _userSession.Get<string>("Authorization");
                    await _webSocketClientService.SendJsonMessageAsync(jsonMessage, authToken);
                    MessageTextBox.Clear();
                    Console.WriteLine(jsonMessage);
                }
                catch (Exception ex) {
                    MessageBox.Show("Erreur lors de l'envoi du message : " + ex.Message);
                }
            }
        }
        private void DisplayUserSessionProperties() {
            var properties = _userSession.GetAllProperties(); // Assurez-vous que UserSession a une méthode GetAllProperties()
            foreach (var property in properties) {
                Console.WriteLine($"{property.Key}: {property.Value}");
            }
        }

    }
}
