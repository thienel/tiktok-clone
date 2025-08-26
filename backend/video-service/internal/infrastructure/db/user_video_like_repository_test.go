package db

import (
	"context"
	"testing"
	"video-service/internal/domain"

	"github.com/google/uuid"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
)

func createTestLike() *domain.UserVideoLike {
	return &domain.UserVideoLike{
		UserID:  uuid.New(),
		VideoID: uuid.New(),
	}
}

func TestLikeCreate(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewUserVideoLikeRepository(db)
	like := createTestLike()

	err := repo.Create(context.Background(), like)
	require.NoError(t, err)
	assert.NotEqual(t, uuid.Nil, like.ID)
}

func TestLikeExists_Found(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewUserVideoLikeRepository(db)

	like := createTestLike()
	err := repo.Create(context.Background(), like)
	require.NoError(t, err)

	isExisted, err := repo.Exists(context.Background(), like.UserID, like.VideoID)
	require.NoError(t, err)
	assert.True(t, isExisted)
}

func TestLikeExists_NotFound(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewUserVideoLikeRepository(db)

	like := createTestLike()

	isExisted, err := repo.Exists(context.Background(), like.UserID, like.VideoID)
	require.NoError(t, err)
	assert.False(t, isExisted)
}

func TestLikeDelete(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewUserVideoLikeRepository(db)

	like := createTestLike()
	err := repo.Create(context.Background(), like)
	require.NoError(t, err)

	err = repo.Delete(context.Background(), like.UserID, like.VideoID)
	require.NoError(t, err)

	existed, err := repo.Exists(context.Background(), like.UserID, like.VideoID)
	require.NoError(t, err)
	assert.False(t, existed)
}

func TestLikeCountByVideoID(t *testing.T) {
	db, cleanDb := setupTestDB(t)
	defer cleanDb()

	repo := NewUserVideoLikeRepository(db)

	videoID := uuid.New()
	for i := 1; i <= 5; i++ {
		like := createTestLike()
		like.VideoID = videoID
		err := repo.Create(context.Background(), like)
		require.NoError(t, err)
	}

	count, err := repo.CountByVideoID(context.Background(), videoID)
	require.NoError(t, err)
	assert.Equal(t, int64(5), count)
}
