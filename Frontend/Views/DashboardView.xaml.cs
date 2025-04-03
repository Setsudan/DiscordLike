using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
        private readonly IConfiguration _configuration;


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


        private void ClearMainGrid() {
            MainGrid.Children.Clear();
        }
        private void ClearChannelStackPanel() {
            ChannelStackPanel.Children.Clear();
        }
        // Lorsqu'un bouton de serveur est cliqué
        private void OnServerButtonClick(object sender, RoutedEventArgs e) {
            ClearChannelStackPanel();
            ClearMainGrid();
            if (sender is Button button) {
                _currentServerId = button.Content.ToString();
                LoadServer(_currentServerId);
            }
        }

        // Charge la vue du serveur (les canaux)
        private async void LoadServer(string serverId) {
            ClearChannelStackPanel();
            await LoadChannels(serverId);
        }

        // Lorsqu'un bouton de canal est cliqué, on affiche la vue de chat
        private void OnChannelButtonClick(object sender, RoutedEventArgs e) {
            if (sender is Button button) {
                string channelId = button.Content.ToString();
                LoadChannel(channelId);
            }
        }

        // Charge la liste des canaux du serveur via GET et les affiche
        private async System.Threading.Tasks.Task LoadChannels(string serverId) {
          ClearChannelStackPanel();
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
        private void LoadChannel(string channelName) {
            MainGrid.Children.Add(new ChatView(_apiService, _userSession, channelName));
        }


        // Gestion de la création d'un nouveau canal
        private async void OnCreateChannelButtonClick(object sender, RoutedEventArgs e) {
           ClearChannelStackPanel();
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
            ClearChannelStackPanel();
            var channelManager = new ChannelManager(channelId, _apiService, _userSession);
            MainGrid.Children.Add(channelManager);
        }

        // Affiche les paramètres utilisateur
        private void OnUserSettingsButtonClick(object sender, RoutedEventArgs e) {
            var userSettings = new UserSettings(_userSession, _configuration);
            MainGrid.Children.Add(userSettings);
        }


        // Affiche la liste de tous les utilisateurs et permet d'ajouter des amis
        private async void OnShowUsersButtonClick(object sender, RoutedEventArgs e) {
            ClearMainGrid();

            try {
                string currentUsername = _userSession.Get<string>("Username");
                var users = await _apiService.GetAsync<List<User>>("/users");
                var friends = await _apiService.GetAsync<List<User>>("/friends");
                var friendRequests = await _apiService.GetAsync<List<User>>("/friends/requests");

                ListBox usersListBox = new ListBox {
                    Margin = new Thickness(10),
                    Background = (Brush)new BrushConverter().ConvertFromString("#2C2F33") // Fond bleu foncé
                };

                foreach (var user in users) {
                    if (user.Username == currentUsername || friends.Any(f => f.Id == user.Id))
                        continue;

                    var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                    var userText = new TextBlock {
                        Text = user.Username,
                        Width = 150,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = Brushes.White
                    };

                    if (friendRequests.Any(fr => fr.Id == user.Id)) {
                        var requestSentText = new TextBlock {
                            Text = "Demande d'ami envoyée",
                            Foreground = Brushes.Gray,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        panel.Children.Add(userText);
                        panel.Children.Add(requestSentText);
                    }
                    else {
                        var addFriendButton = new Button {
                            Content = "Ajouter en ami",
                            Height = 35,
                            Width = 200, // Augmentez la largeur du bouton
                            FontSize = 14,
                            FontWeight = FontWeights.Bold,
                            Foreground = Brushes.White,
                            Background = (Brush)new BrushConverter().ConvertFromString("#5865F2"),
                            BorderBrush = Brushes.Transparent,
                            Cursor = System.Windows.Input.Cursors.Hand,
                            Padding = new Thickness(10, 5, 10, 5),
                            Margin = new Thickness(5)
                        };

                        addFriendButton.Template = CreateCustomButtonTemplate();
                        addFriendButton.Click += (s, args) => AddFriend(user);

                        panel.Children.Add(userText);
                        panel.Children.Add(addFriendButton);
                    }

                    usersListBox.Items.Add(new ListBoxItem { Content = panel });
                }

                MainGrid.Children.Clear();
                MainGrid.Children.Add(usersListBox);
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur lors du chargement des utilisateurs: " + ex.Message);
            }
        }




        private ControlTemplate CreateCustomButtonTemplate() {
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(5)); // Coins légèrement arrondis
            borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            borderFactory.SetValue(Border.BorderBrushProperty, Brushes.Transparent);

            var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentPresenterFactory);

            var template = new ControlTemplate(typeof(Button)) {
                VisualTree = borderFactory
            };

            template.Triggers.Add(new Trigger {
                Property = Button.IsMouseOverProperty,
                Value = true,
                Setters = { new Setter(Button.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4752C4"))) }
            });

            template.Triggers.Add(new Trigger {
                Property = Button.IsPressedProperty,
                Value = true,
                Setters = { new Setter(Button.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A42A5"))) }
            });

            return template;
        }

        // Méthode pour ajouter un ami
        private async void AddFriend(User user) {
            try {
                var result = await _apiService.PostAsync<User, ApiResponse<User>>($"friends/request?recipientId={user.Id}", user);
                if (result.Success) {
                    MessageBox.Show($"{user.Username} a été ajouté en tant qu'ami.");
                }
                else {
                    MessageBox.Show($"{result.Message}");
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur: " + ex.Message);
            }
        }

        private async void OnSearchUsersButtonClick(object sender, RoutedEventArgs e) {
            ClearChannelStackPanel();

            try {
                var friends = await _apiService.GetAsync<List<User>>("/friends");
                var friendRequests = await _apiService.GetAsync<List<User>>("/friends/requests");

                ListBox friendsListBox = new ListBox {
                    Margin = new Thickness(10),
                    Background = (Brush)new BrushConverter().ConvertFromString("#2C2F33"),
                    Foreground = Brushes.White
                };

                ListBox friendRequestsListBox = new ListBox {
                    Margin = new Thickness(10),
                    Background = (Brush)new BrushConverter().ConvertFromString("#2C2F33"),
                    Foreground = Brushes.White
                };

                foreach (var request in friendRequests) {
                    var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                    var userText = new TextBlock {
                        Text = request.Username,
                        Width = 150,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = Brushes.White
                    };

                    var acceptButton = new Button {
                        Content = "Accepter",
                        Height = 35,
                        Width = 100,
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        Background = (Brush)new BrushConverter().ConvertFromString("#5865F2"),
                        BorderBrush = Brushes.Transparent,
                        Cursor = System.Windows.Input.Cursors.Hand,
                        Padding = new Thickness(10, 5, 10, 5),
                        Margin = new Thickness(5)
                    };
                    acceptButton.Click += async (s, args) => await AcceptFriendRequest(request.Id);

                    var declineButton = new Button {
                        Content = "Refuser",
                        Height = 35,
                        Width = 100,
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        Background = (Brush)new BrushConverter().ConvertFromString("#FF0000"),
                        BorderBrush = Brushes.Transparent,
                        Cursor = System.Windows.Input.Cursors.Hand,
                        Padding = new Thickness(10, 5, 10, 5),
                        Margin = new Thickness(5)
                    };
                    declineButton.Click += async (s, args) => await DeclineFriendRequest(request.Id);

                    panel.Children.Add(userText);
                    panel.Children.Add(acceptButton);
                    panel.Children.Add(declineButton);

                    friendRequestsListBox.Items.Add(new ListBoxItem { Content = panel });
                }

                foreach (var friend in friends) {
                    var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                    var friendText = new TextBlock {
                        Text = friend.Username,
                        Width = 150,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = Brushes.White
                    };

                    panel.Children.Add(friendText);

                    friendsListBox.Items.Add(new ListBoxItem { Content = panel });
                }

                // Afficher la ListBox dans le ChannelStackPanel
                ChannelStackPanel.Children.Clear();
                ChannelStackPanel.Children.Add(friendRequestsListBox);
                ChannelStackPanel.Children.Add(friendsListBox);
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur lors du chargement des amis: " + ex.Message);
            }
        }

        private async Task AcceptFriendRequest(Guid friendshipId) {
            try {
                var result = await _apiService.PostAsync<object, ApiResponse<object>>($"/friends/{friendshipId}/accept", null);
                if (result.Success) {
                    MessageBox.Show("Demande d'ami acceptée.");
                }
                else {
                    MessageBox.Show( result.Message);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur: " + ex.Message);
            }
        }

        private async Task DeclineFriendRequest(Guid friendshipId) {
            try {
                var result = await _apiService.PostAsync<object, ApiResponse<object>>($"/friends/{friendshipId}/decline", null);
                if (result.Success) {
                    MessageBox.Show("Demande d'ami refusée.");
                }
                else {
                    MessageBox.Show( result.Message);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Erreur: " + ex.Message);
            }
        }


        private void AssignAdminRoleToUser() {
            // Logique pour attribuer le rôle d'administrateur à l'utilisateur (à compléter)
        }
    }
}
