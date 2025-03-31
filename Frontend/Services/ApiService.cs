using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DiscordFrontEnd.Models;

namespace DiscordFrontEnd.Services {

    public class ApiService {
        private readonly HttpClient _httpClient;

        public ApiService() {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:8080"); //changez par le bon url
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> LoginAsync(string username, string password) {
            var user = new {
                username,
                password
            };
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/auth/login", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterAsync(User user) {
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/auth/register", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<User>> GetUsersAsync() {
            var response = await _httpClient.GetAsync("/users");

            if (response.IsSuccessStatusCode) {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<User>>(json);
            }

            return null;
        }

    }
}
