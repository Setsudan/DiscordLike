using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordLikeChatApp.Models
{
    public class ApiResponse<T> {
        public int Status {
            get; set;
        }
        public string Message {
            get; set;
        }
        public T Data {
            get; set;
        }
        public string Error {
            get; set;
        }
    }
}
