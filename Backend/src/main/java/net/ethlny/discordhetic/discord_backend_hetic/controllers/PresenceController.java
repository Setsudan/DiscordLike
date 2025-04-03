package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import java.util.Set;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.presence.PresenceService;

@RestController
@RequestMapping("/presence")
public class PresenceController {

    private final PresenceService presenceService;

    public PresenceController(PresenceService presenceService) {
        this.presenceService = presenceService;
    }

    @GetMapping("/online")
    public StandardResponse getOnlineUsers() {
        Set<User> onlineUsers = presenceService.getOnlineUsers();
        return new StandardResponse(HttpStatus.OK.value(), "Online users retrieved", onlineUsers, null);
    }
}
