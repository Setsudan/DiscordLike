package net.ethlny.discordhetic.discord_backend_hetic.services.friend;

import net.ethlny.discordhetic.discord_backend_hetic.models.Friendship;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;

import java.util.List;
import java.util.UUID;

public interface FriendService {
    Friendship sendFriendRequest(User requester, UUID recipientId);

    Friendship acceptFriendRequest(UUID friendshipId, User currentUser);

    Friendship declineFriendRequest(UUID friendshipId, User currentUser);

    List<User> getFriends(User user);

    List<Friendship> getPendingRequests(User user);
}
