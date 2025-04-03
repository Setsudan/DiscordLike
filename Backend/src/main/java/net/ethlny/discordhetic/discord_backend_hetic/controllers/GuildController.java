package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

import org.springframework.http.HttpStatus;
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
import net.ethlny.discordhetic.discord_backend_hetic.models.Guild;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.guild.GuildService;

@RestController
@RequestMapping("/guilds")
@Tag(name = "Guilds", description = "Gestion des serveurs (guilds)")
public class GuildController {

    private final GuildService guildService;

    public GuildController(GuildService guildService) {
        this.guildService = guildService;
    }

    @Operation(
    summary = "Lister toutes les guilds",
    description = "Renvoie toutes les guilds existantes.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Liste récupérée",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @GetMapping
    public StandardResponse getAllGuilds() {
        List<Guild> guilds = guildService.getAllGuilds();
        return new StandardResponse(HttpStatus.OK.value(), "Guilds retrieved", guilds, null);
    }

    @Operation(
    summary = "Obtenir une guild par ID",
    description = "Renvoie une guild si elle existe, sinon retourne 404.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Guild trouvée"),
        @ApiResponse(responseCode = "404", description = "Guild introuvable"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @GetMapping("/{guildId}")
    public StandardResponse getGuildById(@PathVariable("guildId") UUID guildId) {
        Optional<Guild> guildOpt = guildService.getGuildById(guildId);
        if (guildOpt.isPresent()) {
            return new StandardResponse(HttpStatus.OK.value(), "Guild found", guildOpt.get(), null);
        } else {
            return new StandardResponse(HttpStatus.NOT_FOUND.value(), "Guild not found", null, null);
        }
    }

    @Operation(
    summary = "Créer une nouvelle guild",
    description = "Crée une guild et l'associe à l'utilisateur actuel comme propriétaire.",
    responses = {
        @ApiResponse(responseCode = "201", description = "Guild créée",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "400", description = "Requête invalide"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @PostMapping
    public StandardResponse createGuild(@RequestBody Guild guild,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        guild.setOwner(currentUser.getUser());
        Guild createdGuild = guildService.createGuild(guild);
        return new StandardResponse(HttpStatus.CREATED.value(), "Guild created", createdGuild, null);
    }

    @Operation(
    summary = "Mettre à jour une guild",
    description = "Met à jour une guild si l'utilisateur est autorisé.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Guild mise à jour"),
        @ApiResponse(responseCode = "404", description = "Guild introuvable"),
        @ApiResponse(responseCode = "403", description = "Non autorisé"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @PutMapping("/{guildId}")
    public StandardResponse updateGuild(@PathVariable("guildId") UUID guildId,
            @RequestBody Guild guild,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        guild.setId(guildId);
        Guild updatedGuild = guildService.updateGuild(guild, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Guild updated", updatedGuild, null);
    }

    @Operation(
    summary = "Supprimer une guild",
    description = "Supprime une guild si l'utilisateur en est le propriétaire.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Guild supprimée"),
        @ApiResponse(responseCode = "404", description = "Guild introuvable"),
        @ApiResponse(responseCode = "403", description = "Non autorisé"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @DeleteMapping("/{guildId}")
    public StandardResponse deleteGuild(@PathVariable("guildId") UUID guildId,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        guildService.deleteGuild(guildId, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Guild deleted", null, null);
    }

    @Operation(
    summary = "Rejoindre une guild",
    description = "Permet à l'utilisateur de rejoindre une guild existante.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Guild rejointe"),
        @ApiResponse(responseCode = "404", description = "Guild introuvable"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @PostMapping("/{guildId}/join")
    public StandardResponse joinGuild(@PathVariable("guildId") UUID guildId,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        Guild guild = guildService.joinGuild(guildId, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Joined guild", guild, null);
    }

    @Operation(
    summary = "Quitter une guild",
    description = "Permet à l'utilisateur de quitter une guild à laquelle il appartient.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Guild quittée"),
        @ApiResponse(responseCode = "404", description = "Guild introuvable"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @PostMapping("/{guildId}/leave")
    public StandardResponse leaveGuild(@PathVariable("guildId") UUID guildId,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        guildService.leaveGuild(guildId, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Left guild", null, null);
    }
}
