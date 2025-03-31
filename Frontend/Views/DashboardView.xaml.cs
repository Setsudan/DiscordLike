using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiscordLikeChatApp.Views {
    public partial class DashboardView : UserControl {
        public DashboardView() {
            InitializeComponent();
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
            var generalButton = new Button { Content = "# général", Margin = new Thickness(10), Background = Brushes.Transparent, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Left };
            generalButton.Click += OnChannelButtonClick;
            ChannelStackPanel.Children.Add(generalButton);
            var randomButton = new Button { Content = "# random", Margin = new Thickness(10), Background = Brushes.Transparent, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Left };
            randomButton.Click += OnChannelButtonClick;
            ChannelStackPanel.Children.Add(randomButton);

            // Ajouter un bouton pour créer un nouveau canal
            var createChannelButton = new Button { Content = "Créer un nouveau canal", Margin = new Thickness(10), Background = Brushes.Transparent, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Left };
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
            // Logique pour créer un nouveau canal
            string newChannelId = "# nouveau-canal";
            var newChannelButton = new Button { Content = newChannelId, Margin = new Thickness(10), Background = Brushes.Transparent, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Left };
            newChannelButton.Click += OnChannelButtonClick;
            ChannelStackPanel.Children.Add(newChannelButton);

            // Attribuer automatiquement le rôle d'administrateur à l'utilisateur
            AssignAdminRoleToUser();
        }

        private void AssignAdminRoleToUser() {
            // Logique pour attribuer le rôle d'administrateur à l'utilisateur
            MessageBox.Show("Le rôle d'administrateur a été attribué à l'utilisateur pour le nouveau canal.");
        }
    }
}
