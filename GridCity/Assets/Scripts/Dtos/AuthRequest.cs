using Newtonsoft.Json;

namespace Assets.Scripts.Dtos
{
    [System.Serializable]
    public class AuthRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
