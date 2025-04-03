using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DiscordFrontEnd.Models;
using DiscordLikeChatApp.Models;
using DiscordLikeChatApp.Services;
using DiscordLikeChatApp.Views.Components;
using Microsoft.Extensions.Configuration;

namespace DiscordLikeChatApp.Views {
    public partial class DashboardView : UserControl, INotifyPropertyChanged {

        private readonly UserSession _userSession;
        private string _username;
        private readonly ApiService _apiService;
        private string _currentServerId;

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

        // Constructeur avec paramètres (utilisé dans le code-behind)
        public DashboardView(UserSession userSession, IConfiguration configuration) {
            InitializeComponent();
            _userSession = userSession;
            string authToken = _userSession.Get<string>("Authorization");
            _apiService = new ApiService(configuration, authToken);
            InitializeUserDetails();
            // Atterrir directement sur un serveur par défaut (exemple : "1")
            _currentServerId = "1";
            LoadServer(_currentServerId);
        }

        // Constructeur par défaut requis par le XAML
        public DashboardView() {
            InitializeComponent();
        }

        private void InitializeUserDetails() {
            Username = _userSession.Get<string>("Username");
            UserNameTextBlock.Text = Username;
            UserStatusTextBlock.Text = "En ligne";
        }

        /// <summary>
        /// Vide tous les conteneurs d'affichage.
        /// </summary>
        private void ClearDisplay() {
            ChannelStackPanel.Children.Clear();
            FriendsListBox.Items.Clear();
            FriendsListBox.Visibility = Visibility.Collapsed;
            MainGrid.Children.Clear();
        }

        // Lorsqu'un bouton de serveur est cliqué
        private void OnServerButtonClick(object sender, RoutedEventArgs e) {
            ClearDisplay();
            if (sender is Button button) {
                _currentServerId = button.Content.ToString();
                LoadServer(_currentServerId);
            }
        }

        // Charge la vue du serveur (les canaux)
        private async void LoadServer(string serverId) {
            ClearDisplay();
            await LoadChannels(serverId);
        }

        // Lorsqu'un bouton de canal est cliqué, on affiche la vue de chat
        private void OnChannelButtonClick(object sender, RoutedEventArgs e) {
            ClearDisplay();
            if (sender is Button button) {
                string channelId = button.Content.ToString();
                LoadChannel(channelId);
            }
        }

        // Charge la liste des canaux du serveur via GET et les affiche
        private async System.Threading.Tasks.Task LoadChannels(string serverId) {
            ClearDisplay();
            // Récupérer la liste des canaux via l'API GET
            var channels = await _apiService.GetAsync<List<Channel>>("/channels");

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

        // Ajoute un bouton pour un canal
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

        // Affiche la vue de chat pour le canal sélectionné
        private void LoadChannel(string channelId) {
            ClearDisplay();
            MainGrid.Children.Add(new ChatView(_apiService, channelId));
        }

        // Gestion de la création d'un nouveau canal
        private async void OnCreateChannelButtonClick(object sender, RoutedEventArgs e) {
            ClearDisplay();
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

        // Affiche la vue de gestion du canal
        private void OnClickManageChannel(string channelId) {
            ClearDisplay();
            var channelManager = new ChannelManager();
            MainGrid.Children.Add(channelManager);
        }

        // Affiche les paramètres utilisateur
        private void OnUserSettingsButtonClick(object sender, RoutedEventArgs e) {
            ClearDisplay();
            var userSettings = new UserSettings();
            MainGrid.Children.Add(userSettings);
        }


        // Affiche la liste de tous les utilisateurs et permet d'ajouter des amis
        private async void OnShowUsersButtonClick(object sender, RoutedEventArgs e) {
            // Vider tous les conteneurs pour être sûr de n'afficher qu'une vue
            ClearDisplay();
            try {
                // Récupérer la liste de tous les utilisateurs via l'API GET sur "/users"
                var users = await _apiService.GetAsync<List<User>>("/users");

                // Créer une ListBox pour afficher tous les utilisateurs dans le MainGrid
                ListBox usersListBox = new ListBox {
                    Margin = new Thickness(10)
                };

                foreach (var user in users) {
                    var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                    var userText = new TextBlock {
                        Text = user.Username,
                        Width = 150,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    // Par exemple, on peut ajouter un bouton pour ajouter en ami
                    var addFriendButton = new Button {
                        Content = "Ajouter en ami",
                        Margin = new Thickness(5)
                    };
                    //addFriendButton.Click += (s, args) => AddFriend(user);

                    panel.Children.Add(userText);
                    panel.Children.Add(addFriendButton);
                    usersListBox.Items.Add(new ListBoxItem { Content = panel });
                }

                // Afficher la ListBox dans le MainGrid
                MainGrid.Children.Clear();
                MainGrid.Children.Add(usersListBox);
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur lors du chargement des utilisateurs: " + ex.Message);
            }
        }


        // Si OnSearchUsersButtonClick existe déjà, on le redirige
        private async void OnSearchUsersButtonClick(object sender, RoutedEventArgs e) {
            // Vider tous les conteneurs pour n'afficher qu'une vue
            ClearDisplay();
            try {
                // Récupérer la liste des amis via l'API GET sur "/friends"
                var friends = await _apiService.GetAsync<List<User>>("/friends");

                // Créer une ListBox pour afficher les amis dans le ChannelStackPanel
                ListBox friendsListBox = new ListBox {
                    Margin = new Thickness(10)
                };

                foreach (var friend in friends) {
                    var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                    var friendText = new TextBlock {
                        Text = friend.Username,
                        Width = 150,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    // Vous pouvez ajouter un bouton de suppression ou autre action pour un ami
                    var removeFriendButton = new Button {
                        Content = "Supprimer ami",
                        Margin = new Thickness(5)
                    };
                    //removeFriendButton.Click += (s, args) => RemoveFriend(friend);

                    panel.Children.Add(friendText);
                    panel.Children.Add(removeFriendButton);
                    friendsListBox.Items.Add(new ListBoxItem { Content = panel });
                }

                // Afficher la ListBox dans le ChannelStackPanel
                ChannelStackPanel.Children.Clear();
                ChannelStackPanel.Children.Add(friendsListBox);
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur lors du chargement des amis: " + ex.Message);
            }
        }

        private void AssignAdminRoleToUser() {
            // Logique pour attribuer le rôle d'administrateur à l'utilisateur (à compléter)
        }
    }
}
