package net.ethlny.discordhetic.discord_backend_hetic.repositories;

import java.util.Optional;
import java.util.UUID;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import net.ethlny.discordhetic.discord_backend_hetic.models.User;

@Repository
public interface UserRepository extends JpaRepository<User, UUID> {
    Optional<User> findByUsername(String username);

    Optional<User> findByEmail(String email);

    Boolean existsByUsername(String username);
    
    Boolean existsByEmail(String email);
}
