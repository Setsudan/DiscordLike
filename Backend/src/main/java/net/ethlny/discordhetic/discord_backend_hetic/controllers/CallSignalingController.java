package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import net.ethlny.discordhetic.discord_backend_hetic.models.SignalMessage;
import org.springframework.messaging.handler.annotation.DestinationVariable;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Controller;

import java.security.Principal;

@Controller
public class CallSignalingController {

    private final SimpMessagingTemplate messagingTemplate;

    public CallSignalingController(SimpMessagingTemplate messagingTemplate) {
        this.messagingTemplate = messagingTemplate;
    }

    /**
     * Receives a signaling message (SDP offer/answer or ICE candidate) from the
     * caller
     * and sends it to the intended recipient.
     *
     * The targetId should be the unique identifier of the recipient (for example,
     * their username or ID).
     * The clients must subscribe to their private queue (e.g.,
     * "/user/queue/signaling") to receive messages.
     */
    @MessageMapping("/call/{targetId}/signal")
    public void sendSignal(@DestinationVariable("targetId") String targetId,
            SignalMessage message,
            Principal principal) {
        // Optionally log the event:
        System.out.println("Received signal of type " + message.getType() +
                " from " + principal.getName() +
                " for target " + targetId);

        // Forward the signaling message to the target user on a private destination.
        messagingTemplate.convertAndSendToUser(targetId, "/queue/signaling", message);
    }
}
