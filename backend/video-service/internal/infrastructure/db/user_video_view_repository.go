package db

import (
	"context"
	"time"
	"video-service/internal/domain"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type userVideoViewRepository struct {
	db *gorm.DB
}

func NewUserVideoViewRepository(db *gorm.DB) domain.UserVideoViewRepository {
	return &userVideoViewRepository{db: db}
}

func (repository *userVideoViewRepository) Create(ctx context.Context, view *domain.UserVideoView) error {
	view.ID = uuid.New()
	view.CreatedAt = time.Now()
	return repository.db.WithContext(ctx).Create(view).Error
}

func (repository *userVideoViewRepository) Delete(ctx context.Context, userID, videoID uuid.UUID) error {
	return repository.db.WithContext(ctx).
		Where("user_id = ?", userID).
		Where("video_id = ?", videoID).
		Delete(&domain.UserVideoView{}).Error
}

func (repository *userVideoViewRepository) Exists(ctx context.Context, userID, videoID uuid.UUID) (bool, error) {
	var count int64
	err := repository.db.WithContext(ctx).
		Model(&domain.UserVideoView{}).
		Where("user_id = ? AND video_id = ?", userID, videoID).
		Count(&count).Error

	return count > 0, err
}

func (repository *userVideoViewRepository) CountByVideoID(ctx context.Context, videoID uuid.UUID) (int64, error) {
	var count int64
	err := repository.db.WithContext(ctx).
		Model(&domain.UserVideoView{}).
		Where("video_id = ?", videoID).
		Count(&count).Error

	return count, err
}
