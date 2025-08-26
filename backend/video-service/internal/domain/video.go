package domain

import (
	"context"
	"time"

	"github.com/google/uuid"
)

type Video struct {
	ID           uuid.UUID `json:"id" gorm:"type:uuid;primary_key"`
	UserID       uuid.UUID `json:"user_id" gorm:"type:uuid;not null"`
	Title        string    `json:"title" gorm:"not null"`
	Description  string    `json:"description"`
	VideoURL     string    `json:"video_url" gorm:"not null"`
	ThumbnailURL string    `json:"thumbnail_url"`
	Duration     int       `json:"duration" gorm:"not null"`
	ViewCount    int64     `json:"view_count" gorm:"default:0"`
	LikeCount    int64     `json:"like_count" gorm:"default:0"`
	ShareCount   int64     `json:"share_count" gorm:"default:0"`
	IsPublic     bool      `json:"is_public"`
	CreatedAt    time.Time `json:"created_at"`
	UpdatedAt    time.Time `json:"updated_at"`
}

type VideoRepository interface {
	Create(ctx context.Context, video *Video) error
	GetByID(ctx context.Context, id uuid.UUID) (*Video, error)
	GetByUserID(ctx context.Context, userID uuid.UUID, limit, offset int) ([]*Video, error)
	CountByUserID(ctx context.Context, userID uuid.UUID) (int64, error)
	GetPublicVideos(ctx context.Context, limit, offset int) ([]*Video, error)
	CountPublicVideos(ctx context.Context) (int64, error)
	Update(ctx context.Context, video *Video) error
	Delete(ctx context.Context, id uuid.UUID) error
}

type UserVideoLike struct {
	ID        uuid.UUID `json:"id" gorm:"type:uuid;primary_key"`
	UserID    uuid.UUID `json:"user_id" gorm:"type:uuid;not null"`
	VideoID   uuid.UUID `json:"video_id" gorm:"type:uuid;not null"`
	CreatedAt time.Time `json:"created_at"`
}

type UserVideoLikeRepository interface {
	Create(ctx context.Context, like *UserVideoLike) error
	Delete(ctx context.Context, userID, videoID uuid.UUID) error
	Exists(ctx context.Context, userID, videoID uuid.UUID) (bool, error)
	CountByVideoID(ctx context.Context, videoID uuid.UUID) (int64, error)
}

type UserVideoView struct {
	ID        uuid.UUID `json:"id" gorm:"type:uuid;primary_key"`
	UserID    uuid.UUID `json:"user_id" gorm:"type:uuid;not null"`
	VideoID   uuid.UUID `json:"video_id" gorm:"type:uuid;not null"`
	WatchTime int       `json:"watch_time" gorm:"default:0"`
	CreatedAt time.Time `json:"created_at"`
}

type UserVideoViewRepository interface {
	Create(ctx context.Context, view *UserVideoView) error
	Delete(ctx context.Context, userID, videoID uuid.UUID) error
	Exists(ctx context.Context, userID, videoID uuid.UUID) (bool, error)
	CountByVideoID(ctx context.Context, videoID uuid.UUID) (int64, error)
}
