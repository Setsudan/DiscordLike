using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DiscordLikeChatApp.Models;
using DiscordLikeChatApp.Services;

namespace DiscordLikeChatApp.Views {
    public partial class ChatView : UserControl {
        private readonly DispatcherTimer _pollTimer;
        private readonly ApiService _apiService;

        // Identifiant du canal à charger (en string, qui sera converti en Guid)
        public string ChannelId {
            get; set;
        }

        public ChatView(ApiService apiService, string channelId) {
            InitializeComponent();
            _apiService = apiService;
            ChannelId = channelId;

            _pollTimer = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(2)
            };
            _pollTimer.Tick += async (s, e) => await FetchMessagesAsync();
            _pollTimer.Start();

            _ = FetchMessagesAsync();
        }

        // Méthode pour récupérer les messages du canal via GET
        private async Task FetchMessagesAsync() {
            try {
                // Appel GET à /messages/channel/{ChannelId}
                var messages = await _apiService.GetAsync<List<Message>>($"/messages/channel/{ChannelId}");

                MessagesListBox.Items.Clear();
                foreach (var msg in messages) {
                    MessagesListBox.Items.Add($"{msg.Content}");
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Erreur lors de la récupération des messages: " + ex.Message);
            }
        }

        // Envoi d'un message via POST
        private async void OnSendButtonClick(object sender, RoutedEventArgs e) {
            string messageText = MessageTextBox.Text;
            if (!string.IsNullOrEmpty(messageText)) {
                var messageRequest = new Message {
                    ChannelId = Guid.Parse(ChannelId), // Utilise la propriété ChannelId convertie en Guid
                    Content = messageText,
                };

                try {
                    var sentMessage = await _apiService.PostAsync<Message, Message>("/messages", messageRequest);
                    MessageTextBox.Clear();
                    await FetchMessagesAsync();
                }
                catch (Exception ex) {
                    MessageBox.Show("Erreur lors de l'envoi du message : " + ex.Message);
                }
            }
        }
    }
}
