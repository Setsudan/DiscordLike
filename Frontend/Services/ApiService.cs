using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DiscordFrontEnd.Models;
using DiscordLikeChatApp.Models;

namespace DiscordLikeChatApp.Services {
    public class ApiService {
        private readonly HttpClient _httpClient;

        public ApiService() {
            _httpClient = new HttpClient {
                BaseAddress = new Uri("http://localhost:8080/")
            };

            // Configure les en-têtes par défaut (ici, on accepte du JSON)
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Méthode GET générique
        public async Task<T> GetAsync<T>(string endpoint) {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        // Méthode POST générique
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data) {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        // Méthode PUT générique
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data) {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        // Méthode DELETE générique
        public async Task<bool> DeleteAsync(string endpoint) {
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }

        // Méthode PATCH générique (HttpClient ne possède pas de méthode Patch par défaut)
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

        // Exemple de méthode pour créer un nouveau canal dans un serveur
        public async Task<Channel> CreateChannelAsync(string serverId, string channelName) {
            var payload = new {
                name = channelName
            };
            return await PostAsync<object, Channel>($"api/servers/{serverId}/channels", payload);
        }

        // Exemple de méthode pour supprimer un canal
        public async Task<bool> DeleteChannelAsync(string serverId, string channelId) {
            return await DeleteAsync($"api/servers/{serverId}/channels/{channelId}");
        }

        // Exemple de méthode pour mettre à jour un canal via PUT
        public async Task<Channel> UpdateChannelAsync(string serverId, string channelId, string newChannelName) {
            var payload = new {
                name = newChannelName
            };
            return await PutAsync<object, Channel>($"api/servers/{serverId}/channels/{channelId}", payload);
        }

        // Exemple de méthode pour appliquer un patch sur un canal
        public async Task<Channel> PatchChannelAsync(string serverId, string channelId, object patchData) {
            return await PatchAsync<object, Channel>($"api/servers/{serverId}/channels/{channelId}", patchData);
        }
    
     public async Task<bool> LoginAsync(string username, string password) {
            var payload = new {
                Username = username,
                Password = password
            };
            return await PostAsync<object, bool>("api/login", payload);
        }
    }
}
