package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import net.ethlny.discordhetic.discord_backend_hetic.models.Friendship;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.friend.FriendService;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@RestController
@RequestMapping("/friends")
public class FriendController {

    private final FriendService friendService;

    public FriendController(FriendService friendService) {
        this.friendService = friendService;
    }

    // Send a friend request.
    @PostMapping("/request")
    public StandardResponse sendFriendRequest(@AuthenticationPrincipal UserDetailsImpl currentUser,
            @RequestParam UUID recipientId) {
        Friendship friendship = friendService.sendFriendRequest(currentUser.getUser(), recipientId);
        return new StandardResponse(HttpStatus.CREATED.value(), "Friend request sent", friendship, null);
    }

    // Accept a friend request.
    @PostMapping("/{friendshipId}/accept")
    public StandardResponse acceptFriendRequest(@PathVariable UUID friendshipId,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        Friendship friendship = friendService.acceptFriendRequest(friendshipId, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Friend request accepted", friendship, null);
    }

    // Decline a friend request.
    @PostMapping("/{friendshipId}/decline")
    public StandardResponse declineFriendRequest(@PathVariable UUID friendshipId,
            @AuthenticationPrincipal UserDetailsImpl currentUser) {
        Friendship friendship = friendService.declineFriendRequest(friendshipId, currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Friend request declined", friendship, null);
    }

    // Get list of accepted friends.
    @GetMapping
    public StandardResponse getFriends(@AuthenticationPrincipal UserDetailsImpl currentUser) {
        List<User> friends = friendService.getFriends(currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Friends retrieved", friends, null);
    }

    // Get list of pending friend requests.
    @GetMapping("/requests")
    public StandardResponse getPendingRequests(@AuthenticationPrincipal UserDetailsImpl currentUser) {
        List<Friendship> pendingRequests = friendService.getPendingRequests(currentUser.getUser());
        return new StandardResponse(HttpStatus.OK.value(), "Pending friend requests retrieved", pendingRequests, null);
    }
}
