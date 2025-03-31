package net.ethlny.discordhetic.discord_backend_hetic.security.jwt;

import io.jsonwebtoken.ExpiredJwtException;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.MalformedJwtException;
import io.jsonwebtoken.SignatureAlgorithm;
import io.jsonwebtoken.UnsupportedJwtException;
import io.jsonwebtoken.lang.Strings;
import io.jsonwebtoken.security.Keys;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;

import java.security.SecureRandom;
import java.util.Base64;
import java.util.Date;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Component;

/**
 * This class provide methods to encrypt JWT before sending it back
 */
@Component
public class JwtTokenProvider {

    private static final Logger LOGGER = LoggerFactory.getLogger(JwtTokenProvider.class);

    @Value("${wework.app.jwtSecret}")
    private String jwtSecret;

    @Value("${wework.app.jwtExpirationMs}")
    private int jwtExpirationMs;

    /**
     * This method allows to generate JWT from the authentication object
     * 
     * @param authentication the authentication object
     * @return A JWT generated.
     */
    public String generateJwtToken(Authentication authentication) {

        UserDetailsImpl userPrincipal = (UserDetailsImpl) authentication.getPrincipal();

        return Jwts.builder()
                .setSubject(userPrincipal.getUsername())
                .setIssuedAt(new Date())
                .setExpiration(new Date((new Date()).getTime() + jwtExpirationMs))
                .signWith(Keys.hmacShaKeyFor(generateSecretKey().getBytes()))
                .compact();
    }

    public String getUserNameFromJwtToken(String token) {
        return Jwts.parserBuilder().setSigningKey(Keys.hmacShaKeyFor(this.jwtSecret.getBytes())).build()
                .parseClaimsJws(token).getBody().getSubject();
    }

    public boolean validateJwtToken(String authToken) {
        try {
            Jwts.parserBuilder().setSigningKey(this.jwtSecret.getBytes()).build().parseClaimsJws(authToken);
            return true;
        } catch (MalformedJwtException e) {
            LOGGER.error("Invalid JWT token: {}", e.getMessage());
        } catch (ExpiredJwtException e) {
            LOGGER.error("JWT token is expired: {}", e.getMessage());
        } catch (UnsupportedJwtException e) {
            LOGGER.error("JWT token is unsupported: {}", e.getMessage());
        } catch (IllegalArgumentException e) {
            LOGGER.error("JWT claims string is empty: {}", e.getMessage());
        }

        return false;
    }

    private String generateSecretKey() {

        if (Strings.hasText(this.jwtSecret)) {
            return this.jwtSecret;
        } else {
            // length means (32 bytes are required for 256-bit key)
            int length = 64;

            // Create a secure random generator
            SecureRandom secureRandom = new SecureRandom();

            // Create a byte array to hold the random bytes
            byte[] keyBytes = new byte[length];

            // Generate the random bytes
            secureRandom.nextBytes(keyBytes);

            this.jwtSecret = Base64.getEncoder().encodeToString(keyBytes);

            // Encode the key in Base64 format for easier storage and usage
            return this.jwtSecret;
        }
    }
}