using System.Windows;
using System.Windows.Controls;
using DiscordHeticWpf.ViewModels;

namespace DiscordHeticWpf.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        // Assuming DataContext is an instance of AuthViewModel
        private AuthViewModel ViewModel => (AuthViewModel)DataContext;

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && sender is PasswordBox passwordBox)
            {
                // Update the ViewModel's Password property whenever the PasswordBox changes.
                ViewModel.Password = passwordBox.Password;
            }
        }
    }
}
