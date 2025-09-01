CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TYPE token_type AS ENUM ('access', 'refresh');

CREATE TABLE tokens (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token TEXT NOT NULL,
    type token_type NOT NULL,
    expiry_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    deleted_at TIMESTAMP WITH TIME ZONE DEFAULT NULL
);

CREATE INDEX idx_tokens_user_id ON tokens(user_id);
CREATE INDEX idx_tokens_token ON tokens(token);
CREATE INDEX idx_tokens_deleted_at ON tokens(deleted_at);
CREATE INDEX idx_tokens_user_type_active ON tokens(user_id, type) WHERE deleted_at IS NULL;