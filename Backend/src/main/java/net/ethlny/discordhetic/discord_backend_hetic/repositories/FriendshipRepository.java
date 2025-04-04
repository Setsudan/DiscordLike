package net.ethlny.discordhetic.discord_backend_hetic.repositories;

import net.ethlny.discordhetic.discord_backend_hetic.models.Friendship;
import net.ethlny.discordhetic.discord_backend_hetic.models.FriendshipStatus;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import org.springframework.data.repository.query.Param;

import java.util.List;
import java.util.UUID;

@Repository
public interface FriendshipRepository extends JpaRepository<Friendship, UUID> {

    List<Friendship> findByRecipient_IdAndStatus(@Param("recipientId") UUID recipientId,
            @Param("status") FriendshipStatus status);

    List<Friendship> findByRequesterAndStatus(@Param("requester") User requester,
            @Param("status") FriendshipStatus status);

    boolean existsByRequesterAndRecipientOrRequesterAndRecipient(@Param("requester") User requester,
            @Param("recipient") User recipient,
            @Param("reverseRequester") User reverseRequester, @Param("reverseRecipient") User reverseRecipient);
}