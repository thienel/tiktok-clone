package db

import (
	"context"
	"time"
	"video-service/internal/domain"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type videoRepository struct {
	db *gorm.DB
}

func NewVideoRepository(db *gorm.DB) domain.VideoRepository {
	return &videoRepository{db: db}
}

func (repository *videoRepository) Create(ctx context.Context, video *domain.Video) error {
	video.ID = uuid.New()
	video.CreatedAt = time.Now()
	return repository.db.WithContext(ctx).Create(&video).Error
}

func (repository *videoRepository) GetByID(ctx context.Context, id uuid.UUID) (
	*domain.Video, error) {

	var video domain.Video
	err := repository.db.WithContext(ctx).Where("id = ?", id).First(&video).Error
	if err != nil {
		return nil, err
	}
	return &video, nil
}

func (repository *videoRepository) GetByUserID(ctx context.Context, userID uuid.UUID,
	limit, offset int) ([]*domain.Video, error) {

	var videos []*domain.Video
	err := repository.db.WithContext(ctx).
		Where("user_id = ?", userID).
		Limit(limit).
		Offset(offset).
		Order("created_at DESC").
		Find(&videos).Error

	return videos, err
}

func (repository *videoRepository) GetPublicVideos(ctx context.Context, limit, offset int) (
	[]*domain.Video, error) {

	var videos []*domain.Video
	err := repository.db.WithContext(ctx).
		Where("is_public = ?", true).
		Limit(limit).
		Offset(offset).
		Order("created_at DESC").
		Find(&videos).Error

	return videos, err
}

func (repository *videoRepository) Update(ctx context.Context, video *domain.Video) error {
	return repository.db.WithContext(ctx).
		Model(&video).
		Updates(map[string]any{
			"title":       video.Title,
			"description": video.Description,
			"is_public":   video.IsPublic,
			"updated_at":  time.Now(),
		}).Error
}

func (repository *videoRepository) Delete(ctx context.Context, id uuid.UUID) error {
	return repository.db.WithContext(ctx).Delete(&domain.Video{}, id).Error
}
