package net.ethlny.discordhetic.discord_backend_hetic.services.User;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.stereotype.Service;

import net.ethlny.discordhetic.discord_backend_hetic.models.User;
import net.ethlny.discordhetic.discord_backend_hetic.repositories.UserRepository;


@Service
public class UserDetailsServiceImpl implements UserDetailsService {

    @Autowired
    UserRepository userRepository;

    @Override
    public UserDetails loadUserByUsername(String usernameOrEmail) throws UsernameNotFoundException {
        User user = userRepository.findByUsername(usernameOrEmail)
                .orElseThrow(() -> new UsernameNotFoundException("User not found with username: " + usernameOrEmail));

        return UserDetailsImpl.build(user);
    }
}
