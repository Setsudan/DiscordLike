package net.ethlny.discordhetic.discord_backend_hetic.models;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;
import java.time.LocalDateTime;
import java.util.UUID;

@Data
@NoArgsConstructor
@Entity
@Table(name = "notifications")
public class Notification {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    private UUID id;

    @ManyToOne(fetch = FetchType.EAGER)
    @JoinColumn(name = "user_id", nullable = false)
    private User user;

    @Column(nullable = false, length = 1000)
    private String message;

    // You can use a dedicated type or just a string to indicate the kind of
    // notification.
    private String type;

    @Column(nullable = false)
    private LocalDateTime createdAt;

    public Notification(User user, String message, String type) {
        this.user = user;
        this.message = message;
        this.type = type;
        this.createdAt = LocalDateTime.now();
    }
}
