package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.UserRepository;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import org.springframework.security.core.annotation.AuthenticationPrincipal;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

@RestController
@RequestMapping("/users")
public class UserController {

    private final UserRepository userRepository;

    public UserController(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    // Get the profile of the currently authenticated user.
    @GetMapping("/me")
    public StandardResponse getCurrentUser(@AuthenticationPrincipal UserDetailsImpl currentUser) {
        User user = currentUser.getUser();
        return new StandardResponse(HttpStatus.OK.value(), "User profile retrieved", user, null);
    }

    // Update the profile of the currently authenticated user.
    @PutMapping("/me")
    public StandardResponse updateCurrentUser(@AuthenticationPrincipal UserDetailsImpl currentUser,
            @RequestBody User updatedUser) {
        User existingUser = currentUser.getUser();
        // Update allowed fields
        existingUser.setEmail(updatedUser.getEmail());
        existingUser.setDisplayName(updatedUser.getDisplayName());
        existingUser.setAvatarUrl(updatedUser.getAvatarUrl());
        // Save changes
        User savedUser = userRepository.save(existingUser);
        return new StandardResponse(HttpStatus.OK.value(), "User profile updated", savedUser, null);
    }

    // Get all users.
    @GetMapping
    public StandardResponse getAllUsers() {
        List<User> users = userRepository.findAll();
        return new StandardResponse(HttpStatus.OK.value(), "Users retrieved", users, null);
    }

    // Get a user by their ID.
    @GetMapping("/{id}")
    public StandardResponse getUserById(@PathVariable("id") UUID id) {
        Optional<User> userOpt = userRepository.findById(id);
        if (userOpt.isPresent()) {
            return new StandardResponse(HttpStatus.OK.value(), "User found", userOpt.get(), null);
        } else {
            return new StandardResponse(HttpStatus.NOT_FOUND.value(), "User not found", null, null);
        }
    }
}
