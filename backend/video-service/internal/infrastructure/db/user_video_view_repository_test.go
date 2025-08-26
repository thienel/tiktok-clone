package db

import (
	"context"
	"testing"
	"video-service/internal/domain"

	"github.com/google/uuid"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
)

func createTestView() *domain.UserVideoView {
	return &domain.UserVideoView{
		UserID:  uuid.New(),
		VideoID: uuid.New(),
	}
}

func TestViewCreate(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewUserVideoViewRepository(db)
	view := createTestView()

	err := repo.Create(context.Background(), view)
	require.NoError(t, err)
	assert.NotEqual(t, uuid.Nil, view.ID)
}

func TestViewExists_Found(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewUserVideoViewRepository(db)

	view := createTestView()
	err := repo.Create(context.Background(), view)
	require.NoError(t, err)

	isExisted, err := repo.Exists(context.Background(), view.UserID, view.VideoID)
	require.NoError(t, err)
	assert.True(t, isExisted)
}

func TestViewExists_NotFound(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewUserVideoViewRepository(db)

	view := createTestView()

	isExisted, err := repo.Exists(context.Background(), view.UserID, view.VideoID)
	require.NoError(t, err)
	assert.False(t, isExisted)
}

func TestViewDelete(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewUserVideoViewRepository(db)

	view := createTestView()
	err := repo.Create(context.Background(), view)
	require.NoError(t, err)

	err = repo.Delete(context.Background(), view.UserID, view.VideoID)
	require.NoError(t, err)

	existed, err := repo.Exists(context.Background(), view.UserID, view.VideoID)
	require.NoError(t, err)
	assert.False(t, existed)
}

func TestViewCountByVideoID(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewUserVideoViewRepository(db)

	videoID := uuid.New()
	for i := 1; i <= 5; i++ {
		view := createTestView()
		view.VideoID = videoID
		err := repo.Create(context.Background(), view)
		require.NoError(t, err)
	}

	count, err := repo.CountByVideoID(context.Background(), videoID)
	require.NoError(t, err)
	assert.Equal(t, int64(5), count)
}
