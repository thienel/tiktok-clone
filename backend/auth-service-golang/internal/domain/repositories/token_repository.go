package repositories

import (
	"auth-service/internal/domain/entities"
	"auth-service/internal/errors/apperrors"
	"context"
	"errors"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type TokenRepository interface {
	Create(ctx context.Context, params *entities.Token) error
	FindByID(ctx context.Context, id uuid.UUID) (*entities.Token, error)
	FindByUserID(ctx context.Context, userID uuid.UUID) ([]entities.Token, error)
	FindByToken(ctx context.Context, tokenStr string) (*entities.Token, error)
	SoftDelete(ctx context.Context, id uuid.UUID) error
}

type tokenRepository struct {
	db *gorm.DB
}

func NewTokenRepository(db *gorm.DB) TokenRepository { return &tokenRepository{db} }

func (t tokenRepository) Create(ctx context.Context, token *entities.Token) error {
	if err := t.db.WithContext(ctx).Create(token).Error; err != nil {
		if isDuplicateKeyError(err) {
			return apperrors.ErrDuplicateKey
		}
		return err
	}
	return nil
}

func (t tokenRepository) FindByID(ctx context.Context, id uuid.UUID) (*entities.Token, error) {
	var token entities.Token
	if err := t.db.WithContext(ctx).Where("id = ?", id).First(&token).Error; err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, apperrors.ErrNotFound
		}
		return nil, err
	}
	return &token, nil
}

func (t tokenRepository) FindByUserID(ctx context.Context, userID uuid.UUID) ([]entities.Token, error) {
	var tokens []entities.Token
	if err := t.db.WithContext(ctx).Where("user_id = ?", userID).Find(&tokens).Error; err != nil {
		return nil, err
	}
	if len(tokens) == 0 {
		return nil, apperrors.ErrNotFound
	}
	return tokens, nil
}

func (t tokenRepository) FindByToken(ctx context.Context, tokenStr string) (*entities.Token, error) {
	var token entities.Token
	if err := t.db.WithContext(ctx).Where("token = ?", tokenStr).First(&token).Error; err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, apperrors.ErrNotFound
		}
		return nil, err
	}
	return &token, nil
}

func (t tokenRepository) SoftDelete(ctx context.Context, id uuid.UUID) error {
	if err := t.db.WithContext(ctx).Delete(&entities.Token{}, "id = ?", id).Error; err != nil {
		return err
	}
	return nil
}
