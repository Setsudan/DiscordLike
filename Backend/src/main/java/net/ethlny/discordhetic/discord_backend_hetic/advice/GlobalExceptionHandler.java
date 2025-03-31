package net.ethlny.discordhetic.discord_backend_hetic.advice;

import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ExceptionHandler;

@ControllerAdvice
public class GlobalExceptionHandler {

    @ExceptionHandler(Exception.class)
    public ResponseEntity<StandardResponse> handleException(Exception ex) {
        StandardResponse resp = new StandardResponse(500, "Error: " + ex.getMessage(), null,
                ex.getClass().getSimpleName());
        return new ResponseEntity<>(resp, HttpStatus.INTERNAL_SERVER_ERROR);
    }
}
