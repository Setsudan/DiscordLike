using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using DiscordLikeChatApp.Services;

namespace DiscordLikeChatApp.Views {
    public partial class ChatView : UserControl {
        private WebSocketClientService _webSocketClientService;
        private ApiService _apiService;
        public string ChannelId {
            get; set;
        }

        public ChatView() {
            InitializeComponent();
            InitializeWebSocket();
        }
        public ChatView(ApiService apiService, string channelId) : this() {
            _apiService = apiService;
            ChannelId = channelId;
        }
        private async void InitializeWebSocket() {
            _webSocketClientService = new WebSocketClientService();
            _webSocketClientService.MessageReceived += OnWebSocketMessageReceived;
            try {
                await _webSocketClientService.ConnectAsync("wss://discord-backend.ethlny.net/ws");
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur de connexion WebSocket : " + ex.Message);
            }
        }

        // Méthode appelée quand un message est reçu
        private void OnWebSocketMessageReceived(string message) {
            // Utilisation du Dispatcher pour mettre à jour l'interface utilisateur depuis un thread non-UI.
            Dispatcher.Invoke(() => {
                MessagesListBox.Items.Add(message);
            });
        }

        // Envoi d'un message via WebSocket
        private async void OnSendButtonClick(object sender, RoutedEventArgs e) {
            string messageText = MessageTextBox.Text;
            if (!string.IsNullOrEmpty(messageText)) {
                try {
                    await _webSocketClientService.SendMessageAsync(messageText);
                    MessageTextBox.Clear();
                }
                catch (Exception ex) {
                    MessageBox.Show("Erreur lors de l'envoi du message : " + ex.Message);
                }
            }
        }
    }
}
