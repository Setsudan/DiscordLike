package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.UserRepository;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;
import net.ethlny.discordhetic.discord_backend_hetic.services.avatar.AvatarService;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.security.core.annotation.AuthenticationPrincipal;

import java.io.IOException;

@RestController
@RequestMapping("/users/me/avatar")
public class UserAvatarController {

    private final AvatarService avatarService;
    private final UserRepository userRepository;

    @Autowired
    public UserAvatarController(AvatarService avatarService, UserRepository userRepository) {
        this.avatarService = avatarService;
        this.userRepository = userRepository;
    }

    @PostMapping(consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    public StandardResponse uploadAvatar(@AuthenticationPrincipal UserDetailsImpl currentUser,
            @RequestParam("file") MultipartFile file) {
        // Validate file type
        String contentType = file.getContentType();
        if (contentType == null ||
                !(contentType.equals("image/jpeg") ||
                        contentType.equals("image/png") ||
                        contentType.equals("image/gif"))) {
            return new StandardResponse(HttpStatus.BAD_REQUEST.value(),
                    "Invalid file type. Only JPEG, PNG, and GIF images are allowed.",
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
