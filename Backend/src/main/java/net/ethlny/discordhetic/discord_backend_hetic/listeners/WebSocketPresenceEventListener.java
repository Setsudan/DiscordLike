package net.ethlny.discordhetic.discord_backend_hetic.listeners;

import net.ethlny.discordhetic.discord_backend_hetic.services.presence.PresenceService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.event.EventListener;
import org.springframework.messaging.simp.stomp.StompHeaderAccessor;
import org.springframework.stereotype.Component;
import org.springframework.web.socket.messaging.SessionConnectEvent;
import org.springframework.web.socket.messaging.SessionDisconnectEvent;

@Component
public class WebSocketPresenceEventListener {

    private static final Logger logger = LoggerFactory.getLogger(WebSocketPresenceEventListener.class);

    private final PresenceService presenceService;

    public WebSocketPresenceEventListener(PresenceService presenceService) {
        this.presenceService = presenceService;
    }

    @EventListener
    public void handleSessionConnectedEvent(SessionConnectEvent event) {
        StompHeaderAccessor sha = StompHeaderAccessor.wrap(event.getMessage());
        String sessionId = sha.getSessionId();
        // Here you would extract your authenticated User (or its identifier) from the
        // header or Principal.
        // For demonstration, we assume the user is set via a header "userId" (this is
        // simplistic).
        String userId = sha.getFirstNativeHeader("userId");
        if (userId != null) {
            // In a real app, you’d look up the User from the database.
            // For now, we'll log the connection.
            logger.info("User with id {} connected on session {}", userId, sessionId);
            // presenceService.userConnected(sessionId, user); <-- populate user
            // accordingly.
        } else {
            logger.info("New WebSocket connection, sessionId: {}", sessionId);
        }
    }

    @EventListener
    public void handleSessionDisconnectEvent(SessionDisconnectEvent event) {
        String sessionId = event.getSessionId();
        presenceService.userDisconnected(sessionId);
        logger.info("WebSocket disconnected, sessionId: {}", sessionId);
    }
}
