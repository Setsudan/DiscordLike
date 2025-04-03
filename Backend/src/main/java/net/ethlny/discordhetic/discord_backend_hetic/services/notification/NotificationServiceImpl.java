package net.ethlny.discordhetic.discord_backend_hetic.services.notification;

import net.ethlny.discordhetic.discord_backend_hetic.models.Notification;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.NotificationRepository;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.List;

@Service
public class NotificationServiceImpl implements NotificationService {

    private final NotificationRepository notificationRepository;
    private final SimpMessagingTemplate messagingTemplate;

    public NotificationServiceImpl(NotificationRepository notificationRepository,
            SimpMessagingTemplate messagingTemplate) {
        this.notificationRepository = notificationRepository;
        this.messagingTemplate = messagingTemplate;
    }

    @Override
    public Notification sendNotification(User user, String message, String type) {
        Notification notification = new Notification();
        notification.setUser(user);
        notification.setMessage(message);
        notification.setType(type);
        notification.setCreatedAt(LocalDateTime.now());

        Notification savedNotification = notificationRepository.save(notification);
        // Push the notification via WebSocket on a user-specific destination.
        // Here, we assume the user's username is the identifier for WebSocket
        // messaging.
        messagingTemplate.convertAndSendToUser(user.getUsername(), "/queue/notifications", savedNotification);
        return savedNotification;
    }

    @Override
    public List<Notification> getNotificationsForUser(User user) {
        return notificationRepository.findByUserOrderByCreatedAtDesc(user);
    }
}
