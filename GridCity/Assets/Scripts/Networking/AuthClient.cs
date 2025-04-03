using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using Assets.Scripts.Dtos;

namespace Assets.Scripts.Networking
{
    public class AuthClient
    {
        private const string AUTH_URL = "https://localhost:7259/api/auth";
        private readonly HttpClient _client;

        public AuthClient()
        {
            _client = new HttpClient();
        }

        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            try
            {
                var loginData = new AuthRequest { Username = username, Password = password };
                string jsonData = JsonConvert.SerializeObject(loginData);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync($"{AUTH_URL}/login", content);
                string responseText = await response.Content.ReadAsStringAsync();
                AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseText);
                return authResponse;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Login request failed: {ex.Message}");
                return null;
            }
        }

        public async Task<AuthResponse> RegisterAsync(string username, string password)
        {
            try
            {
                var loginData = new AuthRequest { Username = username, Password = password };
                string jsonData = JsonConvert.SerializeObject(loginData);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync($"{AUTH_URL}/register", content);
                string responseText = await response.Content.ReadAsStringAsync();
                AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseText);
                return authResponse;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Register request failed: {ex.Message}");
                return null;
            }
        }
    }
}
