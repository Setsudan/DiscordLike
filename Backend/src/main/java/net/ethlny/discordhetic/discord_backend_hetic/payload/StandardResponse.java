package net.ethlny.discordhetic.discord_backend_hetic.payload;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class StandardResponse {
    private int status;
    private String message;
    private Object data;
    private Object error;
}
