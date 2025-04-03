package net.ethlny.discordhetic.discord_backend_hetic.handlers;

import jakarta.servlet.http.HttpServletRequest;
import org.springframework.core.MethodParameter;
import org.springframework.http.MediaType;
import org.springframework.http.server.ServerHttpRequest;
import org.springframework.http.server.ServerHttpResponse;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.servlet.mvc.method.annotation.ResponseBodyAdvice;
import net.ethlny.discordhetic.discord_backend_hetic.payload.StandardResponse;

@ControllerAdvice
public class GlobalResponseHandler implements ResponseBodyAdvice<Object> {

    @Override
    public boolean supports(MethodParameter returnType, Class converterType) {
        // Apply to all responses
        return true;
    }

    @Override
    public Object beforeBodyWrite(Object body, MethodParameter returnType, MediaType selectedContentType,
            Class selectedConverterType, ServerHttpRequest request, ServerHttpResponse response) {
        // If this is an actuator endpoint, skip wrapping
        String path = request.getURI().getPath();
        if (path.startsWith("/actuator") || path.startsWith("/swagger-ui") || path.startsWith("/v3/api-docs")) {
            return body;
        }
        // Otherwise, if the body is already a StandardResponse, return it as is
        if (body instanceof StandardResponse) {
            return body;
        }
        // Wrap the response into our StandardResponse format
        return new StandardResponse(200, "Success", body, null);
    }
}
