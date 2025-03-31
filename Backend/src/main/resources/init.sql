-- Créer la table roles si elle n'existe pas
CREATE TABLE IF NOT EXISTS roles (
  id uuid DEFAULT gen_random_uuid() PRIMARY KEY,
  name VARCHAR(255) NOT NULL UNIQUE,
  description VARCHAR(255)
);

-- Créer la table users si elle n'existe pas
CREATE TABLE IF NOT EXISTS users (
  id uuid DEFAULT gen_random_uuid() PRIMARY KEY,
  username VARCHAR(255) NOT NULL,
  email VARCHAR(255) NOT NULL,
  password VARCHAR(255) NOT NULL,
  display_name VARCHAR(255),
  avatar_url VARCHAR(255),
  created_at TIMESTAMP,
  last_login TIMESTAMP,
  online BOOLEAN,
  activation_key VARCHAR(255),
  CONSTRAINT users_username_key UNIQUE (username),
  CONSTRAINT users_email_key UNIQUE (email)
);

-- Créer la table d'association user_roles (ManyToMany)
CREATE TABLE IF NOT EXISTS user_roles (
  user_id uuid NOT NULL,
  role_id uuid NOT NULL,
  PRIMARY KEY (user_id, role_id),
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE
);

-- Insère le rôle USER s'il n'existe pas
INSERT INTO roles (name, description)
SELECT 'ROLE_USER', 'Rôle utilisateur'
WHERE NOT EXISTS (SELECT 1 FROM roles WHERE name = 'ROLE_USER');

-- Insère le rôle ADMIN s'il n'existe pas
INSERT INTO roles (name, description)
SELECT 'ROLE_ADMIN', 'Rôle administrateur'
WHERE NOT EXISTS (SELECT 1 FROM roles WHERE name = 'ROLE_ADMIN');
