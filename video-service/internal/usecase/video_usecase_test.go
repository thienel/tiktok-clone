package usecase

import (
	"context"
	"errors"
	"testing"
	"time"
	"video-service/internal/domain"

	"github.com/google/uuid"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/mock"
	"github.com/stretchr/testify/require"
	"gorm.io/gorm"
)

type MockVideoRepository struct {
	mock.Mock
}

func (m *MockVideoRepository) Create(ctx context.Context,
	video *domain.Video) error {
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

func (m *MockVideoRepository) GetByUserID(ctx context.Context,
	userID uuid.UUID,
	limit, offset int) ([]*domain.Video, error) {

	args := m.Called(ctx, userID, limit, offset)
	if args.Get(0) == nil {
		return nil, args.Error(1)
	}
	return args.Get(0).([]*domain.Video), args.Error(1)
}

func (m *MockVideoRepository) GetPublicVideos(ctx context.Context,
	limit, offset int) (
	[]*domain.Video, error) {

	args := m.Called(ctx, limit, offset)
	if args.Get(0) == nil {
		return nil, args.Error(1)
	}
	return args.Get(0).([]*domain.Video), args.Error(1)
}

func (m *MockVideoRepository) Update(ctx context.Context,
	video *domain.Video) error {
	args := m.Called(ctx, video)
	return args.Error(0)
}

func (m *MockVideoRepository) Delete(ctx context.Context,
	id uuid.UUID) error {
	args := m.Called(ctx, id)
	return args.Error(0)
}

func (m *MockVideoRepository) CountPublicVideos(ctx context.Context) (
	int64, error) {
	args := m.Called(ctx)
	return args.Get(0).(int64), args.Error(1)
}

func (m *MockVideoRepository) CountByUserID(ctx context.Context,
	userID uuid.UUID) (int64, error) {
	args := m.Called(ctx, userID)
	return args.Get(0).(int64), args.Error(1)
}

type MockUserVideoLikeRepository struct {
	mock.Mock
}

func (m *MockUserVideoLikeRepository) Create(ctx context.Context,
	like *domain.UserVideoLike) error {
	args := m.Called(ctx, like)
	return args.Error(0)
}

func (m *MockUserVideoLikeRepository) Delete(ctx context.Context,
	userID, videoID uuid.UUID) error {
	args := m.Called(ctx, userID, videoID)
	return args.Error(0)
}

func (m *MockUserVideoLikeRepository) Exists(ctx context.Context,
	userID, videoID uuid.UUID) (bool, error) {

	args := m.Called(ctx, userID, videoID)
	return args.Bool(0), args.Error(1)
}

func (m *MockUserVideoLikeRepository) CountByVideoID(ctx context.Context,
	videoID uuid.UUID) (int64, error) {
	args := m.Called(ctx, videoID)
	return args.Get(0).(int64), args.Error(1)
}

type MockUserVideoViewRepository struct {
	mock.Mock
}

func (m *MockUserVideoViewRepository) Create(ctx context.Context,
	view *domain.UserVideoView) error {
	args := m.Called(ctx, view)
	return args.Error(0)
}

func (m *MockUserVideoViewRepository) Delete(ctx context.Context,
	userID, videoID uuid.UUID) error {
	args := m.Called(ctx, userID, videoID)
	return args.Error(0)
}

func (m *MockUserVideoViewRepository) Exists(ctx context.Context,
	userID, videoID uuid.UUID) (bool, error) {

	args := m.Called(ctx, userID, videoID)
	return args.Bool(0), args.Error(1)
}

func (m *MockUserVideoViewRepository) CountByVideoID(ctx context.Context,
	videoID uuid.UUID) (int64, error) {
	args := m.Called(ctx, videoID)
	return args.Get(0).(int64), args.Error(1)
}

func createTestVideoUseCase() (*videoUseCase, *MockVideoRepository,
	*MockUserVideoLikeRepository, *MockUserVideoViewRepository) {

	mockVideoRepository := &MockVideoRepository{}
	mockLikeRepository := &MockUserVideoLikeRepository{}
	mockViewRepository := &MockUserVideoViewRepository{}

	usecase := &videoUseCase{
		videoRepo: mockVideoRepository,
		likeRepo:  mockLikeRepository,
		viewRepo:  mockViewRepository,
	}

	return usecase, mockVideoRepository, mockLikeRepository, mockViewRepository
}

func createTestVideo() *domain.Video {
	return &domain.Video{
		ID:           uuid.New(),
		UserID:       uuid.New(),
		Title:        "Test Video",
		Description:  "Test Description",
		VideoURL:     "https://example.com/video.mp4",
		ThumbnailURL: "https://example.com/thumb.jpg",
		Duration:     120,
		IsPublic:     true,
		CreatedAt:    time.Now(),
		UpdatedAt:    time.Now(),
	}
}

func createTestCreateVideoRequest() *CreateVideoRequest {
	return &CreateVideoRequest{
		UserID:       uuid.New().String(),
		Title:        "Test Video",
		Description:  "Test Description",
		VideoURL:     "https://example.com/video.mp4",
		ThumbnailURL: "https://example.com/thumb.jpg",
		Duration:     120,
		IsPublic:     true,
	}
}

func TestCreateVideo_Success(t *testing.T) {
	usecase, mockVideoRepo, _, _ := createTestVideoUseCase()
	req := createTestCreateVideoRequest()

	mockVideoRepo.On("Create", mock.Anything, mock.AnythingOfType("*domain.Video")).
		Return(nil)

	video, err := usecase.CreateVideo(context.Background(), req)

	require.NoError(t, err)
	assert.NotNil(t, video)
	assert.Equal(t, req.Title, video.Title)
	assert.Equal(t, req.Description, video.Description)
	mockVideoRepo.AssertExpectations(t)
}

func TestCreateVideo_InvalidUserID(t *testing.T) {
	usecase, _, _, _ := createTestVideoUseCase()
	req := createTestCreateVideoRequest()
	req.UserID = "invalid-uuid"

	video, err := usecase.CreateVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, video)
}

func TestCreateVideo_RepositoryError(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()
	req := createTestCreateVideoRequest()

	mockVideoRepository.On("Create", mock.Anything, mock.AnythingOfType("*domain.Video")).
		Return(errors.New("database error"))

	video, err := usecase.CreateVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, video)
	assert.Contains(t, err.Error(), "database error")
	mockVideoRepository.AssertExpectations(t)
}

func TestGetVideo_Success(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()
	testVideo := createTestVideo()

	mockVideoRepository.On("GetByID", mock.Anything, testVideo.ID).
		Return(testVideo, nil)

	video, err := usecase.GetVideo(context.Background(), testVideo.ID.String())

	assert.NoError(t, err)
	assert.Equal(t, testVideo, video)
	mockVideoRepository.AssertExpectations(t)
}

func TestGetVideo_InvalidID(t *testing.T) {
	usecase, _, _, _ := createTestVideoUseCase()

	video, err := usecase.GetVideo(context.Background(), "Invalid ID")

	assert.Error(t, err)
	assert.Nil(t, video)
}

func TestGetVideo_NotFound(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	mockVideoRepository.On("GetByID", mock.Anything, mock.Anything).
		Return(nil, gorm.ErrRecordNotFound)

	video, err := usecase.GetVideo(context.Background(), uuid.NewString())

	assert.Error(t, err)
	assert.ErrorIs(t, err, gorm.ErrRecordNotFound)
	assert.Nil(t, video)
	mockVideoRepository.AssertExpectations(t)
}

func TestListVideos_Success(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	limit, offset := 10, 0
	expectedVideos := []*domain.Video{
		createTestVideo(),
		createTestVideo(),
		createTestVideo(),
	}
	expectedVideos[0].Title = "Video 1"
	expectedVideos[1].Title = "Video 2"
	expectedVideos[2].Title = "Video 3"

	expectedTotalCount := int64(100)

	mockVideoRepository.On("GetPublicVideos", mock.Anything, limit, offset).
		Return(expectedVideos, nil)
	mockVideoRepository.On("CountPublicVideos", mock.Anything).
		Return(expectedTotalCount, nil)

	videosListed, totalCount, err := usecase.ListVideos(context.Background(), limit, offset)

	require.NoError(t, err)
	assert.Equal(t, expectedVideos, videosListed)
	assert.Equal(t, expectedTotalCount, totalCount)
	assert.Len(t, videosListed, 3)
	mockVideoRepository.AssertExpectations(t)
}

func TestListVideos_GetPublicVideosError(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	mockVideoRepository.On("GetPublicVideos", mock.Anything, mock.Anything, mock.Anything).
		Return(nil, errors.New("database error"))

	videos, totalCount, err := usecase.ListVideos(context.Background(), 10, 0)

	assert.Nil(t, videos)
	assert.Equal(t, int64(0), totalCount)
	assert.Error(t, err)
	assert.Contains(t, err.Error(), "database error")
	mockVideoRepository.AssertExpectations(t)
}

func TestListVideos_CountPublicVideosError(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	expectedVideos := []*domain.Video{createTestVideo()}
	mockVideoRepository.On("GetPublicVideos", mock.Anything, 10, 0).
		Return(expectedVideos, nil)

	mockVideoRepository.On("CountPublicVideos", mock.Anything).
		Return(int64(0), errors.New("database error"))

	videos, totalCount, err := usecase.ListVideos(context.Background(), 10, 0)

	assert.Nil(t, videos)
	assert.Equal(t, int64(0), totalCount)
	assert.Error(t, err)
	assert.Contains(t, err.Error(), "database error")
	mockVideoRepository.AssertExpectations(t)
}
