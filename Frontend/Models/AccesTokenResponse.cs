namespace DiscordLikeChatApp.Models {
    public class AccessTokenResponse {
     
        public bool IsSuccess {
            get; set;
        }
        public string AccessToken {
            get; set;
        }
        public string ErrorMessage {
            get; set;
        }
    }
}
