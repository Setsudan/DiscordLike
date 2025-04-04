package net.ethlny.discordhetic.discord_backend_hetic.payload;

import java.time.LocalDateTime;
import java.util.UUID;

import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class MessageRequestDTO {
    private UUID sender;
    private String content;
    private LocalDateTime timestamp;
    private UUID channelId;
    private String attachmentUrl;
}
