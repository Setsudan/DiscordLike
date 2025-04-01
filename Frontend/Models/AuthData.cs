namespace DiscordHeticWpf.Models
{
    public class AuthData
    {
        // For signup response, a message and token are returned.
        public string message { get; set; }
        public string token { get; set; }
        // For signin response, additional fields are returned:
        public string type { get; set; }
        public string id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string[] roles { get; set; }
    }
}
