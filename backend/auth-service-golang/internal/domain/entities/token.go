package entities

import (
	"auth-service/internal/errors/apperrors"
	"database/sql/driver"
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type TokenType string

const (
	TokenTypeAccess  TokenType = "access"
	TokenTypeRefresh TokenType = "refresh"
)

type Token struct {
	ID        uuid.UUID      `json:"id" validate:"required" gorm:"primaryKey;default:uuid_generate_v4()"`
	UserID    uuid.UUID      `json:"user_id" validate:"required" gorm:"index"`
	Token     string         `json:"token" validate:"required,min=32" gorm:"index;type:text"`
	Type      TokenType      `json:"type" validate:"required,oneof=access refresh"`
	ExpiryAt  time.Time      `json:"expiry_at"`
	CreatedAt time.Time      `json:"created_at" gorm:"default:CURRENT_TIMESTAMP"`
	DeletedAt gorm.DeletedAt `json:"-"`
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
		return apperrors.ErrScanValue
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
	}
}

func generateSecureToken() string {
	//TODO
	return ""
}
