package net.ethlny.discordhetic.discord_backend_hetic.repositories;

import net.ethlny.discordhetic.discord_backend_hetic.models.Channel;
import net.ethlny.discordhetic.discord_backend_hetic.models.ChannelType;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.UUID;

@Repository
public interface ChannelRepository extends JpaRepository<Channel, UUID> {
    List<Channel> findByCreatedBy(User createdBy);

    // Custom query to find channels by their type
    List<Channel> findByType(String type);

    List<Channel> findByTypeAndCreatedBy(ChannelType type, User createdBy);

}
