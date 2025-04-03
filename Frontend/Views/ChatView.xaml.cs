using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using DiscordLikeChatApp.Services;
using DiscordLikeChatApp.Models;
using System.Threading.Channels;
using System.Text.Json;


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
            Dispatcher.Invoke(() => {
                MessagesListBox.Items.Add(message);
            });
        }

        // Envoi d'un message via WebSocket
        private async void OnSendButtonClick(object sender, RoutedEventArgs e) {
            string messageText = MessageTextBox.Text;
            if (!string.IsNullOrEmpty(messageText)) {
                try {
                    var message = new {
                        content = messageText,
                        timestamp = DateTime.UtcNow
                    };
                    string jsonMessage = System.Text.Json.JsonSerializer.Serialize(message);
                    await _webSocketClientService.SendMessageAsync(jsonMessage);
                    MessageTextBox.Clear();
                    Console.WriteLine(jsonMessage);

                    // Ajoutez immédiatement le message à la ListBox pour l'afficher
                    // Vous pouvez formater l'affichage comme vous le souhaitez.
                    MessagesListBox.Items.Add($"Moi: {messageText}");
                }
                catch (Exception ex) {
                    MessageBox.Show("Erreur lors de l'envoi du message : " + ex.Message);
                }
            }
        }
        //private async void GetChannelMessages(string channelId) {
        //    var messages = await _apiService.GetAsync<Message>($"messages/channel/{channelId}");
        //    Console.WriteLine(messages);
        //}
    }
}
