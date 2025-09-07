package persistence

import (
	"auth-service/internal/domain/entities"
	"auth-service/internal/domain/repositories"
	"auth-service/internal/errors/apperrors"
	"context"
	"errors"

	"github.com/google/uuid"
	"github.com/lib/pq"
	"gorm.io/gorm"
)

type userRepository struct {
	db *gorm.DB
}

func NewUserRepository(db *gorm.DB) repositories.UserRepository {
	return &userRepository{db}
}

func (u *userRepository) Create(ctx context.Context, user *entities.User) error {
	if err := u.db.WithContext(ctx).Create(user).Error; err != nil {
		if isDuplicateKeyError(err) {
			return apperrors.ErrDuplicateKey
		}
		return apperrors.ErrDBOperation(err)
	}
	return nil
}

func (u *userRepository) FindByID(ctx context.Context, id uuid.UUID) (*entities.User, error) {
	var user entities.User
	if err := u.db.WithContext(ctx).Where("id = ?", id).First(&user).Error; err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, apperrors.ErrNotFound
		}
		return nil, apperrors.ErrDBOperation(err)
	}
	return &user, nil
}

func (u *userRepository) FindByEmail(ctx context.Context, email string) (*entities.User, error) {
	var user entities.User
	if err := u.db.WithContext(ctx).Where("email = ?", email).First(&user).Error; err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, apperrors.ErrNotFound
		}
		return nil, apperrors.ErrDBOperation(err)
	}
	return &user, nil
}

func (u *userRepository) FindByUsername(ctx context.Context, username string) (*entities.User, error) {
	var user entities.User
	if err := u.db.WithContext(ctx).Where("username = ?", username).First(&user).Error; err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, apperrors.ErrNotFound
		}
		return nil, apperrors.ErrDBOperation(err)
	}
	return &user, nil
}

func (u *userRepository) Update(ctx context.Context, user *entities.User) error {
	if err := u.db.WithContext(ctx).Model(user).Select("username", "email", "status", "password_hash").
		Updates(user).Error; err != nil {
		return apperrors.ErrDBOperation(err)
	}
	return nil
}

func (u *userRepository) SoftDelete(ctx context.Context, id uuid.UUID) error {
	if err := u.db.WithContext(ctx).Delete(&entities.User{}, "id = ?", id).Error; err != nil {
		return apperrors.ErrDBOperation(err)
	}
	return nil
}

func isDuplicateKeyError(err error) bool {
	var pqErr *pq.Error
	if errors.As(err, &pqErr) {
		return pqErr.Code == "23505"
	}
	return false
}
