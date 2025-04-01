using DiscordHeticWpf.Models;

namespace DiscordHeticWpf.Services
{
    public class AuthService
    {
        private readonly ApiService _apiService;

        public AuthService(string baseUrl, string token = null)
        {
            // The ApiService is assumed to be defined as shown earlier.
            _apiService = new ApiService(baseUrl, token);
        }

        /// <summary>
        /// Registers a new user by calling /auth/signup.
        /// </summary>
        public async Task<AuthResponse> SignUpAsync(SignUpRequest request)
        {
            // POST request to /auth/signup endpoint.
            // Adjust endpoint path if needed.
            var response = await _apiService.PostAsync<AuthResponse>("/auth/signup", request);
            return response;
        }

        /// <summary>
        /// Authenticates a user by calling /auth/signin.
        /// </summary>
        public async Task<AuthResponse> SignInAsync(SignInRequest request)
        {
            // POST request to /auth/signin endpoint.
            var response = await _apiService.PostAsync<AuthResponse>("/auth/signin", request);
            return response;
        }

        /// <summary>
        /// Sets the authentication token for the ApiService.
        /// </summary>
        public void SetToken(string token)
        {
            _apiService.SetToken(token);
        }

        /// <summary>
        /// Logs out the user by calling /auth/logout.
        /// </summary>
        public async Task LogoutAsync()
        {
            // POST request to /auth/logout endpoint.
            await _apiService.PostAsync<object>("/auth/logout", null);
        }
    }
}
