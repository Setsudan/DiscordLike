package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import net.ethlny.discordhetic.discord_backend_hetic.models.Message;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.messaging.MessagingService;
import org.springframework.messaging.handler.annotation.DestinationVariable;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.SendTo;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.stereotype.Controller;

import java.util.UUID;

@Controller
public class ChannelMessagingController {

    private final MessagingService messagingService;

    public ChannelMessagingController(MessagingService messagingService) {
        this.messagingService = messagingService;
    }

    /**
     * When a client sends a message to /app/channel/{channelId}/send,
     * this method is invoked. It persists the message and broadcasts the result
     * to all subscribers of /topic/channel/{channelId}.
     */
    @MessageMapping("/channel/{channelId}/send")
    @SendTo("/topic/channel/{channelId}")
    public StandardResponse sendMessage(@DestinationVariable("channelId") String channelId,
            Message message,
            SimpMessageHeaderAccessor headerAccessor,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        // Set the sender from the authenticated user (if available)
        User sender = currentUser.getUser();
        message.setSender(sender);
        // Ensure the channelId is set correctly in the message
        message.setChannelId(UUID.fromString(channelId));
        Message savedMessage = messagingService.sendMessage(message);
        return new StandardResponse(200, "Message sent", savedMessage, null);
    }
}
