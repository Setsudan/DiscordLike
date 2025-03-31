# Stage 1: Build the application using the Gradle wrapper on Java 21
FROM eclipse-temurin:21 as builder
WORKDIR /app
# Copy all project files to the container
COPY . .
# Ensure the Gradle wrapper is executable and build the app
RUN chmod +x gradlew
RUN ./gradlew clean bootJar --no-daemon

# Stage 2: Create the runtime image
FROM eclipse-temurin:21-jre
WORKDIR /app
# Copy the generated jar from the builder stage
COPY --from=builder /app/build/libs/*.jar app.jar
EXPOSE 8080
ENTRYPOINT ["java", "-jar", "app.jar"]
