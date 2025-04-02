using System;
using System.Collections.Generic;

namespace DiscordLikeChatApp.Services {
    public class UserSession {
        private readonly Dictionary<string, object> _sessionData = new Dictionary<string, object>();

        public void Set<T>(string key, T value) {
            _sessionData[key] = value;
        }

        public T Get<T>(string key) {
            if (_sessionData.TryGetValue(key, out var value)) {
                return (T)value;
            }
            else {
                T data = default(T);
                _sessionData[key] = data;
                return data;
            }
        }
    }
}
