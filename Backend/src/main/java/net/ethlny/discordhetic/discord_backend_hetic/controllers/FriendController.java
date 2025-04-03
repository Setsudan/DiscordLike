package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import java.util.List;
import java.util.UUID;

import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import net.ethlny.discordhetic.discord_backend_hetic.models.Friendship;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.friend.FriendService;

@RestController
@RequestMapping("/friends")
@Tag(name = "Amis", description = "Demandes d'amis et gestion des relations")
public class FriendController {

    private final FriendService friendService;

    public FriendController(FriendService friendService) {
        this.friendService = friendService;
    }

    // Send a friend request.
    @Operation(
    summary = "Envoyer une demande d'ami",
    description = "Envoie une demande d'amitié à un autre utilisateur (UUID requis).",
    responses = {
        @ApiResponse(responseCode = "201", description = "Demande envoyée",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "400", description = "Utilisateur invalide ou déjà en relation"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @PostMapping("/request")
    public StandardResponse sendFriendRequest(@AuthenticationPrincipal UserDetailsImpl currentUser,
            @RequestParam UUID recipientId) {
        Friendship friendship = friendService.sendFriendRequest(currentUser.getUser(), recipientId);
        return new StandardResponse(HttpStatus.CREATED.value(), "Friend request sent", friendship, null);
    }

    // Accept a friend request.
    @Operation(
    summary = "Accepter une demande d'ami",
    description = "Accepte une demande d'amitié existante avec l'ID de la relation (UUID).",
    responses = {
        @ApiResponse(responseCode = "200", description = "Demande acceptée",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "404", description = "Demande non trouvée"),
        @ApiResponse(responseCode = "403", description = "Non autorisé"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @PostMapping("/{friendshipId}/accept")
    public StandardResponse acceptFriendRequest(@PathVariable UUID friendshipId,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        Friendship friendship = friendService.acceptFriendRequest(friendshipId, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Friend request accepted", friendship, null);
    }

    // Decline a friend request.
    @Operation(
    summary = "Refuser une demande d'ami",
    description = "Refuse une demande d'amitié en attente.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Demande refusée"),
        @ApiResponse(responseCode = "404", description = "Demande non trouvée"),
        @ApiResponse(responseCode = "403", description = "Non autorisé"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @PostMapping("/{friendshipId}/decline")
    public StandardResponse declineFriendRequest(@PathVariable UUID friendshipId,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        Friendship friendship = friendService.declineFriendRequest(friendshipId, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Friend request declined", friendship, null);
    }

    // Get list of accepted friends.
    @Operation(
    summary = "Récupérer la liste des amis",
    description = "Renvoie tous les utilisateurs avec qui l'utilisateur courant est ami.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Liste d'amis récupérée",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @GetMapping
    public StandardResponse getFriends(@AuthenticationPrincipal UserDetailsImpl currentUser) {
        List<User> friends = friendService.getFriends(currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Friends retrieved", friends, null);
    }

    // Get list of pending friend requests.
    @Operation(
    summary = "Récupérer les demandes d'ami en attente",
    description = "Renvoie toutes les demandes d'ami reçues mais non encore acceptées/refusées.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Demandes récupérées",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @GetMapping("/requests")
    public StandardResponse getPendingRequests(@AuthenticationPrincipal UserDetailsImpl currentUser) {
        List<Friendship> pendingRequests = friendService.getPendingRequests(currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Pending friend requests retrieved", pendingRequests, null);
    }
}
