using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using DiscordFrontEnd.Models;
using DiscordLikeChatApp;
using DiscordLikeChatApp.Services;
using Microsoft.Extensions.Configuration;

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

        public LoginViewModel(IConfiguration configuration,string authToken) {
            _apiService = new ApiService(configuration, authToken);
            LoginCommand = new RelayCommand(async _ => await LoginAsync());
        }

        private async Task LoginAsync() {
            var success = await _apiService.PostAsync<LoginRequest, bool>("login", new LoginRequest { Username = Username, Password = Password });

            if (success) {
                // Connexion réussie
              
            }
            else {
                // Connexion échouée
            }
        }
        public class LoginRequest {
            public string Username {
                get; set;
            }
            public string Password {
                get; set;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
