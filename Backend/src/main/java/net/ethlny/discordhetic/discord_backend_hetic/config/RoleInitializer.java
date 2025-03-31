package net.ethlny.discordhetic.discord_backend_hetic.config;

import net.ethlny.discordhetic.discord_backend_hetic.models.Role;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.RoleRepository;
import org.springframework.boot.CommandLineRunner;
import org.springframework.stereotype.Component;

@Component
public class RoleInitializer implements CommandLineRunner {

    private final RoleRepository roleRepository;

    public RoleInitializer(RoleRepository roleRepository) {
        this.roleRepository = roleRepository;
    }

    @Override
    public void run(String... args) throws Exception {
        createRoleIfNotFound("ADMIN");
        createRoleIfNotFound("MODERATOR");
        createRoleIfNotFound("USER");
    }

    private void createRoleIfNotFound(String roleName) {
        if (!roleRepository.existsByName(roleName)) {
            Role role = new Role(roleName);
            roleRepository.save(role);
        }
    }
}
