using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using DiscordFrontEnd.Models;
using DiscordFrontEnd.Services;


namespace DiscordFrontEnd.ViewModels {
    public class LoginViewModel : INotifyPropertyChanged {
        private string _username;
        private string _password;
        private readonly ApiService _apiService;

        public string Username {
            get => _username;
            set {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password {
            get => _password;
            set {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand {
            get;
        }

        public LoginViewModel() {
            _apiService = new ApiService();
            LoginCommand = new RelayCommand(async () => await LoginAsync());
        }

        private async Task LoginAsync() {
            var success = await _apiService.LoginAsync(Username, Password);

            if (success) {
                // Connexion réussie
            }
            else {
                // Connexion échouée
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
