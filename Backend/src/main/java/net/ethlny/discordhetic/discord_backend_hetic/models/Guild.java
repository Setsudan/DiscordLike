package net.ethlny.discordhetic.discord_backend_hetic.models;

import jakarta.persistence.*;
import lombok.Data;
import lombok.NoArgsConstructor;
import java.time.LocalDateTime;
import java.util.HashSet;
import java.util.Set;
import java.util.UUID;

@Data
@NoArgsConstructor
@Entity
@Table(name = "guilds", uniqueConstraints = {
        @UniqueConstraint(columnNames = "name")
})
public class Guild {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    private UUID id;

    @Column(nullable = false)
    private String name;

    private String description;

    private String iconUrl;

    @ManyToOne(fetch = FetchType.EAGER)
    @JoinColumn(name = "owner_id", nullable = false)
    private User owner;

    // Members of the guild; the owner is automatically added to this set.
    @ManyToMany(fetch = FetchType.LAZY)
    @JoinTable(name = "guild_members", joinColumns = @JoinColumn(name = "guild_id"), inverseJoinColumns = @JoinColumn(name = "user_id"))
    private Set<User> members = new HashSet<>();

    @Column(nullable = false)
    private LocalDateTime createdAt;

    public Guild(String name, String description, String iconUrl, User owner) {
        this.name = name;
        this.description = description;
        this.iconUrl = iconUrl;
        this.owner = owner;
        this.createdAt = LocalDateTime.now();
        this.members.add(owner);
    }
}
