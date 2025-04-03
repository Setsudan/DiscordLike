using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordLikeChatApp.Models
{
    public class LoginData {
        public string Id {
            get; set;
        }
        public string Username {
            get; set;
        }
        public string Email {
            get; set;
        }
        public List<string> Roles {
            get; set;
        }
        public string Token {
            get; set;
        }
        public string Type {
            get; set;
        }
    }
}
