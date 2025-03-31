using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiscordLikeChatApp.Views {
    /// <summary>
    /// Logique d'interaction pour DashboardView.xaml
    /// </summary>
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
            // Par exemple, mettre à jour la barre latérale des canaux et la zone principale

            // Exemple de mise à jour de la barre latérale des canaux
            ChannelStackPanel.Children.Clear();
            ChannelStackPanel.Children.Add(new TextBlock { Text = $"Serveur {serverId}", Foreground = Brushes.White, FontSize = 20, Margin = new Thickness(10) });
            ChannelStackPanel.Children.Add(new Button { Content = "# général", Margin = new Thickness(10), Background = Brushes.Transparent, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Left });
            ChannelStackPanel.Children.Add(new Button { Content = "# random", Margin = new Thickness(10), Background = Brushes.Transparent, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Left });

            // Exemple de mise à jour de la zone principale
            MainGrid.Children.Clear();
            MainGrid.Children.Add(new TextBlock { Text = $"Bienvenue sur le serveur {serverId}", FontSize = 16, Foreground = Brushes.White, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center });
        }
    }
}
