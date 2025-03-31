package net.ethlny.discordhetic.discord_backend_hetic.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.messaging.simp.config.MessageBrokerRegistry;
import org.springframework.web.socket.config.annotation.*;

@Configuration
@EnableWebSocketMessageBroker
public class WebSocketConfig implements WebSocketMessageBrokerConfigurer {

    @Override
    public void registerStompEndpoints(StompEndpointRegistry registry) {
        // Clients will connect to this endpoint for WebSocket communication
        registry.addEndpoint("/ws")
                .setAllowedOrigins("http://localhost:3000")
                .withSockJS();
    }

    @Override
    public void configureMessageBroker(MessageBrokerRegistry registry) {
        // Messages bound for methods annotated with @MessageMapping will be prefixed
        // with /app
        registry.setApplicationDestinationPrefixes("/app");
        // Enable a simple in-memory message broker for destinations prefixed with
        // /topic
        registry.enableSimpleBroker("/topic");
    }
}
