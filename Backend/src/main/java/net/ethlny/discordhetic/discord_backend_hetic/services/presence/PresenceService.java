package net.ethlny.discordhetic.discord_backend_hetic.services.presence;

import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import java.util.Set;

public interface PresenceService {
    void userConnected(String sessionId, User user);

    void userDisconnected(String sessionId);

    Set<User> getOnlineUsers();
}
