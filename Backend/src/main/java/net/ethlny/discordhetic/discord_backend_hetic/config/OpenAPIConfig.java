package net.ethlny.discordhetic.discord_backend_hetic.config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.info.Info;

@Configuration
public class OpenAPIConfig {

    @Bean
    public OpenAPI customOpenAPI() {
        return new OpenAPI()
                .info(new Info()
                    .title("API Discord HETIC")
                    .version("1.0.0")
                    .description("Documentation de l'API pour l'application Discord HETIC"));
    }
}
