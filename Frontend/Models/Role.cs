using System;

namespace DiscordFrontEnd.Models {
    public class Role {
        public Guid Id {
            get; set;
        }
        public string Name {
            get; set;
        }
        public string Description {
            get; set;
        }

        public Role() {
        }

        public Role(string name) {
            Name = name;
        }
    }
}
