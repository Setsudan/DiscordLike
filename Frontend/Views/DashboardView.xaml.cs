﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DiscordLikeChatApp.Models;
using DiscordLikeChatApp.Services;
using DiscordLikeChatApp.Views.Components;
using Microsoft.Extensions.Configuration;

namespace DiscordLikeChatApp.Views {
    public partial class DashboardView : UserControl, INotifyPropertyChanged {

        private readonly UserSession _userSession;
        private string _username;
        private readonly ApiService _apiService;
        private string _currentServerId; // Pour stocker le serveur courant

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Username {
            get => _username;
            set {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public DashboardView(UserSession userSession, IConfiguration configuration) {
            InitializeComponent();
            _userSession = userSession;
            string authToken = _userSession.Get<string>("Authorization");
            _apiService = new ApiService(configuration, authToken);
            InitializeUserDetails();
        }

        private void InitializeUserDetails() {
            Username = _userSession.Get<string>("Username");
            UserNameTextBlock.Text = Username;
            UserStatusTextBlock.Text = "En ligne";
        }

        private void OnServerButtonClick(object sender, RoutedEventArgs e) {
            if (sender is Button button) {
                _currentServerId = button.Content.ToString();
                LoadServer(_currentServerId);
            }
        }

        private async void LoadServer(string serverId) {
            // Effacer la liste des amis
            FriendsListBox.Visibility = Visibility.Collapsed;
            FriendsListBox.Items.Clear();
            // Charger les canaux du serveur
            await LoadChannels(serverId);
        }

        private void OnChannelButtonClick(object sender, RoutedEventArgs e) {
            if (sender is Button button) {
                string channelId = button.Content.ToString();
                // Charger la vue de chat pour le canal sélectionné
                LoadChannel(channelId);
            }
        }

        private async Task LoadChannels(string serverId) {
            // Masquer la liste des amis
            FriendsListBox.Visibility = Visibility.Collapsed;
            FriendsListBox.Items.Clear();

            // Récupérer la liste des canaux via l'API GET
            var channels = await _apiService.GetAsync<List<Channel>>("/channels");

            // Réinitialiser la zone des canaux
            ChannelStackPanel.Children.Clear();

            // Afficher un titre (par exemple, "Serveur 1")
            ChannelStackPanel.Children.Add(new TextBlock {
                Text = $"Serveur {serverId}",
                Foreground = Brushes.White,
                FontSize = 20,
                Margin = new Thickness(10)
            });

            // Pour chaque canal, ajouter un bouton
            foreach (var channel in channels) {
                AddChannelButton(channel.Name);
            }

            // Ajouter le bouton pour créer un nouveau canal
            var createChannelButton = new Button {
                Content = "+ Ajouter un nouveau canal",
                Margin = new Thickness(10),
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            createChannelButton.Click += OnCreateChannelButtonClick;
            ChannelStackPanel.Children.Add(createChannelButton);
        }

        private void AddChannelButton(string channelName) {
            var channelPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10) };

            var manageButton = new Button {
                Content = "⚙",
                Width = 20,
                Height = 20,
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            manageButton.Click += (s, e) => OnClickManageChannel(channelName);
            channelPanel.Children.Add(manageButton);

            var channelButton = new Button {
                Content = channelName,
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            channelButton.Click += OnChannelButtonClick;
            channelPanel.Children.Add(channelButton);

            ChannelStackPanel.Children.Add(channelPanel);
        }

        private void LoadChannel(string channelId) {
            // Afficher la vue de chat pour le canal sélectionné
            FriendsListBox.Visibility = Visibility.Collapsed;
            FriendsListBox.Items.Clear();

            MainGrid.Children.Clear();
            MainGrid.Children.Add(new ChatView(_apiService, channelId));
        }

        private async void OnCreateChannelButtonClick(object sender, RoutedEventArgs e) {
            // Afficher une boîte de dialogue pour entrer le nom du nouveau canal
            var inputDialog = new InputDialog("Entrez le nom du nouveau canal :");
            if (inputDialog.ShowDialog() == true) {
                string newChannelName = inputDialog.ResponseText;
                string description = "General discussion";
                string type = "TEXT"; 

                // Appeler l'API pour créer le canal
                var newChannel = await _apiService.CreateChannelAsync(newChannelName, description, type);

                if (newChannel != null) {
                    AssignAdminRoleToUser();
                    await LoadChannels(_currentServerId);
                }
                else {
                    MessageBox.Show("Erreur lors de la création du canal.");
                }
            }
        }

        private void OnClickManageChannel(string channelId) {
            MainGrid.Children.Clear();
            var channelManager = new ChannelManager();
            MainGrid.Children.Add(channelManager);
        }

        private void OnUserSettingsButtonClick(object sender, RoutedEventArgs e) {
            MainGrid.Children.Clear();
            var userSettings = new UserSettings();
            MainGrid.Children.Add(userSettings);
        }

        private void AssignAdminRoleToUser() {
            // Logique pour attribuer le rôle d'administrateur à l'utilisateur
           //TODO
        }

        private void OnSearchUsersButtonClick(object sender, RoutedEventArgs e) {
            // Effacer les informations du serveur et des canaux
            ChannelStackPanel.Children.Clear();

            // Logique pour rechercher des utilisateurs
            List<string> friends = new List<string> { "Ami1 (En ligne)", "Ami2 (Hors ligne)", "Ami3 (En ligne)" };
            FriendsListBox.Items.Clear();
            foreach (var friend in friends) {
                FriendsListBox.Items.Add(new ListBoxItem { Content = friend });
            }
            FriendsListBox.Visibility = Visibility.Visible;
        }
    }
}
