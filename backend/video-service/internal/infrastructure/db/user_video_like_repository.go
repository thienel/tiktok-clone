package db

import (
	"context"
	"time"
	"video-service/internal/domain"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type userVideoLikeRepository struct {
	db *gorm.DB
}

func NewUserVideoLikeRepository(db *gorm.DB) domain.UserVideoLikeRepository {
	return &userVideoLikeRepository{db: db}
}

func (repository *userVideoLikeRepository) Create(ctx context.Context, like *domain.UserVideoLike) error {
	like.ID = uuid.New()
	like.CreatedAt = time.Now()
	return repository.db.WithContext(ctx).Create(like).Error
}

func (repository *userVideoLikeRepository) Delete(ctx context.Context, userID, videoID uuid.UUID) error {
	return repository.db.WithContext(ctx).
		Where("user_id = ?", userID).
		Where("video_id = ?", videoID).
		Delete(&domain.UserVideoLike{}).Error
}

func (repository *userVideoLikeRepository) Exists(ctx context.Context, userID, videoID uuid.UUID) (bool, error) {
	var count int64
	err := repository.db.WithContext(ctx).
		Model(&domain.UserVideoLike{}).
		Where("user_id = ? AND video_id = ?", userID, videoID).
		Count(&count).Error

	return count > 0, err
}

func (repository *userVideoLikeRepository) CountByVideoID(ctx context.Context, videoID uuid.UUID) (int64, error) {
	var count int64
	err := repository.db.WithContext(ctx).
		Model(&domain.UserVideoLike{}).
		Where("video_id = ?", videoID).
		Count(&count).Error

	return count, err
}
