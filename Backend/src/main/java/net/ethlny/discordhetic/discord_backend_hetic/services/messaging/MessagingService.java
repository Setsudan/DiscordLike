package net.ethlny.discordhetic.discord_backend_hetic.services.messaging;

import net.ethlny.discordhetic.discord_backend_hetic.models.Message;
import java.util.List;
import java.util.UUID;

public interface MessagingService {
    Message sendMessage(Message message);

    List<Message> getMessagesForChannel(UUID channelId);
}
