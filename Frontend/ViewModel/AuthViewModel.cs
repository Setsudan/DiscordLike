using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DiscordHeticWpf.Models;
using DiscordHeticWpf.Services;

namespace DiscordHeticWpf.ViewModels
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;

        private string _username;
        private string _password;
        private string _token;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public string Token
        {
            get => _token;
            set { _token = value; OnPropertyChanged(nameof(Token)); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public ICommand SignInCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand LogoutCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public AuthViewModel()
        {
            // Change the URL to your backend's base URL.
            _authService = new AuthService("http://localhost:8080");
            SignInCommand = new RelayCommand(async () => await SignInAsync(), () => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
            SignUpCommand = new RelayCommand(async () => await SignUpAsync(), () => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
            LogoutCommand = new RelayCommand(Logout, () => !string.IsNullOrEmpty(Token));
        }

        private async Task SignInAsync()
        {
            try
            {
                var signInRequest = new SignInRequest { username = this.Username, password = this.Password };
                var response = await _authService.SignInAsync(signInRequest);
                if (response != null && response.status == 200)
                {
                    Token = response.data.token;
                    _authService.SetToken(Token);
                    ErrorMessage = string.Empty;
                    // Signal successful login (e.g. navigate to another View)
                }
                else
                {
                    ErrorMessage = response?.message ?? "Sign in failed.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error: " + ex.Message;
            }
        }

        private async Task SignUpAsync()
        {
            try
            {
                // For simplicity, using a default email derived from username.
                var signUpRequest = new SignUpRequest
                {
                    username = this.Username,
                    email = $"{Username}@example.com",
                    password = this.Password,
                    roles = new string[] { }
                };
                var response = await _authService.SignUpAsync(signUpRequest);
                if (response != null && response.status == 201)
                {
                    Token = response.data.token;
                    _authService.SetToken(Token);
                    ErrorMessage = string.Empty;
                }
                else
                {
                    ErrorMessage = response?.message ?? "Sign up failed.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error: " + ex.Message;
            }
        }

        private async void Logout()
        {
            Token = string.Empty;
            await _authService.LogoutAsync();
        }
    }
}
