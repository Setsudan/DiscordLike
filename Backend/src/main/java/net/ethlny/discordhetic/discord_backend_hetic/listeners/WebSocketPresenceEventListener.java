package net.ethlny.discordhetic.discord_backend_hetic.listeners;

import net.ethlny.discordhetic.discord_backend_hetic.services.presence.PresenceService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.event.EventListener;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.messaging.simp.stomp.StompHeaderAccessor;
import org.springframework.stereotype.Component;
import org.springframework.web.socket.messaging.SessionConnectEvent;
import org.springframework.web.socket.messaging.SessionDisconnectEvent;

@Component
public class WebSocketPresenceEventListener {

    private static final Logger logger = LoggerFactory.getLogger(WebSocketPresenceEventListener.class);

    private final PresenceService presenceService;
    private final SimpMessagingTemplate messagingTemplate;

    public WebSocketPresenceEventListener(PresenceService presenceService, SimpMessagingTemplate messagingTemplate) {
        this.presenceService = presenceService;
        this.messagingTemplate = messagingTemplate;
    }

    @EventListener
    public void handleSessionConnectedEvent(SessionConnectEvent event) {
        StompHeaderAccessor sha = StompHeaderAccessor.wrap(event.getMessage());
        String sessionId = sha.getSessionId();
        String userId = sha.getFirstNativeHeader("userId");
        if (userId != null) {
            // You would look up and construct the User entity based on userId.
            // For demonstration, we'll assume presenceService.userConnected(...) is called
            // with a valid User.
            logger.info("User with id {} connected on session {}", userId, sessionId);
            // presenceService.userConnected(sessionId, user); // Provide proper User
            // instance
        } else {
            logger.info("New WebSocket connection, sessionId: {}", sessionId);
        }
        // Broadcast updated presence (for example, the list of online users)
        messagingTemplate.convertAndSend("/topic/presence", presenceService.getOnlineUsers());
    }

    @EventListener
    public void handleSessionDisconnectEvent(SessionDisconnectEvent event) {
        String sessionId = event.getSessionId();
        presenceService.userDisconnected(sessionId);
        logger.info("WebSocket disconnected, sessionId: {}", sessionId);
        // Broadcast updated presence list
        messagingTemplate.convertAndSend("/topic/presence", presenceService.getOnlineUsers());
    }
}
