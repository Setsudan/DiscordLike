package net.ethlny.discordhetic.discord_backend_hetic.repositories;

import java.util.Optional;
import java.util.UUID;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import net.ethlny.discordhetic.discord_backend_hetic.models.Role;

@Repository
public interface RoleRepository extends JpaRepository<Role, UUID> {
    Optional<Role> findByName(String name);

    Boolean existsByName(String name);
}
