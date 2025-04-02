package net.ethlny.discordhetic.discord_backend_hetic.models;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import java.io.Serializable;
import java.util.Date;
import java.util.HashSet;
import java.util.Set;
import java.util.UUID;

import com.fasterxml.jackson.annotation.JsonIgnore;

@Setter
@Getter
@Entity
@Table(name = "users", uniqueConstraints = {
        @UniqueConstraint(columnNames = "username"),
        @UniqueConstraint(columnNames = "email")
})
public class User implements Serializable {

    private static final long serialVersionUID = 1L;

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(columnDefinition = "uuid", updatable = false)
    private UUID id;

    @Column(nullable = false)
    private String username;

    @Column(nullable = false)
    private String email;

    @JsonIgnore
    @Column(nullable = false)
    private String password;

    // Optional field for a display name (can be different from username)
    private String displayName;

    // URL or path to the user's avatar image
    private String avatarUrl;

    // Account creation timestamp
    @Temporal(TemporalType.TIMESTAMP)
    private Date createdAt;

    // Last login timestamp
    @Temporal(TemporalType.TIMESTAMP)
    private Date lastLogin;

    // A flag for user online status (could be updated via websocket events)
    private boolean online;

    // Key for account activation (if using email verification)
    private String activationKey;

    // Roles associated with the user (e.g., admin, moderator, member)
    @ManyToMany(fetch = FetchType.EAGER)
    @JoinTable(name = "user_roles", joinColumns = @JoinColumn(name = "user_id"), inverseJoinColumns = @JoinColumn(name = "role_id"))
    private Set<Role> roles = new HashSet<>();

    public User() {
        this.createdAt = new Date();
        this.online = false;
    }

    // Convenience constructor for creating new users
    public User(String username, String email, String password) {
        this();
        this.username = username;
        this.email = email;
        this.password = password;
    }
}
