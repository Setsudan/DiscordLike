package net.ethlny.discordhetic.discord_backend_hetic.services.channel;

import net.ethlny.discordhetic.discord_backend_hetic.models.Channel;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface ChannelService {
    Channel createChannel(Channel channel);

    Optional<Channel> getChannelById(UUID channelId);

    List<Channel> getAllChannels();

    Channel updateChannel(Channel channel, User currentUser);

    void deleteChannel(UUID channelId, User currentUser);
}
