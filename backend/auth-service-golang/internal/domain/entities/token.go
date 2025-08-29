package entities

import (
	"database/sql/driver"
	"fmt"
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
	RevokedAt *time.Time `json:"revoked_at,omitempty" db:"revoked_at"`
	DeletedAt *time.Time `json:"deleted_at,omitempty" db:"deleted_at"`
}

func (token *Token) IsExpired() bool {
	return time.Now().After(token.ExpiryAt)
}

func (token *Token) IsRevoked() bool {
	return token.RevokedAt != nil
}

func (token *Token) IsDeleted() bool {
	return token.DeletedAt != nil
}

func (token *Token) IsValid() bool {
	return !token.IsExpired() && !token.IsRevoked() && !token.IsDeleted()
}

func (token *Token) Revoke() {
	if !token.IsRevoked() {
		now := time.Now()
		token.RevokedAt = &now
	}
}

func (token *Token) Delete() {
	if !token.IsDeleted() {
		now := time.Now()
		token.DeletedAt = &now
	}
}

func (tt TokenType) Value() (driver.Value, error) {
	return string(tt), nil
}

func (tt *TokenType) Scan(value any) error {
	if value == nil {
		return nil
	}

	switch s := value.(type) {
	case string:
		*tt = TokenType(s)
		return nil
	case []byte:
		*tt = TokenType(s)
		return nil
	default:
		return fmt.Errorf("cannot scan %T into TokenType", value)
	}
}

type TokenCreationParams struct {
	UserID uuid.UUID
	Type   TokenType
	TTL    time.Duration
}

func NewToken(params TokenCreationParams) *Token {
	now := time.Now()

	return &Token{
		ID:        uuid.New(),
		UserID:    params.UserID,
		Token:     generateSecureToken(),
		Type:      params.Type,
		ExpiryAt:  now.Add(params.TTL),
		CreatedAt: now,
		RevokedAt: nil,
		DeletedAt: nil,
	}
}

func generateSecureToken() string {
	//TODO
	return ""
}
