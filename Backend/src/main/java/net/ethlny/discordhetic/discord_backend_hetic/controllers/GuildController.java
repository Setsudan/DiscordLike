package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import net.ethlny.discordhetic.discord_backend_hetic.models.Guild;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.guild.GuildService;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@RestController
@RequestMapping("/guilds")
public class GuildController {

    private final GuildService guildService;

    public GuildController(GuildService guildService) {
        this.guildService = guildService;
    }

    @GetMapping
    public StandardResponse getAllGuilds() {
        List<Guild> guilds = guildService.getAllGuilds();
        return new StandardResponse(HttpStatus.OK.value(), "Guilds retrieved", guilds, null);
    }

    @GetMapping("/{guildId}")
    public StandardResponse getGuildById(@PathVariable("guildId") UUID guildId) {
        Optional<Guild> guildOpt = guildService.getGuildById(guildId);
        if (guildOpt.isPresent()) {
            return new StandardResponse(HttpStatus.OK.value(), "Guild found", guildOpt.get(), null);
        } else {
            return new StandardResponse(HttpStatus.NOT_FOUND.value(), "Guild not found", null, null);
        }
    }

    @PostMapping
    public StandardResponse createGuild(@RequestBody Guild guild,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        guild.setOwner(currentUser.getUser());
        Guild createdGuild = guildService.createGuild(guild);
        return new StandardResponse(HttpStatus.CREATED.value(), "Guild created", createdGuild, null);
    }

    @PutMapping("/{guildId}")
    public StandardResponse updateGuild(@PathVariable("guildId") UUID guildId,
            @RequestBody Guild guild,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        guild.setId(guildId);
        Guild updatedGuild = guildService.updateGuild(guild, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Guild updated", updatedGuild, null);
    }

    @DeleteMapping("/{guildId}")
    public StandardResponse deleteGuild(@PathVariable("guildId") UUID guildId,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        guildService.deleteGuild(guildId, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Guild deleted", null, null);
    }

    @PostMapping("/{guildId}/join")
    public StandardResponse joinGuild(@PathVariable("guildId") UUID guildId,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        Guild guild = guildService.joinGuild(guildId, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Joined guild", guild, null);
    }

    @PostMapping("/{guildId}/leave")
    public StandardResponse leaveGuild(@PathVariable("guildId") UUID guildId,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        guildService.leaveGuild(guildId, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Left guild", null, null);
    }
}
