package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import net.ethlny.discordhetic.discord_backend_hetic.models.Message;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.messaging.MessagingService;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@RestController
@RequestMapping("/messages")
public class MessageController {

    private final MessagingService messagingService;

    public MessageController(MessagingService messagingService) {
        this.messagingService = messagingService;
    }

    // Endpoint to send a new message.
    // The sender field is set from the authenticated user.
    @PostMapping
    public StandardResponse sendMessage(@RequestBody Message message,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        message.setSender(currentUser.getUser());
        Message sentMessage = messagingService.sendMessage(message);
        return new StandardResponse(201, "Message sent successfully", sentMessage, null);
    }

    // Endpoint to retrieve all messages for a given channel.
    @GetMapping("/channel/{channelId}")
    public StandardResponse getMessagesForChannel(@PathVariable("channelId") UUID channelId) {
        List<Message> messages = messagingService.getMessagesForChannel(channelId);
        return new StandardResponse(200, "Messages retrieved successfully", messages, null);
    }
}
