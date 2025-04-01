package net.ethlny.discordhetic.discord_backend_hetic.services.guild;

import net.ethlny.discordhetic.discord_backend_hetic.exception.UnauthorizedOperationException;
import net.ethlny.discordhetic.discord_backend_hetic.models.Guild;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.GuildRepository;
import org.springframework.stereotype.Service;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Service
public class GuildServiceImpl implements GuildService {

    private final GuildRepository guildRepository;

    public GuildServiceImpl(GuildRepository guildRepository) {
        this.guildRepository = guildRepository;
    }

    @Override
    public Guild createGuild(Guild guild) {
        guild.setCreatedAt(LocalDateTime.now());
        // Ensure the owner is added to the members list.
        guild.getMembers().add(guild.getOwner());
        return guildRepository.save(guild);
    }

    @Override
    public Optional<Guild> getGuildById(UUID guildId) {
        return guildRepository.findById(guildId);
    }

    @Override
    public List<Guild> getAllGuilds() {
        return guildRepository.findAll();
    }

    @Override
    public Guild updateGuild(Guild guild, User currentUser) {
        Guild existingGuild = guildRepository.findById(guild.getId())
                .orElseThrow(() -> new IllegalArgumentException("Guild not found"));
        // Only the guild owner can update the guild.
        if (!existingGuild.getOwner().getId().equals(currentUser.getId())) {
            throw new UnauthorizedOperationException("You are not authorized to update this guild");
        }
        existingGuild.setName(guild.getName());
        existingGuild.setDescription(guild.getDescription());
        existingGuild.setIconUrl(guild.getIconUrl());
        return guildRepository.save(existingGuild);
    }

    @Override
    public void deleteGuild(UUID guildId, User currentUser) {
        Guild guild = guildRepository.findById(guildId)
                .orElseThrow(() -> new IllegalArgumentException("Guild not found"));
        // Only the owner can delete the guild.
        if (!guild.getOwner().getId().equals(currentUser.getId())) {
            throw new UnauthorizedOperationException("You are not authorized to delete this guild");
        }
        guildRepository.deleteById(guildId);
    }

    @Override
    public Guild joinGuild(UUID guildId, User user) {
        Guild guild = guildRepository.findById(guildId)
                .orElseThrow(() -> new IllegalArgumentException("Guild not found"));
        guild.getMembers().add(user);
        return guildRepository.save(guild);
    }

    @Override
    public void leaveGuild(UUID guildId, User user) {
        Guild guild = guildRepository.findById(guildId)
                .orElseThrow(() -> new IllegalArgumentException("Guild not found"));
        // The owner cannot leave their guild unless they delete it.
        if (guild.getOwner().getId().equals(user.getId())) {
            throw new UnauthorizedOperationException(
                    "Guild owner cannot leave the guild. Consider deleting the guild instead.");
        }
        guild.getMembers().remove(user);
        guildRepository.save(guild);
    }
}
