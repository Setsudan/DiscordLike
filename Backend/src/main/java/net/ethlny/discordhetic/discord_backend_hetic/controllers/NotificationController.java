package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import java.util.List;

import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import net.ethlny.discordhetic.discord_backend_hetic.models.Notification;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.notification.NotificationService;

@RestController
@RequestMapping("/notifications")
@Tag(name = "Notifications", description = "Envoi et récupération des notifications utilisateur")
public class NotificationController {

    private final NotificationService notificationService;

    public NotificationController(NotificationService notificationService) {
        this.notificationService = notificationService;
    }

    // REST endpoint to fetch notifications for the current user.
    @Operation(
    summary = "Récupérer les notifications de l'utilisateur",
    description = "Renvoie toutes les notifications associées à l'utilisateur actuellement connecté.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Notifications récupérées",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @GetMapping
    public StandardResponse getNotifications(@AuthenticationPrincipal UserDetailsImpl currentUser) {
        List<Notification> notifications = notificationService.getNotificationsForUser(currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Notifications retrieved", notifications, null);
    }

    // REST endpoint to create a notification for the current user.
    @Operation(
    summary = "Envoyer une notification",
    description = "Crée une nouvelle notification pour l'utilisateur actuellement connecté.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Notification envoyée",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "400", description = "Requête invalide"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @PostMapping
    public StandardResponse sendNotification(@AuthenticationPrincipal UserDetailsImpl currentUser,
            @RequestBody Notification notificationRequest) {
        // Use the current authenticated user as the target.
        Notification notification = notificationService.sendNotification(currentUser.getUser(),
                notificationRequest.getMessage(), notificationRequest.getType());
        return new StandardResponse(HttpStatus.OK.value(), "Notification sent", notification, null);
    }
}
