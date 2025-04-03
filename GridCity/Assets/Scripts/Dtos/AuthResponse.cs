namespace Assets.Scripts.Dtos
{
    [System.Serializable]
    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
        public string Error { get; set; }
    }
}
