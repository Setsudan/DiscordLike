namespace DiscordHeticWpf.Models
{
    public class SignUpRequest
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        // If empty, the backend defaults to "USER"
        public string[] roles { get; set; }
    }
}
