using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordLikeChatApp.Models
{
    public class MessagePayload {
        public string Id {
            get; set;
        }
        public UserInfo Sender {
            get; set;
        }
        public string Content {
            get; set;
        }
        public DateTime Timestamp {
            get; set;
        }
        public string ChannelId {
            get; set;
        }
        public string AttachmentUrl {
            get; set;
        }
    }
}
