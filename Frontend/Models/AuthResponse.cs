namespace DiscordHeticWpf.Models
{
    public class AuthResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public AuthData data { get; set; }
        public object error { get; set; }
    }
}
