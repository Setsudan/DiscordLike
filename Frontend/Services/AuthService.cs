using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DiscordLikeChatApp.Models;

namespace DiscordLikeChatApp.Services {
    public class AuthService {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private string _accessToken;

        public AuthService(IConfiguration configuration, HttpClient httpClient) {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<AccessTokenResponse> LoginAsync(string username, string password) {
            var loginEndpoint = _configuration["Auth:LoginEndpoint"];
            var loginData = new {
                Username = username,
                Password = password
            };

            try {
                var response = await _httpClient.PostAsJsonAsync(loginEndpoint, loginData);
                if (response.IsSuccessStatusCode) {
                    // Désérialiser la réponse complète
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<LoginData>>(
                        await response.Content.ReadAsStreamAsync(), options);

                    // Vérifier que l'objet désérialisé n'est pas null
                    if (apiResponse?.Data != null) {
                        _accessToken = apiResponse.Data.Token;
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                        return new AccessTokenResponse {
                            IsSuccess = true,
                            AccessToken = _accessToken
                        };
                    }
                    else {
                        return new AccessTokenResponse {
                            IsSuccess = false,
                            ErrorMessage = "Données de connexion invalides."
                        };
                    }
                }
                else {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new AccessTokenResponse {
                        IsSuccess = false,
                        ErrorMessage = errorContent
                    };
                }
            }
            catch (Exception ex) {
                return new AccessTokenResponse {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public void Logout() {
            _accessToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public bool IsAuthenticated() {
            return !string.IsNullOrEmpty(_accessToken);
        }

        public async Task<bool> RegisterAsync(string username, string email, string password) {
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
    }
}
