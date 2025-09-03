package entities

import (
	"auth-service/internal/errors/apperrors"
	"database/sql/driver"
	"os"
	"time"

	"github.com/golang-jwt/jwt/v5"
	"github.com/google/uuid"
	"gorm.io/gorm"
)

type CustomClaims struct {
	Username  string    `json:"username"`
	TokenType TokenType `json:"token_type"`
	jwt.RegisteredClaims
}

type TokenType string

const (
	TokenTypeAccess  TokenType = "access"
	TokenTypeRefresh TokenType = "refresh"
)

type Token struct {
	ID        uuid.UUID      `json:"id" validate:"required" gorm:"primaryKey"`
	UserID    uuid.UUID      `json:"user_id" validate:"required" gorm:"index"`
	Token     string         `json:"token" validate:"required,min=32" gorm:"index;type:text"`
	Type      TokenType      `json:"type" validate:"required,oneof=access refresh"`
	ExpiryAt  time.Time      `json:"expiry_at"`
	CreatedAt time.Time      `json:"created_at" gorm:"autoCreateTime"`
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

func NewToken(params TokenCreationParams) (*Token, error) {
	userID := params.UserID
	tokenType := params.Type
	expiryAt := time.Now().UTC().Add(params.TTL)

	token, err := generateJWTWithRSA(userID, tokenType, expiryAt)
	if err != nil {
		return nil, err
	}

	return &Token{
		ID:       uuid.New(),
		UserID:   userID,
		Token:    token,
		Type:     tokenType,
		ExpiryAt: expiryAt,
	}, nil
}

func generateJWTWithRSA(userID uuid.UUID, tokenType TokenType, expiryAt time.Time) (string, error) {
	now := time.Now().UTC()
	claims := CustomClaims{
		userID.String(),
		tokenType,
		jwt.RegisteredClaims{
			ID:        uuid.New().String(),
			IssuedAt:  jwt.NewNumericDate(now),
			NotBefore: jwt.NewNumericDate(now),
			ExpiresAt: jwt.NewNumericDate(expiryAt),
			Issuer:    "auth-service",
			Subject:   userID.String(),
		},
	}

	privateKeyData, err := os.ReadFile(os.Getenv("PRIVATE_KEY_PATH"))
	if err != nil {
		return "", err
	}

	privateKey, err := jwt.ParseRSAPrivateKeyFromPEM(privateKeyData)
	if err != nil {
		return "", err
	}

	token := jwt.NewWithClaims(jwt.SigningMethodRS256, claims)
	return token.SignedString(privateKey)
}

func VerifyJWT(tokenStr string) (*CustomClaims, error) {
	publicKeyData, err := os.ReadFile(os.Getenv("PUBLIC_KEY_PATH"))
	if err != nil {
		return nil, err
	}

	publicKey, err := jwt.ParseRSAPublicKeyFromPEM(publicKeyData)
	if err != nil {
		return nil, err
	}

	token, err := jwt.ParseWithClaims(tokenStr, &CustomClaims{}, func(token *jwt.Token) (any, error) {
		return publicKey, nil
	})
	if err != nil {
		return nil, err
	}

	if claims, ok := token.Claims.(*CustomClaims); ok && token.Valid {
		return claims, nil
	}
	return nil, apperrors.ErrInvalidToken
}
