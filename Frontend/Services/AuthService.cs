using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Acr.UserDialogs;
using Microsoft.Extensions.Configuration;

namespace DiscordLikeChatApp.Services
{
    public class AuthService {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private string _accessToken;

        public AuthService(IConfiguration configuration, HttpClient httpClient) {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string username, string password) {
            var loginEndpoint = _configuration["Auth:LoginEndpoint"];
            var loginData = new {
                Username = username,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync(loginEndpoint, loginData);
            if (response.IsSuccessStatusCode) {
                var result = await JsonSerializer.DeserializeAsync<LoginResult>(await response.Content.ReadAsStreamAsync());
                _accessToken = result.AccessToken;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                return true;
            }

            return false;
        }

        public void Logout() {
            _accessToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public bool IsAuthenticated() {
            return !string.IsNullOrEmpty(_accessToken);
        }
        public async Task<bool> RegisterAsync(string username,string email, string password) {
            var registerEndpoint = _configuration["Auth:RegisterEndpoint"];
            var registerData = new {
                Username = username,
                Password = password,
                Email = email,
                Role = "ADMIN"
            };

            var response = await _httpClient.PostAsJsonAsync(registerEndpoint, registerData);
            return response.IsSuccessStatusCode;
        }

        private class LoginResult {
            public string AccessToken {
                get; set;
            }
        }

    }
}
