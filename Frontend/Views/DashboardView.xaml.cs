using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DiscordLikeChatApp.Services;
using DiscordLikeChatApp.Views.Components;

namespace DiscordLikeChatApp.Views {
    public partial class DashboardView : UserControl, INotifyPropertyChanged {

        private readonly UserSession _userSession;
        private string _username;

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

        public DashboardView(UserSession userSession) {
            InitializeComponent();
            _userSession = userSession;
            InitializeUserDetails();
        }

        private void InitializeUserDetails() {
            Username = _userSession.Get<string>("Username");
            UserNameTextBlock.Text = Username;
            UserStatusTextBlock.Text = "En ligne";
        }

        private void OnServerButtonClick(object sender, RoutedEventArgs e) {
            if (sender is Button button) {
                string serverId = button.Content.ToString();
                LoadServer(serverId);
            }
        }

        private void LoadServer(string serverId) {
            // Effacer la liste des amis
            FriendsListBox.Visibility = Visibility.Collapsed;
            FriendsListBox.Items.Clear();

            // Logique pour charger les informations du serveur en fonction de l'ID du serveur
            ChannelStackPanel.Children.Clear();
            ChannelStackPanel.Children.Add(new TextBlock { Text = $"Serveur {serverId}", Foreground = Brushes.White, FontSize = 20, Margin = new Thickness(10) });
            AddChannelButton("# général");
            AddChannelButton("# random");

            // Ajouter un bouton pour créer un nouveau canal
            var createChannelButton = new Button { Content = "+ Ajouter un nouveau canal", Margin = new Thickness(10), Background = Brushes.Transparent, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Left };
            createChannelButton.Click += OnCreateChannelButtonClick;
            ChannelStackPanel.Children.Add(createChannelButton);
        }

        private void OnChannelButtonClick(object sender, RoutedEventArgs e) {
            if (sender is Button button) {
                string channelId = button.Content.ToString();
                LoadChannel(channelId);
            }
        }

        private void LoadChannel(string channelId) {
            // Effacer la liste des amis
            FriendsListBox.Visibility = Visibility.Collapsed;
            FriendsListBox.Items.Clear();

            MainGrid.Children.Clear();
            MainGrid.Children.Add(new ChatView());
        }

        private void OnCreateChannelButtonClick(object sender, RoutedEventArgs e) {
            // Afficher une boîte de dialogue pour entrer le nom du nouveau canal
            var inputDialog = new InputDialog("Entrez le nom du nouveau canal :");
            if (inputDialog.ShowDialog() == true) {
                string newChannelId = inputDialog.ResponseText;
                AddChannelButton(newChannelId);

                // Attribuer automatiquement le rôle d'administrateur à l'utilisateur
                AssignAdminRoleToUser();
            }
        }

        private void AddChannelButton(string channelId) {
            var channelPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10) };

            var manageButton = new Button { Content = "⚙", Width = 20, Height = 20, Background = Brushes.Transparent, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Left };
            manageButton.Click += (s, e) => OnClickManageChannel(channelId);
            channelPanel.Children.Add(manageButton);

            var channelButton = new Button { Content = channelId, Background = Brushes.Transparent, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Left };
            channelButton.Click += OnChannelButtonClick;
            channelPanel.Children.Add(channelButton);

            ChannelStackPanel.Children.Add(channelPanel);
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
            MessageBox.Show("Le rôle d'administrateur a été attribué à l'utilisateur pour le nouveau canal.");
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
