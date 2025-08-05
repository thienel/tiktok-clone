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
