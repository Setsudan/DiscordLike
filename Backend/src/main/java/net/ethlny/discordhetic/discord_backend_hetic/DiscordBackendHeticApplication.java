package net.ethlny.discordhetic.discord_backend_hetic;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;

@SpringBootApplication
@EnableJpaRepositories("net.ethlny.discordhetic.discord_backend_hetic.repositories")
public class DiscordBackendHeticApplication {
    public static void main(String[] args) {
        SpringApplication.run(DiscordBackendHeticApplication.class, args);
    }
}
