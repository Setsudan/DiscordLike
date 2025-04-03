package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import java.util.List;
import java.util.UUID;

import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import net.ethlny.discordhetic.discord_backend_hetic.models.Message;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.messaging.MessagingService;

@RestController
@RequestMapping("/messages")
@Tag(name = "Messages", description = "Envoi et récupération des messages dans un channel")
public class MessageController {

    private final MessagingService messagingService;

    public MessageController(MessagingService messagingService) {
        this.messagingService = messagingService;
    }

    // Endpoint to send a new message.
    // The sender field is set from the authenticated user.
    @Operation(
    summary = "Envoyer un message dans un channel",
    description = "Crée un nouveau message et l'enregistre avec l'utilisateur authentifié comme expéditeur.",
    responses = {
        @ApiResponse(responseCode = "201", description = "Message envoyé",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "400", description = "Requête invalide"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @PostMapping
    public StandardResponse sendMessage(@RequestBody Message message,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        message.setSender(currentUser.getUser());
        Message sentMessage = messagingService.sendMessage(message);
        return new StandardResponse(201, "Message sent successfully", sentMessage, null);
    }

    // Endpoint to retrieve all messages for a given channel.
    @Operation(
    summary = "Récupérer les messages d'un channel",
    description = "Renvoie tous les messages d'un channel spécifique (par son UUID).",
    responses = {
        @ApiResponse(responseCode = "200", description = "Messages récupérés",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "404", description = "Channel non trouvé"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @GetMapping("/channel/{channelId}")
    public StandardResponse getMessagesForChannel(@PathVariable("channelId") UUID channelId) {
        List<Message> messages = messagingService.getMessagesForChannel(channelId);
        return new StandardResponse(200, "Messages retrieved successfully", messages, null);
    }
}
