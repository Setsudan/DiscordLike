package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import net.ethlny.discordhetic.discord_backend_hetic.models.Message;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.MessageRequestDTO;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.UserRepository;
import net.ethlny.discordhetic.discord_backend_hetic.services.messaging.MessagingService;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.messaging.handler.annotation.DestinationVariable;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.handler.annotation.SendTo;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.stereotype.Controller;

import java.util.UUID;

@Controller
public class ChannelMessagingController {

    private final MessagingService messagingService;

    public ChannelMessagingController(MessagingService messagingService) {
        this.messagingService = messagingService;
    }

    @Autowired
    private UserRepository userRepository;

    @MessageMapping("/channel/{channelId}/send")
    @SendTo("/topic/channel/{channelId}")
    public StandardResponse sendMessage(@DestinationVariable("channelId") String channelId,
            @Payload MessageRequestDTO messageRequest,
            SimpMessageHeaderAccessor headerAccessor) {

        System.out.println("Message from sender: " + messageRequest.getSender() +
                " is sending a message: " + messageRequest.getContent());

        // Parse the sender UUID from the request payload
        UUID senderUuid = messageRequest.getSender();

        // Retrieve the user based on the UUID from the repository
        User sender = userRepository.findById(senderUuid)
                .orElse(null);

        if (sender == null) {
            return new StandardResponse(404, "Sender not found", null, "Sender with id " + senderUuid + " not found");
        }

        Message message = new Message();
        message.setSender(sender);
        message.setContent(messageRequest.getContent());
        message.setTimestamp(messageRequest.getTimestamp());
        message.setChannelId(UUID.fromString(channelId));
        message.setAttachmentUrl(messageRequest.getAttachmentUrl());

        Message savedMessage = messagingService.sendMessage(message);

        return new StandardResponse(200, "Message sent", savedMessage, null);
    }

}
