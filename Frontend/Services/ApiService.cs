using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DiscordFrontEnd.Models;
using DiscordLikeChatApp.Models;
using Microsoft.Extensions.Configuration;

namespace DiscordLikeChatApp.Services {
    public class ApiService {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ApiService(IConfiguration configuration, string authToken) {
            _configuration = configuration;
            _httpClient = new HttpClient {
                BaseAddress = new Uri("https://discord-backend.ethlny.net/")
            };

            // Configure les en-têtes par défaut 
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(authToken)) {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }

        }

        // Méthode GET
        public async Task<T> GetAsync<T>(string endpoint) {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var options = new System.Text.Json.JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            };

            // Désérialisation dans ApiResponse<T>
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(options);

            if (apiResponse == null) {
                throw new Exception("La réponse de l'API est vide.");
            }

            return apiResponse.Data;
        }

        // Méthode POST 
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data) {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        // Méthode PUT 
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data) {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        // Méthode DELETE 
        public async Task<bool> DeleteAsync(string endpoint) {
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }

        // Méthode PATCH générique (HttpClient ne possède pas de méthode Patch par défaut) donc a tester
        public async Task<TResponse> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data) {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint) {
                Content = JsonContent.Create(data)
            };
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        // Exemple de méthode spécifique pour récupérer la liste des serveurs
        public async Task<List<Server>> GetServersAsync() {
            return await GetAsync<List<Server>>("api/servers");
        }

        // créer un nouveau canal dans un serveur a corriger avec le bon endpointRe
        public async Task<Channel> CreateChannelAsync(string channelName, string description, string type) {
            var payload = new {
                name = channelName,
                description = description,
                type = type
            };
            return await PostAsync<object, Channel>("/channels", payload);
        }

        // Exemple de méthode pour supprimer un canal
        public async Task<bool> DeleteChannelAsync( string channelId) {
            return await DeleteAsync($"/channels/{channelId}");
        }

        // Exemple de méthode pour mettre à jour un canal via PUT
        public async Task<Channel> UpdateChannelAsync(string serverId, string channelId, string newChannelName) {
            var payload = new {
                name = newChannelName
            };
            return await PutAsync<object, Channel>($"api/servers/{serverId}/channels/{channelId}", payload);
        }
    }
}
