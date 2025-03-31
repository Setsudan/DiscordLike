using System.Windows;
using System.Windows.Controls;


namespace DiscordLikeChatApp.Views
{
    /// <summary>
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // ajouter le HTTP request ici

        }
    }

}
}
