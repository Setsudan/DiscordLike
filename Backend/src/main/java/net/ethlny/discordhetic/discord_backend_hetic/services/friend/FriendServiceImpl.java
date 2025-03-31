package net.ethlny.discordhetic.discord_backend_hetic.services.friend;

import net.ethlny.discordhetic.discord_backend_hetic.exception.UnauthorizedOperationException;
import net.ethlny.discordhetic.discord_backend_hetic.models.Friendship;
import net.ethlny.discordhetic.discord_backend_hetic.models.FriendshipStatus;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.FriendshipRepository;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.UserRepository;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Service
public class FriendServiceImpl implements FriendService {

    private final FriendshipRepository friendshipRepository;
    private final UserRepository userRepository;

    public FriendServiceImpl(FriendshipRepository friendshipRepository, UserRepository userRepository) {
        this.friendshipRepository = friendshipRepository;
        this.userRepository = userRepository;
    }

    @Override
    public Friendship sendFriendRequest(User requester, UUID recipientId) {
        User recipient = userRepository.findById(recipientId)
                .orElseThrow(() -> new IllegalArgumentException("Recipient not found"));
        // Check for an existing friendship in either direction.
        if (friendshipRepository.existsByRequesterAndRecipientOrRequesterAndRecipient(
                requester, recipient, recipient, requester)) {
            throw new IllegalArgumentException("Friend request already exists or you are already friends");
        }
        Friendship friendship = new Friendship(requester, recipient, FriendshipStatus.PENDING);
        return friendshipRepository.save(friendship);
    }

    @Override
    public Friendship acceptFriendRequest(UUID friendshipId, User currentUser) {
        Friendship friendship = friendshipRepository.findById(friendshipId)
                .orElseThrow(() -> new IllegalArgumentException("Friend request not found"));
        if (!friendship.getRecipient().getId().equals(currentUser.getId())) {
            throw new UnauthorizedOperationException("You are not authorized to accept this friend request");
        }
        friendship.setStatus(FriendshipStatus.ACCEPTED);
        return friendshipRepository.save(friendship);
    }

    @Override
    public Friendship declineFriendRequest(UUID friendshipId, User currentUser) {
        Friendship friendship = friendshipRepository.findById(friendshipId)
                .orElseThrow(() -> new IllegalArgumentException("Friend request not found"));
        if (!friendship.getRecipient().getId().equals(currentUser.getId())) {
            throw new UnauthorizedOperationException("You are not authorized to decline this friend request");
        }
        friendship.setStatus(FriendshipStatus.DECLINED);
        return friendshipRepository.save(friendship);
    }

    @Override
    public List<User> getFriends(User user) {
        List<User> friends = new ArrayList<>();
        List<Friendship> asRequester = friendshipRepository.findByRequesterAndStatus(user, FriendshipStatus.ACCEPTED);
        List<Friendship> asRecipient = friendshipRepository.findByRecipientAndStatus(user, FriendshipStatus.ACCEPTED);
        asRequester.forEach(f -> friends.add(f.getRecipient()));
        asRecipient.forEach(f -> friends.add(f.getRequester()));
        return friends;
    }

    @Override
    public List<Friendship> getPendingRequests(User user) {
        return friendshipRepository.findByRecipientAndStatus(user, FriendshipStatus.PENDING);
    }
}
