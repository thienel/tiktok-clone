-- Create UUID extension if not exists
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create user_status enum to match Go constants
CREATE TYPE user_status AS ENUM ('active', 'inactive', 'suspended', 'pending');

-- Create users table
CREATE TABLE users (
                       id UUID PRIMARY KEY,  -- No default, GORM handles this
                       username VARCHAR(24) UNIQUE NOT NULL,  -- Match Go validation max=24
                       email VARCHAR(100) UNIQUE NOT NULL,    -- Keep reasonable limit for email
                       password_hash VARCHAR(255) NOT NULL,   -- BCrypt hashes are typically 60 chars, but 255 for safety
                       status user_status NOT NULL DEFAULT 'pending',  -- Default matches Go NewUser function
                       created_at TIMESTAMP WITH TIME ZONE NOT NULL,   -- GORM handles this, no default needed
                       updated_at TIMESTAMP WITH TIME ZONE NOT NULL,   -- GORM handles this, no default needed
                       deleted_at TIMESTAMP WITH TIME ZONE DEFAULT NULL
);

CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_status ON users(status);
CREATE INDEX idx_users_deleted_at ON users(deleted_at);