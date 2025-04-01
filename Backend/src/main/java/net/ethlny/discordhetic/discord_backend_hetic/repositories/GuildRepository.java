package net.ethlny.discordhetic.discord_backend_hetic.repositories;

import net.ethlny.discordhetic.discord_backend_hetic.models.Guild;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import java.util.UUID;

@Repository
public interface GuildRepository extends JpaRepository<Guild, UUID> {
    // You can add custom queries if needed.
}
