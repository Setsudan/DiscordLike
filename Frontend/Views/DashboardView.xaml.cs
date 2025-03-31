﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DiscordLikeChatApp.Views.Components;

namespace DiscordLikeChatApp.Views {
    public partial class DashboardView : UserControl {
        public DashboardView() {
            InitializeComponent();
            InitializeUserDetails();
        }

        private void InitializeUserDetails() {
            UserNameTextBlock.Text = "Utilisateur123"; // Remplacez par le nom d'utilisateur réel
            UserStatusTextBlock.Text = "En ligne"; // Remplacez par le statut réel de l'utilisateur
        }

        private void OnServerButtonClick(object sender, RoutedEventArgs e) {
            if (sender is Button button) {
                string serverId = button.Content.ToString();
                LoadServer(serverId);
            }
        }

        private void LoadServer(string serverId) {
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
            // Logique pour rechercher des utilisateurs
            MessageBox.Show("Fonction de recherche d'utilisateurs non implémentée.");
        }
    }
}
