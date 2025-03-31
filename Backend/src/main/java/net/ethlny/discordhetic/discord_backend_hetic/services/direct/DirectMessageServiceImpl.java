package net.ethlny.discordhetic.discord_backend_hetic.services.direct;

import net.ethlny.discordhetic.discord_backend_hetic.models.DirectMessage;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.DirectMessageRepository;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.UserRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.UUID;

@Service
public class DirectMessageServiceImpl implements DirectMessageService {

    private final DirectMessageRepository directMessageRepository;
    private final UserRepository userRepository;

    public DirectMessageServiceImpl(DirectMessageRepository directMessageRepository, UserRepository userRepository) {
        this.directMessageRepository = directMessageRepository;
        this.userRepository = userRepository;
    }

    @Override
    public DirectMessage sendDirectMessage(User sender, UUID recipientId, String content) {
        User recipient = userRepository.findById(recipientId)
                .orElseThrow(() -> new IllegalArgumentException("Recipient not found"));
        DirectMessage dm = new DirectMessage(sender, recipient, content);
        return directMessageRepository.save(dm);
    }

    @Override
    public List<DirectMessage> getConversation(User user1, User user2) {
        return directMessageRepository.findConversation(user1, user2);
    }
}
