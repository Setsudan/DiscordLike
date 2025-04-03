using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordLikeChatApp.Models {
    class Message {

        public Guid ChannelId {
            get; set;
        }

        public string Content {
            get; set;
        }

        public string AttachmentUrl {
            get; set;
        }

        public string Sender {
            get; set;
        }
    }
}

