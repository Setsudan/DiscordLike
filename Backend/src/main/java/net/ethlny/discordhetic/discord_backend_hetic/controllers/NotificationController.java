package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import net.ethlny.discordhetic.discord_backend_hetic.models.Notification;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.notification.NotificationService;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/notifications")
public class NotificationController {

    private final NotificationService notificationService;

    public NotificationController(NotificationService notificationService) {
        this.notificationService = notificationService;
    }

    // REST endpoint to fetch notifications for the current user.
    @GetMapping
    public StandardResponse getNotifications(@AuthenticationPrincipal UserDetailsImpl currentUser) {
        List<Notification> notifications = notificationService.getNotificationsForUser(currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Notifications retrieved", notifications, null);
    }

    // REST endpoint to create a notification for the current user.
    @PostMapping
    public StandardResponse sendNotification(@AuthenticationPrincipal UserDetailsImpl currentUser,
            @RequestBody Notification notificationRequest) {
        // Use the current authenticated user as the target.
        Notification notification = notificationService.sendNotification(currentUser.getUser(),
                notificationRequest.getMessage(), notificationRequest.getType());
        return new StandardResponse(HttpStatus.OK.value(), "Notification sent", notification, null);
    }
}
