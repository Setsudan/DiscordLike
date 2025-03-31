package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import net.ethlny.discordhetic.discord_backend_hetic.models.DirectMessage;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.UserRepository;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.direct.DirectMessageService;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@RestController
@RequestMapping("/direct-messages")
public class DirectMessageController {

    private final DirectMessageService directMessageService;
    private final UserRepository userRepository;

    public DirectMessageController(DirectMessageService directMessageService, UserRepository userRepository) {
        this.directMessageService = directMessageService;
        this.userRepository = userRepository;
    }

    // Send a direct message.
    @PostMapping
    public StandardResponse sendDirectMessage(@AuthenticationPrincipal UserDetailsImpl currentUser,
            @RequestParam UUID recipientId,
            @RequestParam String content) {
        DirectMessage dm = directMessageService.sendDirectMessage(currentUser.getUser(), recipientId, content);
        return new StandardResponse(HttpStatus.CREATED.value(), "Direct message sent", dm, null);
    }

    // Retrieve the conversation between the current user and another user.
    @GetMapping("/conversation/{userId}")
    public StandardResponse getConversation(@AuthenticationPrincipal UserDetailsImpl currentUser,
            @PathVariable("userId") UUID userId) {
        User otherUser = userRepository.findById(userId)
                .orElseThrow(() -> new IllegalArgumentException("User not found"));
        List<DirectMessage> conversation = directMessageService.getConversation(currentUser.getUser(), otherUser);
        return new StandardResponse(HttpStatus.OK.value(), "Conversation retrieved", conversation, null);
    }
}
