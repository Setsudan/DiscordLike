package net.ethlny.discordhetic.discord_backend_hetic.repositories;

import net.ethlny.discordhetic.discord_backend_hetic.models.Friendship;
import net.ethlny.discordhetic.discord_backend_hetic.models.FriendshipStatus;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.UUID;

@Repository
public interface FriendshipRepository extends JpaRepository<Friendship, UUID> {

    // Find pending requests for a given recipient using recipient's ID.
    List<Friendship> findByRecipient_IdAndStatus(UUID recipientId, FriendshipStatus status);

    // Find accepted friendships where the user is the requester.
    List<Friendship> findByRequesterAndStatus(User requester, FriendshipStatus status);

    // Check if a friendship already exists (in either direction).
    boolean existsByRequesterAndRecipientOrRequesterAndRecipient(User requester, User recipient,
            User reverseRequester, User reverseRecipient);
}
