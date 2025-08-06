package usecase

import (
	"context"
	"video-service/internal/domain"

	"github.com/google/uuid"
	"github.com/stretchr/testify/mock"
)

type MockVideoRepository struct {
	mock.Mock
}

func (m *MockVideoRepository) Create(ctx context.Context, video *domain.Video) error {
	args := m.Called(ctx, video)
	return args.Error(0)
}

func (m *MockVideoRepository) GetByID(ctx context.Context, id uuid.UUID) (
	*domain.Video, error) {

	args := m.Called(ctx, id)
	if args.Get(0) == nil {
		return nil, args.Error(1)
	}
	return args.Get(0).(*domain.Video), args.Error(1)
}

func (m *MockVideoRepository) GetByUserID(ctx context.Context, userID uuid.UUID,
	limit, offset int) ([]*domain.Video, error) {

	args := m.Called(ctx, userID, limit, offset)
	if args.Get(0) == nil {
		return nil, args.Error(1)
	}
	return args.Get(0).([]*domain.Video), args.Error(1)
}

func (m *MockVideoRepository) GetPublicVideos(ctx context.Context, limit, offset int) (
	[]*domain.Video, error) {

	args := m.Called(ctx, limit, offset)
	if args.Get(0) == nil {
		return nil, args.Error(1)
	}
	return args.Get(0).([]*domain.Video), args.Error(1)
}

func (m *MockVideoRepository) Update(ctx context.Context, video *domain.Video) error {
	args := m.Called(ctx, video)
	return args.Error(0)
}

func (m *MockVideoRepository) Delete(ctx context.Context, id uuid.UUID) error {
	args := m.Called(ctx, id)
	return args.Error(0)
}

func (m *MockVideoRepository) CountPublicVideos(ctx context.Context) (int64, error) {
	args := m.Called(ctx)
	return args.Get(0).(int64), args.Error(1)
}

func (m *MockVideoRepository) CountByUserID(ctx context.Context, userID uuid.UUID) (int64, error) {
	args := m.Called(ctx, userID)
	return args.Get(0).(int64), args.Error(1)
}

type MockUserVideoLikeRepository struct {
	mock.Mock
}

func (m *MockUserVideoLikeRepository) Create(ctx context.Context, like *domain.UserVideoLike) error {
	args := m.Called(ctx, like)
	return args.Error(0)
}

func (m *MockUserVideoLikeRepository) Delete(ctx context.Context, userID, videoID uuid.UUID) error {
	args := m.Called(ctx, userID, videoID)
	return args.Error(0)
}

func (m *MockUserVideoLikeRepository) Exists(ctx context.Context, userID, videoID uuid.UUID) (bool, error) {

	args := m.Called(ctx, userID, videoID)
	return args.Bool(0), args.Error(1)
}

func (m *MockUserVideoLikeRepository) CountByVideoID(ctx context.Context, videoID uuid.UUID) (int64, error) {
	args := m.Called(ctx, videoID)
	return args.Get(0).(int64), args.Error(1)
}

type MockUserVideoViewRepository struct {
	mock.Mock
}

func (m *MockUserVideoViewRepository) Create(ctx context.Context, view *domain.UserVideoView) error {
	args := m.Called(ctx, view)
	return args.Error(0)
}

func (m *MockUserVideoViewRepository) Delete(ctx context.Context, userID, videoID uuid.UUID) error {
	args := m.Called(ctx, userID, videoID)
	return args.Error(0)
}

func (m *MockUserVideoViewRepository) Exists(ctx context.Context, userID, videoID uuid.UUID) (bool, error) {

	args := m.Called(ctx, userID, videoID)
	return args.Bool(0), args.Error(1)
}

func (m *MockUserVideoViewRepository) CountByVideoID(ctx context.Context, videoID uuid.UUID) (int64, error) {
	args := m.Called(ctx, videoID)
	return args.Get(0).(int64), args.Error(1)
}
