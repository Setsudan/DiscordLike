package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import net.ethlny.discordhetic.discord_backend_hetic.models.Channel;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.channel.ChannelService;

import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@RestController
@RequestMapping("/channels")
public class ChannelController {

    private final ChannelService channelService;

    public ChannelController(ChannelService channelService) {
        this.channelService = channelService;
    }

    @GetMapping
    public StandardResponse getAllChannels() {
        List<Channel> channels = channelService.getAllChannels();
        return new StandardResponse(200, "Success", channels, null);
    }

    @GetMapping("/{id}")
    public StandardResponse getChannelById(@PathVariable("id") UUID id) {
        Optional<Channel> channelOpt = channelService.getChannelById(id);
        if (channelOpt.isPresent()) {
            return new StandardResponse(200, "Success", channelOpt.get(), null);
        } else {
            return new StandardResponse(404, "Channel not found", null, null);
        }
    }

    @PostMapping
    public StandardResponse createChannel(@RequestBody Channel channel,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        // Override the createdBy field with the current authenticated user
        channel.setCreatedBy(currentUser.getUser());
        Channel createdChannel = channelService.createChannel(channel);
        return new StandardResponse(201, "Channel created", createdChannel, null);
    }

    @PutMapping("/{id}")
    public StandardResponse updateChannel(@PathVariable("id") UUID id,
            @RequestBody Channel channel,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        // Ensure the channel ID is set so that the correct channel is updated
        channel.setId(id);
        Channel updatedChannel = channelService.updateChannel(channel, currentUser.getUser());
        return new StandardResponse(200, "Channel updated", updatedChannel, null);
    }

    @DeleteMapping("/{id}")
    public StandardResponse deleteChannel(@PathVariable("id") UUID id,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        channelService.deleteChannel(id, currentUser.getUser());
        return new StandardResponse(200, "Channel deleted", null, null);
    }
}
