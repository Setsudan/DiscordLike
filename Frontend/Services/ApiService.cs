using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace DiscordHeticWpf.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        public string BaseUrl { get; }

        public ApiService(string baseUrl, string token = null)
        {
            BaseUrl = baseUrl;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };

            if (!string.IsNullOrEmpty(token))
            {
                SetToken(token);
            }
        }

        /// <summary>
        /// Sets or updates the Authorization header with a bearer token.
        /// </summary>
        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        /// <summary>
        /// Sends a GET request and deserializes the JSON response into type T.
        /// </summary>
        public async Task<T> GetAsync<T>(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Sends a POST request with JSON-serialized data and returns a deserialized response of type T.
        /// </summary>
        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Sends a PUT request with JSON-serialized data and returns a deserialized response of type T.
        /// </summary>
        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Sends a DELETE request.
        /// </summary>
        public async Task DeleteAsync(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
        }
    }
}
