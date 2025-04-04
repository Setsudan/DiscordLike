using System;
using System.Collections.Generic;

namespace DiscordFrontEnd.Models {
    public class User {
        public Guid Id {
            get; set;
        }
        public string Username {
            get; set;
        }
        public string Email {
            get; set;
        }
        public string Password {
            get; set;
        }
        public string DisplayName {
            get; set;
        }
        public string AvatarUrl {
            get; set;
        }
        public DateTime CreatedAt {
            get; set;
        }
        public DateTime? LastLogin {
            get; set;
        }
        public bool Online {
            get; set;
        }
        public string ActivationKey {
            get; set;
        }
        public ICollection<Role> Roles {
            get; set;
        }

        public User() {
            CreatedAt = DateTime.Now;
            Online = false;
            Roles = new HashSet<Role>();
        }

        public User(string username, string email, string password) : this() {
            Username = username;
            Email = email;
            Password = password;
        }
    }
}
