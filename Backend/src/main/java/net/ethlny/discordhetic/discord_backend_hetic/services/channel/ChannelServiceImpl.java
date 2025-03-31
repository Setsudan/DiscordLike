package net.ethlny.discordhetic.discord_backend_hetic.services.channel;

import net.ethlny.discordhetic.discord_backend_hetic.exception.UnauthorizedOperationException;
import net.ethlny.discordhetic.discord_backend_hetic.models.Channel;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.ChannelRepository;
import org.springframework.stereotype.Service;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class ChannelServiceImpl implements ChannelService {

    private final ChannelRepository channelRepository;

    public ChannelServiceImpl(ChannelRepository channelRepository) {
        this.channelRepository = channelRepository;
    }

    @Override
    public Channel createChannel(Channel channel) {
        if (channel.getCreatedAt() == null) {
            channel.setCreatedAt(java.time.LocalDateTime.now());
        }
        return channelRepository.save(channel);
    }

    @Override
    public Optional<Channel> getChannelById(UUID channelId) {
        return channelRepository.findById(channelId);
    }

    @Override
    public List<Channel> getAllChannels() {
        return channelRepository.findAll();
    }

    @Override
    public Channel updateChannel(Channel channel, User currentUser) {
        Optional<Channel> optionalExisting = channelRepository.findById(channel.getId());
        if (optionalExisting.isEmpty()) {
            throw new IllegalArgumentException("Channel not found");
        }
        Channel existingChannel = optionalExisting.get();
        // Check if the current user is the creator of the channel
        if (!existingChannel.getCreatedBy().getId().equals(currentUser.getId())) {
            throw new UnauthorizedOperationException("You are not authorized to update this channel.");
        }
        // Update allowed fields
        existingChannel.setName(channel.getName());
        existingChannel.setDescription(channel.getDescription());
        existingChannel.setType(channel.getType());
        return channelRepository.save(existingChannel);
    }

    @Override
    public void deleteChannel(UUID channelId, User currentUser) {
        Optional<Channel> optionalChannel = channelRepository.findById(channelId);
        if (optionalChannel.isEmpty()) {
            throw new IllegalArgumentException("Channel not found");
        }
        Channel channel = optionalChannel.get();
        // Check if the current user is the creator of the channel
        if (!channel.getCreatedBy().getId().equals(currentUser.getId())) {
            throw new UnauthorizedOperationException("You are not authorized to delete this channel.");
        }
        channelRepository.deleteById(channelId);
    }
}
