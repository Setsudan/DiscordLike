package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import java.io.IOException;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.multipart.MultipartFile;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.parameters.RequestBody;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.UserRepository;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.avatar.AvatarService;

@RestController
@RequestMapping("/users/me/avatar")
@Tag(name = "Avatar", description = "Upload d'avatar pour l'utilisateur connecté")
public class UserAvatarController {

    private final AvatarService avatarService;
    private final UserRepository userRepository;

    @Autowired
    public UserAvatarController(AvatarService avatarService, UserRepository userRepository) {
        this.avatarService = avatarService;
        this.userRepository = userRepository;
    }

    @PostMapping(consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    @Operation(summary = "Uploader un avatar", description = "Permet à l'utilisateur actuellement connecté d'uploader une image comme avatar (format image uniquement, max 500MB).", requestBody = @RequestBody(required = true, content = @Content(mediaType = MediaType.MULTIPART_FORM_DATA_VALUE)), responses = {
            @ApiResponse(responseCode = "200", description = "Avatar uploadé avec succès", content = @Content(schema = @Schema(implementation = StandardResponse.class))),
            @ApiResponse(responseCode = "400", description = "Fichier invalide ou trop volumineux", content = @Content(schema = @Schema(implementation = StandardResponse.class))),
            @ApiResponse(responseCode = "500", description = "Erreur lors de l'upload", content = @Content(schema = @Schema(implementation = StandardResponse.class)))
    })
    public StandardResponse uploadAvatar(@AuthenticationPrincipal UserDetailsImpl currentUser,
            @RequestParam("file") MultipartFile file) {
        // Validate file size
        if (file.getSize() > 1500 * 1024 * 1024) { // 1500MB
            return new StandardResponse(HttpStatus.BAD_REQUEST.value(),
                    "File size exceeds the maximum allowed size of 500MB.",
                    null, null);
        }

        // Validate file type
        String contentType = file.getContentType();
        if (contentType == null || !contentType.startsWith("image/")) {
            return new StandardResponse(HttpStatus.BAD_REQUEST.value(),
                    "Invalid file type. Only web-compatible image types are allowed.",
                    null, null);
        }

        try {
            String avatarUrl = avatarService.uploadAvatar(file);
            // Update the user's avatarUrl field
            User user = currentUser.getUser();
            user.setAvatarUrl(avatarUrl);
            userRepository.save(user);
            return new StandardResponse(HttpStatus.OK.value(), "Avatar uploaded successfully", avatarUrl, null);
        } catch (IOException e) {
            return new StandardResponse(HttpStatus.INTERNAL_SERVER_ERROR.value(), "Error uploading avatar", null,
                    e.getMessage());
        }
    }
}
