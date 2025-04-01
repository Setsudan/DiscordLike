package net.ethlny.discordhetic.discord_backend_hetic.services.guild;

import net.ethlny.discordhetic.discord_backend_hetic.models.Guild;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface GuildService {
    Guild createGuild(Guild guild);

    Optional<Guild> getGuildById(UUID guildId);

    List<Guild> getAllGuilds();

    Guild updateGuild(Guild guild, User currentUser);

    void deleteGuild(UUID guildId, User currentUser);

    Guild joinGuild(UUID guildId, User user);

    void leaveGuild(UUID guildId, User user);
}
