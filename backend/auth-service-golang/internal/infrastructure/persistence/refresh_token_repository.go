package persistence

import (
	"auth-service/internal/domain/entities"
	"auth-service/internal/domain/repositories"
	"auth-service/internal/errors/apperrors"
	"context"
	"errors"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type refreshTokenRepository struct {
	db *gorm.DB
}

func NewTokenRepository(db *gorm.DB) repositories.RefreshTokenRepository {
	return &refreshTokenRepository{db}
}

func (t *refreshTokenRepository) Create(ctx context.Context, token *entities.RefreshToken) error {
	if err := t.db.WithContext(ctx).Create(token).Error; err != nil {
		if isDuplicateKeyError(err) {
			return apperrors.ErrDuplicateKey
		}
		return apperrors.ErrDBOperation(err)
	}
	return nil
}

func (t *refreshTokenRepository) Update(ctx context.Context, token *entities.RefreshToken) error {
	if err := t.db.WithContext(ctx).Model(token).Select("is_revoked").Updates(token).Error; err != nil {
		return apperrors.ErrDBOperation(err)
	}
	return nil
}

func (t *refreshTokenRepository) FindByID(ctx context.Context, id uuid.UUID) (*entities.RefreshToken, error) {
	var token entities.RefreshToken
	if err := t.db.WithContext(ctx).Where("id = ?", id).First(&token).Error; err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, apperrors.ErrNotFound
		}
		return nil, apperrors.ErrDBOperation(err)
	}
	return &token, nil
}

func (t *refreshTokenRepository) FindByUserID(ctx context.Context, userID uuid.UUID) ([]entities.RefreshToken, error) {
	var tokens []entities.RefreshToken
	if err := t.db.WithContext(ctx).Where("user_id = ?", userID).Find(&tokens).Error; err != nil {
		return nil, apperrors.ErrDBOperation(err)
	}
	if len(tokens) == 0 {
		return nil, apperrors.ErrNotFound
	}
	return tokens, nil
}

func (t *refreshTokenRepository) FindByToken(ctx context.Context, tokenStr string) (*entities.RefreshToken, error) {
	var token entities.RefreshToken
	if err := t.db.WithContext(ctx).Where("token = ?", tokenStr).First(&token).Error; err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, apperrors.ErrNotFound
		}
		return nil, apperrors.ErrDBOperation(err)
	}
	return &token, nil
}

func (t *refreshTokenRepository) RevokeByToken(ctx context.Context, tokenStr string) error {
	if err := t.db.WithContext(ctx).Model(&entities.RefreshToken{}).
		Where("token = ?", tokenStr).
		Update("is_revoked", true).Error; err != nil {
		return apperrors.ErrDBOperation(err)
	}
	return nil
}

func (t *refreshTokenRepository) RevokeAllByUserID(ctx context.Context, userID uuid.UUID) error {
	if err := t.db.WithContext(ctx).Model(&entities.RefreshToken{}).
		Where("user_id = ?", userID).
		Update("is_revoked", true).Error; err != nil {
		return apperrors.ErrDBOperation(err)
	}
	return nil
}
