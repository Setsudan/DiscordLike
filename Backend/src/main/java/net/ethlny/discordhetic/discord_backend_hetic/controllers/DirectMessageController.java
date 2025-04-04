package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import java.util.List;
import java.util.UUID;

import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import net.ethlny.discordhetic.discord_backend_hetic.models.DirectMessage;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.UserRepository;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.direct.DirectMessageService;

@RestController
@RequestMapping("/direct-messages")
@Tag(name = "Messages privés", description = "Messagerie privée entre utilisateurs")
public class DirectMessageController {

    private final DirectMessageService directMessageService;
    private final UserRepository userRepository;

    public DirectMessageController(DirectMessageService directMessageService, UserRepository userRepository) {
        this.directMessageService = directMessageService;
        this.userRepository = userRepository;
    }

    // Send a direct message.
    @Operation(
    summary = "Envoyer un message privé",
    description = "Envoie un message direct (privé) à un autre utilisateur via son UUID.",
    responses = {
        @ApiResponse(responseCode = "201", description = "Message envoyé",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "400", description = "Requête invalide"),
        @ApiResponse(responseCode = "404", description = "Destinataire introuvable"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @PostMapping
    public StandardResponse sendDirectMessage(@AuthenticationPrincipal UserDetailsImpl currentUser,
            @RequestParam UUID recipientId,
            @RequestParam String content) {
        DirectMessage dm = directMessageService.sendDirectMessage(currentUser.getUser(), recipientId, content);
        return new StandardResponse(HttpStatus.CREATED.value(), "Direct message sent", dm, null);
    }

    // Retrieve the conversation between the current user and another user.
    @Operation(
    summary = "Récupérer une conversation",
    description = "Renvoie l'historique des messages entre l'utilisateur connecté et un autre utilisateur.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Conversation récupérée avec succès",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "404", description = "Utilisateur introuvable"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @GetMapping("/conversation/{userId}")
    public StandardResponse getConversation(@AuthenticationPrincipal UserDetailsImpl currentUser,
            @PathVariable("userId") UUID userId) {
        User otherUser = userRepository.findById(userId)
                .orElseThrow(() -> new IllegalArgumentException("User not found"));
        List<DirectMessage> conversation = directMessageService.getConversation(currentUser.getUser(), otherUser);
        return new StandardResponse(HttpStatus.OK.value(), "Conversation retrieved", conversation, null);
    }
}
