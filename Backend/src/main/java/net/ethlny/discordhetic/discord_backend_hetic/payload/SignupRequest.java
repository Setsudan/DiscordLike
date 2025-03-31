package net.ethlny.discordhetic.discord_backend_hetic.payload;

import java.util.Set;

import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class SignupRequest {
    private String username;
    private String email;
    private String password;
    // Optionnel : liste des rôles à attribuer
    private Set<String> roles;
}
