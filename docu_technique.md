# Documentation Technique

## 1. Introduction

Ce projet est une application de type « chat à la Discord » qui se compose de deux parties principales :

- **Backend** : Une application serveur développée en Java (Spring Boot) qui fournit une API REST, des WebSockets pour la messagerie en temps réel et intègre plusieurs services externes (base de données PostgreSQL, stockage S3 via MinIO, monitoring avec Prometheus et Grafana, proxy inverse via Nginx, etc.).
- **Frontend** : Une application desktop basée sur WPF (Windows Presentation Foundation) en C#, qui offre une interface utilisateur pour la connexion, la messagerie et la gestion des utilisateurs.

Le projet est orchestré à l’aide de Docker Compose afin de faciliter le déploiement de l’ensemble des services.

---

## 2. Architecture Générale

### 2.1. Structure du Répertoire

Le projet est organisé de la manière suivante :

```
-discordlike/
├── postman.json                  // Collection Postman pour tester l’API
├── Backend/                      // Code du serveur
│   ├── docker-compose.yml        // Orchestration des conteneurs Docker
│   ├── Dockerfile                // Dockerfile pour builder et lancer l’application Java
│   ├── gradlew / gradlew.bat     // Scripts Gradle pour Unix/Windows
│   ├── .gitattributes/.gitignore // Fichiers de configuration Git
│   ├── cloudflared/              // Configuration du tunnel Cloudflared
│   ├── gradle/wrapper/           // Configuration du wrapper Gradle
│   ├── grafana/                  // Fichiers de provisioning pour Grafana
│   ├── nginx/                    // Fichier de configuration Nginx
│   ├── prometheus/               // Configuration de Prometheus
│   ├── src/                      // Code source Java
│   │   ├── main/                 // Code applicatif et ressources
│   │   └── test/                 // Tests unitaires et d’intégration
└── Frontend/                     // Code de l’application WPF (C#)
    ├── App.xaml, App.xaml.cs      // Fichiers d’application WPF
    ├── DiscordLikeChatApp.csproj  // Fichier projet
    ├── MainWindow.xaml, MainWindow.xaml.cs
    ├── Models/                   // Modèles de données (Utilisateur, Rôle, etc.)
    ├── Services/                 // Services pour communiquer avec l’API
    ├── Views/                    // Vues XAML (ex : LoginView)
    └── ViewsModels/              // ViewModels (ex : LoginViewModel)
```

### 2.2. Infrastructure et Déploiement

Le fichier `docker-compose.yml` définit plusieurs services :

- **app** : L’application Java (Spring Boot) exposée sur le port 8080.
- **db** : Une instance PostgreSQL (v15) pour la persistance.
- **prometheus** : Le service de monitoring Prometheus, accessible sur le port 9090.
- **grafana** : L’interface de visualisation Grafana, disponible sur le port 3000.
- **minio** : Service de stockage compatible S3 (MinIO) pour le stockage des avatars et autres fichiers.
- **nginx** : Proxy inverse qui redirige les requêtes vers les différents services (API, MinIO, Grafana, Prometheus).

Des volumes Docker sont utilisés pour la persistance des données de PostgreSQL, Grafana et MinIO.

---

## 3. Détails du Backend

### 3.1. Technologies et Frameworks

- **Java 21 & Spring Boot** : Fournit l’environnement d’exécution, la gestion des dépendances et la configuration automatique.
- **Gradle** : Outil de build et de gestion de projet, avec des scripts pour Unix (`gradlew`) et Windows (`gradlew.bat`).
- **Spring Data JPA (Hibernate)** : Permet la gestion de la persistance des données avec PostgreSQL.
- **Spring Security + JWT** : Sécurise l’API avec authentification par JSON Web Token.
- **WebSockets** : Utilisés pour le signalement des appels et la gestion de la présence en temps réel.
- **AWS SDK pour Java** : Intégré pour communiquer avec MinIO en tant que service de stockage S3.

### 3.2. Structure du Code Source

Le code source est organisé en plusieurs packages pour séparer les responsabilités :

- **config** : Contient la configuration OpenAPI, l’initialisation des rôles, la configuration S3 et la configuration WebSocket.
  - *OpenAPIConfig.java* : Configure la documentation API Swagger.
  - *RoleInitializer.java* : Initialise les rôles (ADMIN, MODERATOR, USER) à l’exécution.
  - *S3Config.java* : Configure le client AmazonS3 pour interagir avec MinIO.
  - *WebSocketConfig.java* : Définit le point d’accès et les préfixes pour les messages WebSocket.
  
- **controllers** : Définit les points de terminaison REST pour les différentes fonctionnalités.
  - **AuthController.java** : Gère la connexion (`/auth/signin`) et l’inscription (`/auth/signup`).
  - **UserController.java** : Gestion du profil utilisateur et de la récupération des utilisateurs.
  - **ChannelController.java** : Opérations CRUD sur les canaux (channels).
  - **MessageController.java** et **ChannelMessagingController.java** : Envoi et récupération des messages.
  - **DirectMessageController.java** : Gestion des messages privés.
  - **FriendController.java** : Envoi, acceptation, refus des demandes d’amitié.
  - **GuildController.java** : Gestion des serveurs/guilds.
  - **NotificationController.java** : Envoi et récupération des notifications.
  - **PresenceController.java** : Récupération de la liste des utilisateurs en ligne.
  - **UserAvatarController.java** : Upload d’avatars pour les utilisateurs.
  - **CallSignalingController.java** : Transfert de messages de signalisation pour la gestion des appels.

- **exception** : Contient des classes d’exception personnalisées (ex. *UnauthorizedOperationException.java*).

- **handlers** :
  - *GlobalExceptionHandler.java* : Capture les exceptions globales et formate la réponse.
  - *GlobalResponseHandler.java* : Intercepte les réponses pour les envelopper dans un format standardisé.

- **listeners** :
  - *WebSocketPresenceEventListener.java* : Gère les événements de connexion/déconnexion des WebSockets pour mettre à jour la présence.

- **models** : Définit les entités persistantes et les objets métiers (User, Channel, Guild, Message, DirectMessage, Notification, Role, Friendship, etc.) ainsi que des classes pour la signalisation des appels.
  
- **payload** : Contient les classes pour le transfert de données via l’API (LoginRequest, SignupRequest, JwtResponse, StandardResponse, MessageRequestDTO, etc.).

- **repositories** : Interfaces Spring Data JPA pour accéder aux données (UserRepository, ChannelRepository, MessageRepository, etc.).

- **security** et **security.jwt** : Gèrent la sécurité de l’application.
  - *SecurityConfig.java* : Configure la sécurité HTTP, les autorisations et les filtres.
  - *JwtAuthenticationFilter.java*, *JwtTokenProvider.java*, *CustomUserDetailsService.java* : Gèrent l’authentification par JWT.
  - *AuthEntryPointJwt.java* : Gère les erreurs d’authentification.

- **services** : Logique métier et services d’accès aux données.
  - Sous-package *avatar* : Service d’upload d’avatar (AvatarService.java).
  - Sous-package *channel*, *direct*, *friend*, *guild*, *messaging*, *notification*, *presence* et *User* : Implémentent la logique métier pour chaque fonctionnalité.

### 3.3. Points d’Entrée de l’API

#### Authentification et Gestion des Utilisateurs

- **/auth/signin** : Authentifie l’utilisateur et retourne un token JWT.
- **/auth/signup** : Inscrit un nouvel utilisateur et retourne un token JWT en cas de succès.
- **/users/me** : Permet de récupérer ou de mettre à jour le profil de l’utilisateur authentifié.
- **/users/{id}** et **/users** : Accès aux informations d’un utilisateur spécifique ou à la liste complète des utilisateurs.

#### Gestion des Canaux et de la Messagerie

- **/channels** : CRUD pour les canaux (création, récupération, mise à jour, suppression). Seuls les créateurs peuvent modifier ou supprimer leur canal.
- **/messages** et **/channels/{channelId}** : Envoi et récupération des messages dans un canal.
- **/direct-messages** : Gestion des messages privés (envoi et récupération de conversations).

#### Amis et Guilds

- **/friends** : Envoi, acceptation, refus et consultation des demandes d’amitié.
- **/guilds** : Création, modification, suppression, adhésion et départ des guilds (serveurs). Seul le propriétaire peut mettre à jour ou supprimer une guild.

#### Notifications et Présence

- **/notifications** : Envoi et récupération des notifications utilisateur, avec envoi en temps réel via WebSocket.
- **/presence/online** : Récupère la liste des utilisateurs actuellement en ligne. La présence est mise à jour grâce aux événements WebSocket.

#### Upload d’Avatar

- **/users/me/avatar** : Permet à l’utilisateur authentifié de téléverser un avatar. Le service vérifie le type et la taille du fichier avant de l’uploader sur le service MinIO.

#### Communication en Temps Réel

- **WebSocket** :  
  - Les endpoints WebSocket (ex. `/ws`) sont configurés pour gérer le signalement des appels (offres, réponses, candidats ICE) via le *CallSignalingController*.
  - Un listener (*WebSocketPresenceEventListener*) écoute les événements de connexion/déconnexion pour mettre à jour la présence.

---

## 4. Configuration et Déploiement

### 4.1. Fichiers de Configuration

- **application.properties** :  
  - Définit le nom de l’application, les configurations de la base de données, la sécurité (clé secrète JWT, durée d’expiration), les configurations pour S3/MinIO, et la taille maximale des fichiers uploadés.

- **init.sql** :  
  - Script SQL pour initialiser la base de données (création des tables *roles*, *users*, et la table d’association *user_roles*), avec insertion initiale des rôles.

- **logback-spring.xml** :  
  - Configuration du logging (console et fichier) avec gestion de la rotation des fichiers de log.

### 4.2. Fichiers de Build et Scripts

- **Dockerfile** :  
  - Multi-stage build pour compiler l’application avec Gradle puis créer une image d’exécution basée sur Java 21 JRE.
- **docker-compose.yml** :  
  - Définit et orchestre les conteneurs pour l’application, la base de données, Prometheus, Grafana, MinIO et Nginx.
- **Scripts Gradle** (*gradlew*, *gradlew.bat*) et *gradle-wrapper.properties* :  
  - Permettent de gérer le build de l’application de manière indépendante de l’environnement.

### 4.3. Autres Configurations

- **cloudflared/config.yml** :  
  - Configuration du tunnel Cloudflared pour rediriger le trafic vers le service Nginx.
- **Fichiers de provisioning pour Grafana** :  
  - Configurent la source de données pour Prometheus via Grafana.

---

## 5 Conclusion

Ce projet est une application complète de chat inspirée de Discord, intégrant des technologies modernes pour la création d’API REST sécurisées, la communication en temps réel via WebSockets, et la gestion de la persistance des données avec une base de données relationnelle. Le frontend WPF, conçu selon le pattern MVVM, offre une interface utilisateur desktop fonctionnelle. L’utilisation de Docker Compose permet de déployer facilement l’ensemble des services nécessaires, tandis que les outils de monitoring et de stockage assurent la robustesse et la scalabilité du système.
