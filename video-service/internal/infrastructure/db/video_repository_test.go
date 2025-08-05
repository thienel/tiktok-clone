package db

import (
	"context"
	"fmt"
	"testing"
	"video-service/internal/domain"

	"github.com/google/uuid"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
	"gorm.io/gorm"
)

func createTestVideo() *domain.Video {
	return &domain.Video{
		UserID:       uuid.New(),
		Title:        "Test Video",
		Description:  "Test Description",
		VideoURL:     "https://example.com/video.mp4",
		ThumbnailURL: "https://example.com/thumb.jpg",
		Duration:     120,
		IsPublic:     true,
	}
}

func TestVideoCreate(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewVideoRepository(db)
	video := createTestVideo()

	err := repo.Create(context.Background(), video)
	require.NoError(t, err)

	assert.NotEqual(t, uuid.Nil, video.ID)
}

func TestVideoGetByID_Success(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewVideoRepository(db)

	video := createTestVideo()
	err := repo.Create(context.Background(), video)
	require.NoError(t, err)

	found, err := repo.GetByID(context.Background(), video.ID)
	require.NoError(t, err)
	assert.Equal(t, video.ID, found.ID)
	assert.Equal(t, video.UserID, found.UserID)
}

func TestVideoGetByID_NotFound(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewVideoRepository(db)

	randomID := uuid.New()
	found, err := repo.GetByID(context.Background(), randomID)

	assert.Error(t, err)
	assert.Nil(t, found)
	assert.Equal(t, gorm.ErrRecordNotFound, err)
}

func TestVideoGetByUserID(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewVideoRepository(db)
	userID := uuid.New()

	for i := 0; i < 5; i++ {
		video := createTestVideo()
		video.UserID = userID
		video.Title = fmt.Sprintf("Video %d", i)
		err := repo.Create(context.Background(), video)
		require.NoError(t, err)
	}

	videos, err := repo.GetByUserID(context.Background(), userID, 3, 0)
	require.NoError(t, err)
	assert.Len(t, videos, 3)

	videos2, err := repo.GetByUserID(context.Background(), userID, 3, 3)
	require.NoError(t, err)
	assert.Len(t, videos2, 2)
}

func TestVideoGetPublicVideos(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewVideoRepository(db)

	publicVideo := createTestVideo()
	publicVideo.IsPublic = true
	err := repo.Create(context.Background(), publicVideo)
	require.NoError(t, err)

	privateVideo := createTestVideo()
	privateVideo.IsPublic = false
	err = repo.Create(context.Background(), privateVideo)
	require.NoError(t, err)

	videos, err := repo.GetPublicVideos(context.Background(), 10, 0)
	require.NoError(t, err)

	for _, video := range videos {
		assert.True(t, video.IsPublic)
	}
}

func TestVideoUpdate(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewVideoRepository(db)

	video := createTestVideo()
	err := repo.Create(context.Background(), video)
	require.NoError(t, err)

	video.Title = "Updated Title"
	video.Description = "Updated Description"
	video.IsPublic = false

	err = repo.Update(context.Background(), video)
	require.NoError(t, err)

	updated, err := repo.GetByID(context.Background(), video.ID)
	require.NoError(t, err)
	assert.Equal(t, "Updated Title", updated.Title)
	assert.Equal(t, "Updated Description", updated.Description)
	assert.False(t, updated.IsPublic)
	assert.True(t, updated.UpdatedAt.After(updated.CreatedAt))
}

func TestVideoDelete(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewVideoRepository(db)

	video := createTestVideo()
	err := repo.Create(context.Background(), video)
	require.NoError(t, err)

	err = repo.Delete(context.Background(), video.ID)
	require.NoError(t, err)

	_, err = repo.GetByID(context.Background(), video.ID)
	assert.Error(t, err)
	assert.Equal(t, gorm.ErrRecordNotFound, err)
}

func TestVideoCountPublicVideos(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewVideoRepository(db)

	// Clear existing videos to ensure clean test
	db.Exec("DELETE FROM videos")

	// Create 5 public videos
	for i := 1; i <= 5; i++ {
		video := &domain.Video{
			UserID:       uuid.New(),
			Title:        fmt.Sprintf("Public Video %d", i),
			Description:  "Test Description",
			VideoURL:     "https://example.com/video.mp4",
			ThumbnailURL: "https://example.com/thumb.jpg",
			Duration:     120,
			IsPublic:     true,
		}
		err := repo.Create(context.Background(), video)
		require.NoError(t, err)
	}

	// Create 3 private videos
	for i := 1; i <= 3; i++ {
		video := &domain.Video{
			UserID:       uuid.New(),
			Title:        fmt.Sprintf("Private Video %d", i),
			Description:  "Test Description",
			VideoURL:     "https://example.com/video.mp4",
			ThumbnailURL: "https://example.com/thumb.jpg",
			Duration:     120,
			IsPublic:     false,
		}
		err := repo.Create(context.Background(), video)
		require.NoError(t, err)
	}

	count, err := repo.CountPublicVideos(context.Background())
	require.NoError(t, err)
	assert.Equal(t, int64(5), count)
}

func TestVideoCountByUserID(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewVideoRepository(db)

	db.Exec("DELETE FROM videos")

	userID := uuid.New()
	otherUserID := uuid.New()

	for i := 1; i <= 4; i++ {
		video := createTestVideo()
		video.UserID = userID
		video.Title = fmt.Sprintf("User Video %d", i)
		err := repo.Create(context.Background(), video)
		require.NoError(t, err)
	}

	for i := 1; i <= 2; i++ {
		video := createTestVideo()
		video.UserID = otherUserID
		video.Title = fmt.Sprintf("Other User Video %d", i)
		err := repo.Create(context.Background(), video)
		require.NoError(t, err)
	}

	count, err := repo.CountByUserID(context.Background(), userID)
	require.NoError(t, err)
	assert.Equal(t, int64(4), count)

	otherCount, err := repo.CountByUserID(context.Background(), otherUserID)
	require.NoError(t, err)
	assert.Equal(t, int64(2), otherCount)
}
