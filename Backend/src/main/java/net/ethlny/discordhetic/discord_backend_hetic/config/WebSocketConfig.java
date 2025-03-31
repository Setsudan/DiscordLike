package net.ethlny.discordhetic.discord_backend_hetic.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.messaging.simp.config.MessageBrokerRegistry;
import org.springframework.web.socket.config.annotation.*;

@Configuration
@EnableWebSocketMessageBroker
public class WebSocketConfig implements WebSocketMessageBrokerConfigurer {

    @Override
    public void registerStompEndpoints(StompEndpointRegistry registry) {
        // Clients connect here; SockJS fallback is enabled.
        registry.addEndpoint("/ws")
                .setAllowedOrigins("http://localhost:3000")
                .withSockJS();
    }

    @Override
    public void configureMessageBroker(MessageBrokerRegistry registry) {
        // All messages bound for @MessageMapping endpoints must be prefixed with /app.
        registry.setApplicationDestinationPrefixes("/app");
        // Enable a simple broker for public topics and private user queues.
        registry.enableSimpleBroker("/topic", "/queue");
        // Set the prefix for messages bound for specific users.
        registry.setUserDestinationPrefix("/user");
    }
}
