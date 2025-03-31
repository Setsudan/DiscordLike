using System.Windows;
using System.Windows.Controls;
using DiscordFrontEnd.Services;


namespace DiscordLikeChatApp.Views {
    /// <summary>
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl {
        private readonly ApiService _apiService;

        public LoginView() {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            bool success = await _apiService.LoginAsync(username, password);

            if (success) {
                // Rediriger vers le tableau de bord
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.Content = new DashboardView();
            }
            else {
                MessageBox.Show("Connexion échouée. Veuillez réessayer.");
            }
        }
    }
}
