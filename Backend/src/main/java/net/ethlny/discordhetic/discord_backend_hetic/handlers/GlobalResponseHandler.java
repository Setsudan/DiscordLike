package net.ethlny.discordhetic.discord_backend_hetic.handlers;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;
import org.springframework.core.MethodParameter;
import org.springframework.http.MediaType;
import org.springframework.http.converter.HttpMessageConverter;
import org.springframework.http.server.ServerHttpRequest;
import org.springframework.http.server.ServerHttpResponse;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.servlet.mvc.method.annotation.ResponseBodyAdvice;

@ControllerAdvice
public class GlobalResponseHandler implements ResponseBodyAdvice<Object> {

    private final ObjectMapper mapper = new ObjectMapper();

    @Override
    public boolean supports(MethodParameter returnType,
            Class<? extends HttpMessageConverter<?>> converterType) {
        // Apply this advice to all controller methods
        return true;
    }

    @Override
    public Object beforeBodyWrite(Object body,
            MethodParameter returnType,
            MediaType selectedContentType,
            Class<? extends HttpMessageConverter<?>> selectedConverterType,
            ServerHttpRequest request,
            ServerHttpResponse response) {
        // If already wrapped, return as is.
        if (body instanceof StandardResponse) {
            return body;
        }
        // Special handling if the controller returns a plain String.
        if (body instanceof String) {
            try {
                return mapper.writeValueAsString(new StandardResponse(200, "Success", body, null));
            } catch (JsonProcessingException e) {
                throw new RuntimeException("Error processing String response", e);
            }
        }
        // Otherwise, wrap the body in a StandardResponse with data populated.
        return new StandardResponse(200, "Success", body, null);
    }
}
