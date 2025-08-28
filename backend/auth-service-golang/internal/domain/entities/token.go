package entities

import (
	"time"

	"github.com/google/uuid"
)

type TokenType string

const (
	TokenTypeAccess  TokenType = "access"
	TokenTypeRefresh TokenType = "refresh"
)

type Token struct {
	ID        uuid.UUID  `json:"id" db:"id" validate:"required"`
	UserID    uuid.UUID  `json:"user_id" db:"user_id" validate:"required"`
	Token     string     `json:"token" db:"token" validate:"required,min=32"`
	Type      TokenType  `json:"type" db:"type" validate:"required,oneof=access refresh"`
	ExpiryAt  time.Time  `json:"expiry_at" db:"expiry_at"`
	CreatedAt time.Time  `json:"created_at" db:"created_at"`
	UpdatedAt *time.Time `json:"updated_at,omitempty" db:"updated_at"`
	RevokedAt *time.Time `json:"revoked_at,omitempty" db:"revoked_at"`
	DeletedAt *time.Time `json:"deleted_at,omitempty" db:"deleted_at"`
}
