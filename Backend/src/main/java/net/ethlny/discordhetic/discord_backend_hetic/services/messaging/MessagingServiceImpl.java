package net.ethlny.discordhetic.discord_backend_hetic.services;

import net.ethlny.discordhetic.discord_backend_hetic.models.Message;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.MessageRepository;
import org.springframework.stereotype.Service;
import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;

@Service
public class MessagingServiceImpl implements MessagingService {

    private final MessageRepository messageRepository;

    public MessagingServiceImpl(MessageRepository messageRepository) {
        this.messageRepository = messageRepository;
    }

    @Override
    public Message sendMessage(Message message) {
        // Set timestamp if not provided
        if (message.getTimestamp() == null) {
            message.setTimestamp(LocalDateTime.now());
        }
        // Additional business logic (like checking sender permissions, channel
        // validity, etc.) can be added here.
        return messageRepository.save(message);
    }

    @Override
    public List<Message> getMessagesForChannel(UUID channelId) {
        return messageRepository.findByChannelIdOrderByTimestampAsc(channelId);
    }
}
