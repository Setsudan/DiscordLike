package net.ethlny.discordhetic.discord_backend_hetic.services.presence;

import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import org.springframework.stereotype.Service;
import java.util.Collections;
import java.util.HashSet;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;

@Service
public class PresenceServiceImpl implements PresenceService {

    // Map sessionId -> User
    private final ConcurrentHashMap<String, User> onlineUsers = new ConcurrentHashMap<>();

    @Override
    public void userConnected(String sessionId, User user) {
        onlineUsers.put(sessionId, user);
        // Optionally update user state (if persistent)
        user.setOnline(true);
    }

    @Override
    public void userDisconnected(String sessionId) {
        User user = onlineUsers.remove(sessionId);
        if (user != null) {
            user.setOnline(false);
        }
    }

    @Override
    public Set<User> getOnlineUsers() {
        return Collections.unmodifiableSet(new HashSet<>(onlineUsers.values()));
    }
}
