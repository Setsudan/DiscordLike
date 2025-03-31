using System.Windows;
using System.Windows.Controls;

namespace DiscordLikeChatApp.Views {
    public partial class ChatView : UserControl {
        public ChatView() {
            InitializeComponent();
        }

        private void OnSendButtonClick(object sender, RoutedEventArgs e) {
            string message = MessageTextBox.Text;
            if (!string.IsNullOrEmpty(message)) {
                MessagesListBox.Items.Add(message);
                MessageTextBox.Clear();
            }
        }
    }
}
