package net.ethlny.discordhetic.discord_backend_hetic.models;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;
import java.time.LocalDateTime;
import java.util.UUID;

@Data
@NoArgsConstructor
@Entity
@Table(name = "channels", uniqueConstraints = {
        @UniqueConstraint(columnNames = "name")
})
public class Channel {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    private UUID id;

    @Column(nullable = false)
    private String name;

    private String description;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private ChannelType type;

    @ManyToOne(fetch = FetchType.EAGER)
    @JoinColumn(name = "created_by_id", nullable = false)
    private User createdBy;

    @Column(nullable = false)
    private LocalDateTime createdAt;

    // Convenience constructor for creation
    public Channel(String name, String description, ChannelType type, User createdBy) {
        this.name = name;
        this.description = description;
        this.type = type;
        this.createdBy = createdBy;
        this.createdAt = LocalDateTime.now();
    }
}
