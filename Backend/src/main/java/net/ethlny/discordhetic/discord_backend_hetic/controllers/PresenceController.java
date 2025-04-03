package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import java.util.Set;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.presence.PresenceService;

@RestController
@RequestMapping("/presence")
@Tag(name = "Présence", description = "Vérification de la présence des utilisateurs en ligne")
public class PresenceController {

    private final PresenceService presenceService;

    public PresenceController(PresenceService presenceService) {
        this.presenceService = presenceService;
    }

    @Operation(
    summary = "Lister les utilisateurs en ligne",
    description = "Renvoie la liste des utilisateurs actuellement connectés en temps réel.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Utilisateurs en ligne récupérés",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @GetMapping("/online")
    public StandardResponse getOnlineUsers() {
        Set<User> onlineUsers = presenceService.getOnlineUsers();
        return new StandardResponse(HttpStatus.OK.value(), "Online users retrieved", onlineUsers, null);
    }
}
