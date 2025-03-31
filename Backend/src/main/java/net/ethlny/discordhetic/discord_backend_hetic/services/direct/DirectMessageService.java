package net.ethlny.discordhetic.discord_backend_hetic.services.direct;

import net.ethlny.discordhetic.discord_backend_hetic.models.DirectMessage;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;

import java.util.List;
import java.util.UUID;

public interface DirectMessageService {
    DirectMessage sendDirectMessage(User sender, UUID recipientId, String content);

    List<DirectMessage> getConversation(User user1, User user2);
}
