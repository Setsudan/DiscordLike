package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.UserRepository;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;

@RestController
@RequestMapping("/users")
@Tag(name = "Utilisateurs", description = "Gestion des utilisateurs")
public class UserController {

    private final UserRepository userRepository;

    public UserController(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    // Get the profile of the currently authenticated user.
    @Operation(
    summary = "Obtenir le profil de l'utilisateur connecté",
    description = "Renvoie les informations du compte actuellement authentifié.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Profil utilisateur récupéré",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "401", description = "Non authentifié"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @GetMapping("/me")
    public StandardResponse getCurrentUser(@AuthenticationPrincipal UserDetailsImpl currentUser) {
        User user = currentUser.getUser();
        return new StandardResponse(HttpStatus.OK.value(), "User profile retrieved", user, null);
    }

    // Update the profile of the currently authenticated user.
    @Operation(
    summary = "Mettre à jour le profil utilisateur",
    description = "Modifie certaines informations du compte de l'utilisateur connecté.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Profil mis à jour",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "400", description = "Requête invalide"),
        @ApiResponse(responseCode = "401", description = "Non authentifié"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
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
    @Operation(
    summary = "Lister tous les utilisateurs",
    description = "Renvoie la liste complète des utilisateurs inscrits.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Utilisateurs récupérés",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
    @GetMapping
    public StandardResponse getAllUsers() {
        List<User> users = userRepository.findAll();
        return new StandardResponse(HttpStatus.OK.value(), "Users retrieved", users, null);
    }

    // Get a user by their ID.
    @Operation(
    summary = "Récupérer un utilisateur par ID",
    description = "Renvoie les informations d’un utilisateur à partir de son UUID.",
    responses = {
        @ApiResponse(responseCode = "200", description = "Utilisateur trouvé",
            content = @Content(schema = @Schema(implementation = StandardResponse.class))),
        @ApiResponse(responseCode = "404", description = "Utilisateur introuvable"),
        @ApiResponse(responseCode = "500", description = "Erreur serveur")
        }
    )
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
