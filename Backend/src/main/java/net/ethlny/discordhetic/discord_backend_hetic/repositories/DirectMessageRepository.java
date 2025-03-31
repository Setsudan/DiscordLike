package net.ethlny.discordhetic.discord_backend_hetic.repositories;

import net.ethlny.discordhetic.discord_backend_hetic.models.DirectMessage;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.UUID;

@Repository
public interface DirectMessageRepository extends JpaRepository<DirectMessage, UUID> {

    @Query("SELECT dm FROM DirectMessage dm WHERE (dm.sender = :user1 AND dm.recipient = :user2) OR (dm.sender = :user2 AND dm.recipient = :user1) ORDER BY dm.timestamp ASC")
    List<DirectMessage> findConversation(@Param("user1") User user1, @Param("user2") User user2);
}
