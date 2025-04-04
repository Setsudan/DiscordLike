using System.Windows;
using System.Windows.Controls;

namespace DiscordLikeChatApp.Views.Components
{
    /// <summary>
    /// Logique d'interaction pour InputDialog.xaml
    /// </summary>
        public partial class InputDialog : Window {
            public string ResponseText {
                get; private set;
            }

            public InputDialog(string question) {
                InitializeComponent();
                QuestionTextBlock.Text = question;
            }

            private void OkButton_Click(object sender, RoutedEventArgs e) {
                ResponseText = InputTextBox.Text;
                DialogResult = true;
                Close();
            }

            private void CancelButton_Click(object sender, RoutedEventArgs e) {
                DialogResult = false;
                Close();
            }
        }
    }


