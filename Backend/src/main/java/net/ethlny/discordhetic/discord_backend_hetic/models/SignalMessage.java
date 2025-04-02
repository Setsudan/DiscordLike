package net.ethlny.discordhetic.discord_backend_hetic.models;

import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
public class SignalMessage {
    // "offer", "answer", "candidate"
    private String type;
    // SDP offer/answer when type is "offer" or "answer"
    private String sdp;
    // ICE candidate when type is "candidate"
    private String candidate;
    // Caller identifier
    private String callerId;
}
