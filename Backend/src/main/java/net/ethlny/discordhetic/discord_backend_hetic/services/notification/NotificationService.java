package net.ethlny.discordhetic.discord_backend_hetic.services.notification;

import net.ethlny.discordhetic.discord_backend_hetic.models.Notification;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import java.util.List;

public interface NotificationService {
    Notification sendNotification(User user, String message, String type);

    List<Notification> getNotificationsForUser(User user);
}
