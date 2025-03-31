package net.ethlny.discordhetic.discord_backend_hetic.payload;

import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class LoginRequest {
    private String username;
    private String password;
}
