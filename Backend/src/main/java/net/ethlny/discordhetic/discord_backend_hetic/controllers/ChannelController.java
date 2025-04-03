package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import net.ethlny.discordhetic.discord_backend_hetic.models.Channel;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.channel.ChannelService;

@RestController
@RequestMapping("/channels")
@Tag(name = "Channels", description = "Gestion des channels")
public class ChannelController {

    private final ChannelService channelService;

    public ChannelController(ChannelService channelService) {
        this.channelService = channelService;
    }

    @Operation(summary = "Récupérer tous les channels", description = "Renvoie une liste de tous les channels disponibles.", responses = {
            @ApiResponse(responseCode = "200", description = "Liste récupérée avec succès", content = @Content(schema = @Schema(implementation = StandardResponse.class))),
            @ApiResponse(responseCode = "500", description = "Erreur serveur")
    })
    @GetMapping
    public StandardResponse getAllChannels() {
        List<Channel> channels = channelService.getAllChannels();
        return new StandardResponse(200, "Success", channels, null);
    }

    @Operation(summary = "Récupérer un channel par son identifiant", description = "Renvoie le channel correspondant à l'ID fourni, ou une erreur 404 si non trouvé.", responses = {
            @ApiResponse(responseCode = "200", description = "Channel trouvé", content = @Content(schema = @Schema(implementation = StandardResponse.class))),
            @ApiResponse(responseCode = "404", description = "Channel introuvable", content = @Content(schema = @Schema(implementation = StandardResponse.class))),
            @ApiResponse(responseCode = "500", description = "Erreur serveur")
    })
    @GetMapping("/{id}")
    public StandardResponse getChannelById(@PathVariable("id") UUID id) {
        Optional<Channel> channelOpt = channelService.getChannelById(id);
        if (channelOpt.isPresent()) {
            return new StandardResponse(200, "Success", channelOpt.get(), null);
        } else {
            return new StandardResponse(404, "Channel not found", null, null);
        }
    }

    @Operation(summary = "Créer un nouveau channel", description = "Crée un nouveau channel associé à l'utilisateur connecté.", responses = {
            @ApiResponse(responseCode = "201", description = "Channel créé avec succès", content = @Content(schema = @Schema(implementation = StandardResponse.class))),
            @ApiResponse(responseCode = "400", description = "Données invalides"),
            @ApiResponse(responseCode = "500", description = "Erreur serveur")
    })
    @PostMapping
    public StandardResponse createChannel(@RequestBody Channel channel,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        // Override the createdBy field with the current authenticated user
        channel.setCreatedBy(currentUser.getUser());
        Channel createdChannel = channelService.createChannel(channel);
        return new StandardResponse(201, "Channel created", createdChannel, null);
    }

    @Operation(summary = "Mettre à jour un channel existant", description = "Met à jour un channel avec les nouvelles données et vérifie les droits de l'utilisateur.", responses = {
            @ApiResponse(responseCode = "200", description = "Channel mis à jour", content = @Content(schema = @Schema(implementation = StandardResponse.class))),
            @ApiResponse(responseCode = "404", description = "Channel non trouvé"),
            @ApiResponse(responseCode = "403", description = "Non autorisé"),
            @ApiResponse(responseCode = "500", description = "Erreur serveur")
    })
    @PutMapping("/{id}")
    public StandardResponse updateChannel(@PathVariable("id") UUID id,
            @RequestBody Channel channel,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        // Ensure the channel ID is set so that the correct channel is updated
        channel.setId(id);
        Channel updatedChannel = channelService.updateChannel(channel, currentUser.getUser());
        return new StandardResponse(200, "Channel updated", updatedChannel, null);
    }

    @Operation(summary = "Supprimer un channel", description = "Supprime un channel existant si l'utilisateur en a les droits.", responses = {
            @ApiResponse(responseCode = "200", description = "Channel supprimé"),
            @ApiResponse(responseCode = "404", description = "Channel non trouvé"),
            @ApiResponse(responseCode = "403", description = "Non autorisé"),
            @ApiResponse(responseCode = "500", description = "Erreur serveur")
    })
    @DeleteMapping("/{id}")
    public StandardResponse deleteChannel(@PathVariable("id") String id,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        try {
            UUID uuid = UUID.fromString(id);
            channelService.deleteChannel(uuid, currentUser.getUser());
            return new StandardResponse(200, "Channel deleted", null, null);
        } catch (IllegalArgumentException e) {
            return new StandardResponse(400, "Invalid UUID format", null, null);
        }
    }
}
