package net.ethlny.discordhetic.discord_backend_hetic.repositories;

import net.ethlny.discordhetic.discord_backend_hetic.models.Notification;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import java.util.List;
import java.util.UUID;

@Repository
public interface NotificationRepository extends JpaRepository<Notification, UUID> {
    List<Notification> findByUserOrderByCreatedAtDesc(User user);
}
