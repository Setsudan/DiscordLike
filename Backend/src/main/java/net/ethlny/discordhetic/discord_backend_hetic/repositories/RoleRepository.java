package net.ethlny.discordhetic.discord_backend_hetic.repositories;

import net.ethlny.discordhetic.discord_backend_hetic.models.Role;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface RoleRepository extends JpaRepository<Role, UUID> {
    boolean existsByName(String name);

    Optional<Role> findByName(String name);
}
