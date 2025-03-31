package net.ethlny.discordhetic.discord_backend_hetic.controllers;

import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import net.ethlny.discordhetic.discord_backend_hetic.models.Role;
import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.payload.JwtResponse;
import net.ethlny.discordhetic.discord_backend_hetic.payload.LoginRequest;
import net.ethlny.discordhetic.discord_backend_hetic.payload.MessageResponse;
import net.ethlny.discordhetic.discord_backend_hetic.payload.SignupRequest;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.RoleRepository;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.UserRepository;
import net.ethlny.discordhetic.discord_backend_hetic.security.jwt.JwtTokenProvider;
import net.ethlny.discordhetic.discord_backend_hetic.services.User.UserDetailsImpl;

@RestController
@RequestMapping("/api/auth")
public class AuthController {

    @Autowired
    AuthenticationManager authenticationManager;
    
    @Autowired
    UserRepository userRepository;
    
    @Autowired
    RoleRepository roleRepository;
    
    @Autowired
    PasswordEncoder encoder;
    
    @Autowired
    JwtTokenProvider jwtTokenProvider;
    
    // Endpoint de connexion
    @PostMapping("/signin")
    public ResponseEntity<?> authenticateUser(@RequestBody LoginRequest loginRequest) {
    
        Authentication authentication = authenticationManager.authenticate(
                new UsernamePasswordAuthenticationToken(loginRequest.getUsername(), loginRequest.getPassword())
        );
    
        SecurityContextHolder.getContext().setAuthentication(authentication);
    
        String jwt = jwtTokenProvider.generateJwtToken(authentication);
    
        UserDetailsImpl userDetails = (UserDetailsImpl) authentication.getPrincipal();
        List<String> roles = userDetails.getAuthorities()
                                        .stream()
                                        .map(item -> item.getAuthority())
                                        .collect(Collectors.toList());
    
        return ResponseEntity.ok(new JwtResponse(jwt, userDetails.getId(), userDetails.getUsername(), userDetails.getEmail(), roles));
    }
    
    // Endpoint d'inscription
    @PostMapping("/signup")
    public ResponseEntity<?> registerUser(@RequestBody SignupRequest signUpRequest) {
        if (userRepository.existsByUsername(signUpRequest.getUsername())) {
            return ResponseEntity.badRequest().body(new MessageResponse("Error: Username is already taken!"));
        }
    
        if (userRepository.existsByEmail(signUpRequest.getEmail())) {
            return ResponseEntity.badRequest().body(new MessageResponse("Error: Email is already in use!"));
        }
    
        User user = new User(signUpRequest.getUsername(),
                             signUpRequest.getEmail(),
                             encoder.encode(signUpRequest.getPassword()));
    
        Set<String> strRoles = signUpRequest.getRoles();
        Set<Role> roles = new HashSet<>();
    
        if (strRoles == null || strRoles.isEmpty()) {
            Role userRole = roleRepository.findByName("ROLE_USER")
                    .orElseThrow(() -> new RuntimeException("Error: Role USER not found."));
            roles.add(userRole);
        } else {
            strRoles.forEach(role -> {
                switch (role) {
                    case "admin":
                        Role adminRole = roleRepository.findByName("ROLE_ADMIN")
                                .orElseThrow(() -> new RuntimeException("Error: Role ADMIN not found."));
                        roles.add(adminRole);
                        break;
                    default:
                        Role userRole = roleRepository.findByName("ROLE_USER")
                                .orElseThrow(() -> new RuntimeException("Error: Role USER not found."));
                        roles.add(userRole);
                }
            });
        }
    
        user.setRoles(roles);
        userRepository.save(user);
    
        return ResponseEntity.ok(new MessageResponse("User registered successfully!"));
    }
}
