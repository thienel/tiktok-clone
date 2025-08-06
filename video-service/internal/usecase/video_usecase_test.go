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

func TestNewVideoUseCase(t *testing.T) {
	_, mockVideoRepository, mockLikeRepository, mockViewRepository := createTestVideoUseCase()

	usecase := NewVideoUseCase(mockVideoRepository, mockLikeRepository, mockViewRepository)

	assert.NotNil(t, usecase)
	concreteUseCase, ok := usecase.(*videoUseCase)

	assert.True(t, ok, "usecase should be of type *videoUseCase")
	assert.Equal(t, mockVideoRepository, concreteUseCase.videoRepo)
	assert.Equal(t, mockLikeRepository, concreteUseCase.likeRepo)
	assert.Equal(t, mockViewRepository, concreteUseCase.viewRepo)
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

func TestGetVideosByUser_Success(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	limit, offset := 10, 0
	userID := uuid.New()
	expectedTotalCount := int64(100)
	expectedVideos := []*domain.Video{
		createTestVideo(),
		createTestVideo(),
		createTestVideo(),
	}
	expectedVideos[0].Title = "Video 1"
	expectedVideos[0].UserID = userID
	expectedVideos[1].Title = "Video 2"
	expectedVideos[1].UserID = userID
	expectedVideos[2].Title = "Video 3"
	expectedVideos[2].UserID = userID

	mockVideoRepository.On("GetByUserID", mock.Anything, userID, limit, offset).
		Return(expectedVideos, nil)
	mockVideoRepository.On("CountByUserID", mock.Anything, userID).
		Return(expectedTotalCount, nil)

	videos, totalCount, err := usecase.GetVideosByUser(context.Background(), userID.String(), limit, offset)

	require.NoError(t, err)
	assert.Equal(t, expectedVideos, videos)
	assert.Equal(t, expectedTotalCount, totalCount)
	assert.Len(t, videos, 3)
	for _, video := range videos {
		assert.Equal(t, userID, video.UserID)
	}
	mockVideoRepository.AssertExpectations(t)
}

func TestGetVideosByUser_InvalidUserID(t *testing.T) {
	usecase, _, _, _ := createTestVideoUseCase()

	videos, totalCount, err := usecase.GetVideosByUser(context.Background(), "Invalid UserID", 10, 0)

	assert.Nil(t, videos)
	assert.Equal(t, int64(0), totalCount)
	assert.Error(t, err)
}

func TestGetVideosByUser_GetByUserIDError(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	mockVideoRepository.On("GetByUserID", mock.Anything, mock.Anything, mock.Anything, mock.Anything).
		Return(nil, errors.New("database error"))

	videos, totalCount, err := usecase.GetVideosByUser(context.Background(), uuid.NewString(), 10, 0)

	assert.Nil(t, videos)
	assert.Equal(t, int64(0), totalCount)
	assert.Error(t, err)
	assert.Contains(t, err.Error(), "database error")
	mockVideoRepository.AssertExpectations(t)
}

func TestGetVideosByUser_CountByUserIDError(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	userID := uuid.New()
	expectedVideos := []*domain.Video{createTestVideo()}
	expectedVideos[0].UserID = userID

	mockVideoRepository.On("GetByUserID", mock.Anything, userID, 10, 0).
		Return(expectedVideos, nil)

	mockVideoRepository.On("CountByUserID", mock.Anything, userID).
		Return(int64(0), errors.New("database error"))

	videos, totalCount, err := usecase.GetVideosByUser(context.Background(), userID.String(), 10, 0)

	assert.Nil(t, videos)
	assert.Equal(t, int64(0), totalCount)
	assert.Error(t, err)
	assert.Contains(t, err.Error(), "database error")
	mockVideoRepository.AssertExpectations(t)
}

func createTestUpdateVideoRequest() *UpdateVideoRequest {
	return &UpdateVideoRequest{
		ID:           uuid.NewString(),
		Title:        "Test Update Video",
		Description:  "Test Update Description",
		ThumbnailURL: "https://update.exmple.com/thumb.jpg",
		IsPublic:     false,
	}
}

func TestUpdatevideo_Success(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	originalVideo := createTestVideo()
	req := createTestUpdateVideoRequest()
	req.ID = originalVideo.ID.String()

	originalTime := originalVideo.UpdatedAt

	mockVideoRepository.On("GetByID", mock.Anything, originalVideo.ID).
		Return(originalVideo, nil)
	mockVideoRepository.On("Update", mock.Anything, mock.AnythingOfType("*domain.Video")).
		Return(nil)

	updatedVideo, err := usecase.UpdateVideo(context.Background(), req)

	require.NoError(t, err)
	assert.NotNil(t, updatedVideo)

	assert.Equal(t, req.Title, updatedVideo.Title)
	assert.Equal(t, req.Description, updatedVideo.Description)
	assert.Equal(t, req.ThumbnailURL, updatedVideo.ThumbnailURL)
	assert.Equal(t, req.IsPublic, updatedVideo.IsPublic)
	assert.True(t, updatedVideo.UpdatedAt.After(originalTime))
	assert.WithinDuration(t, time.Now(), updatedVideo.UpdatedAt, time.Second)
	assert.Equal(t, originalVideo.ID, updatedVideo.ID)
	assert.Equal(t, originalVideo.UserID, updatedVideo.UserID)
	assert.Equal(t, originalVideo.VideoURL, updatedVideo.VideoURL)
	assert.Equal(t, originalVideo.Duration, updatedVideo.Duration)
	assert.Equal(t, originalVideo.CreatedAt, updatedVideo.CreatedAt)

	mockVideoRepository.AssertExpectations(t)
}

func TestUpdatevideo_InvalidID(t *testing.T) {
	usecase, _, _, _ := createTestVideoUseCase()

	req := createTestUpdateVideoRequest()
	req.ID = "Invalid ID"

	video, err := usecase.UpdateVideo(context.Background(), req)

	assert.Nil(t, video)
	assert.Error(t, err)
}

func TestUpdatevideo_NotFound(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	videoID := uuid.New()
	req := createTestUpdateVideoRequest()
	req.ID = videoID.String()

	mockVideoRepository.On("GetByID", mock.Anything, videoID).
		Return(nil, gorm.ErrRecordNotFound)

	video, err := usecase.UpdateVideo(context.Background(), req)

	assert.Nil(t, video)
	assert.Error(t, err)
	assert.ErrorIs(t, err, gorm.ErrRecordNotFound)

	mockVideoRepository.AssertExpectations(t)
}

func TestUpdatevideo_GetByIDError(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	videoID := uuid.New()
	req := createTestUpdateVideoRequest()
	req.ID = videoID.String()

	mockVideoRepository.On("GetByID", mock.Anything, videoID).
		Return(nil, errors.New("database error"))

	video, err := usecase.UpdateVideo(context.Background(), req)

	assert.Nil(t, video)
	assert.Error(t, err)
	assert.Contains(t, err.Error(), "database error")

	mockVideoRepository.AssertExpectations(t)
}

func TestUpdatevideo_UpdateError(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	originalVideo := createTestVideo()
	req := createTestUpdateVideoRequest()
	req.ID = originalVideo.ID.String()

	mockVideoRepository.On("GetByID", mock.Anything, originalVideo.ID).
		Return(originalVideo, nil)
	mockVideoRepository.On("Update", mock.Anything, mock.AnythingOfType("*domain.Video")).
		Return(errors.New("database error"))

	video, err := usecase.UpdateVideo(context.Background(), req)

	assert.Nil(t, video)
	assert.Error(t, err)
	assert.Contains(t, err.Error(), "database error")

	mockVideoRepository.AssertExpectations(t)
}

func TestDeleteVideo_Success(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	videoID := uuid.New()

	mockVideoRepository.On("Delete", mock.Anything, videoID).
		Return(nil)

	err := usecase.DeleteVideo(context.Background(), videoID.String())

	assert.NoError(t, err)
	mockVideoRepository.AssertExpectations(t)
}

func TestDeleteVideo_InvalidID(t *testing.T) {
	usecase, _, _, _ := createTestVideoUseCase()

	err := usecase.DeleteVideo(context.Background(), "Invalid ID")

	assert.Error(t, err)
}

func TestDeleteVideo_NotFound(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	videoID := uuid.New()

	mockVideoRepository.On("Delete", mock.Anything, videoID).
		Return(gorm.ErrRecordNotFound)

	err := usecase.DeleteVideo(context.Background(), videoID.String())

	assert.Error(t, err)
	assert.ErrorIs(t, err, gorm.ErrRecordNotFound)
	mockVideoRepository.AssertExpectations(t)
}

func TestDeleteVideo_RepositoryError(t *testing.T) {
	usecase, mockVideoRepository, _, _ := createTestVideoUseCase()

	videoID := uuid.New()

	mockVideoRepository.On("Delete", mock.Anything, videoID).
		Return(errors.New("database error"))

	err := usecase.DeleteVideo(context.Background(), videoID.String())

	assert.Error(t, err)
	assert.Contains(t, err.Error(), "database error")
	mockVideoRepository.AssertExpectations(t)
}

func TestLikeVideo_Success(t *testing.T) {
	usecase, _, mockLikeRepository, _ := createTestVideoUseCase()

	userID := uuid.New().String()
	videoID := uuid.New().String()

	userUUID, _ := uuid.Parse(userID)
	videoUUID, _ := uuid.Parse(videoID)

	mockLikeRepository.On("Exists", mock.Anything, userUUID, videoUUID).
		Return(false, nil)
	mockLikeRepository.On("Create", mock.Anything, mock.AnythingOfType("*domain.UserVideoLike")).
		Return(nil)
	mockLikeRepository.On("CountByVideoID", mock.Anything, videoUUID).
		Return(int64(1), nil)

	likeCount, err := usecase.LikeVideo(context.Background(), userID, videoID)

	assert.NoError(t, err)
	assert.Equal(t, int64(1), likeCount)
	mockLikeRepository.AssertExpectations(t)
}

func TestLikeVideo_InvalidUserID(t *testing.T) {
	usecase, _, _, _ := createTestVideoUseCase()

	likeCount, err := usecase.LikeVideo(context.Background(), "invalid-uuid", uuid.New().String())

	assert.Error(t, err)
	assert.Equal(t, int64(0), likeCount)
}

func TestLikeVideo_InvalidVideoID(t *testing.T) {
	usecase, _, _, _ := createTestVideoUseCase()

	likeCount, err := usecase.LikeVideo(context.Background(), uuid.New().String(), "invalid-uuid")

	assert.Error(t, err)
	assert.Equal(t, int64(0), likeCount)
}

func TestLikeVideo_AlreadyLiked(t *testing.T) {
	usecase, _, mockLikeRepository, _ := createTestVideoUseCase()

	userID := uuid.New().String()
	videoID := uuid.New().String()

	userUUID, _ := uuid.Parse(userID)
	videoUUID, _ := uuid.Parse(videoID)

	mockLikeRepository.On("Exists", mock.Anything, userUUID, videoUUID).
		Return(true, nil)
	mockLikeRepository.On("CountByVideoID", mock.Anything, videoUUID).
		Return(int64(5), nil)

	likeCount, err := usecase.LikeVideo(context.Background(), userID, videoID)

	assert.NoError(t, err)
	assert.Equal(t, int64(5), likeCount)
	mockLikeRepository.AssertExpectations(t)
}

func TestLikeVideo_ExistsCountError(t *testing.T) {
	usecase, _, mockLikeRepository, _ := createTestVideoUseCase()

	userID := uuid.New().String()
	videoID := uuid.New().String()

	userUUID, _ := uuid.Parse(userID)
	videoUUID, _ := uuid.Parse(videoID)

	mockLikeRepository.On("Exists", mock.Anything, userUUID, videoUUID).
		Return(true, nil)
	mockLikeRepository.On("CountByVideoID", mock.Anything, videoUUID).
		Return(int64(0), errors.New("database error"))

	likeCount, err := usecase.LikeVideo(context.Background(), userID, videoID)

	assert.Equal(t, int64(0), likeCount)
	assert.Error(t, err)
	assert.Contains(t, err.Error(), "database error")
	mockLikeRepository.AssertExpectations(t)
}

func TestLikeVideo_ExistsError(t *testing.T) {
	usecase, _, mockLikeRepository, _ := createTestVideoUseCase()

	userID := uuid.New().String()
	videoID := uuid.New().String()

	userUUID, _ := uuid.Parse(userID)
	videoUUID, _ := uuid.Parse(videoID)

	mockLikeRepository.On("Exists", mock.Anything, userUUID, videoUUID).
		Return(false, errors.New("database error"))

	likeCount, err := usecase.LikeVideo(context.Background(), userID, videoID)

	assert.Error(t, err)
	assert.Equal(t, int64(0), likeCount)
	assert.Contains(t, err.Error(), "database error")
	mockLikeRepository.AssertExpectations(t)
}

func TestLikeVideo_CreateError(t *testing.T) {
	usecase, _, mockLikeRepository, _ := createTestVideoUseCase()

	userID := uuid.New().String()
	videoID := uuid.New().String()

	userUUID, _ := uuid.Parse(userID)
	videoUUID, _ := uuid.Parse(videoID)

	mockLikeRepository.On("Exists", mock.Anything, userUUID, videoUUID).
		Return(false, nil)
	mockLikeRepository.On("Create", mock.Anything, mock.AnythingOfType("*domain.UserVideoLike")).
		Return(errors.New("database error"))

	likeCount, err := usecase.LikeVideo(context.Background(), userID, videoID)

	assert.Error(t, err)
	assert.Equal(t, int64(0), likeCount)
	assert.Contains(t, err.Error(), "database error")
	mockLikeRepository.AssertExpectations(t)
}

func TestLikeVideo_CountError(t *testing.T) {
	usecase, _, mockLikeRepository, _ := createTestVideoUseCase()

	userID := uuid.New().String()
	videoID := uuid.New().String()

	userUUID, _ := uuid.Parse(userID)
	videoUUID, _ := uuid.Parse(videoID)

	mockLikeRepository.On("Exists", mock.Anything, userUUID, videoUUID).
		Return(false, nil)
	mockLikeRepository.On("Create", mock.Anything, mock.AnythingOfType("*domain.UserVideoLike")).
		Return(nil)
	mockLikeRepository.On("CountByVideoID", mock.Anything, videoUUID).
		Return(int64(0), errors.New("database error"))

	likeCount, err := usecase.LikeVideo(context.Background(), userID, videoID)

	assert.Error(t, err)
	assert.Equal(t, int64(0), likeCount)
	assert.Contains(t, err.Error(), "database error")
	mockLikeRepository.AssertExpectations(t)
}

func TestUnlikeVideo_Success(t *testing.T) {
	usecase, _, mockLikeRepository, _ := createTestVideoUseCase()

	userID := uuid.New().String()
	videoID := uuid.New().String()

	userUUID, _ := uuid.Parse(userID)
	videoUUID, _ := uuid.Parse(videoID)

	mockLikeRepository.On("Delete", mock.Anything, userUUID, videoUUID).
		Return(nil)
	mockLikeRepository.On("CountByVideoID", mock.Anything, videoUUID).
		Return(int64(4), nil)

	likeCount, err := usecase.UnlikeVideo(context.Background(), userID, videoID)

	assert.NoError(t, err)
	assert.Equal(t, int64(4), likeCount)
	mockLikeRepository.AssertExpectations(t)
}

func TestUnlikeVideo_InvalidUserID(t *testing.T) {
	usecase, _, _, _ := createTestVideoUseCase()

	likeCount, err := usecase.UnlikeVideo(context.Background(), "invalid-uuid", uuid.New().String())

	assert.Error(t, err)
	assert.Equal(t, int64(0), likeCount)
}

func TestUnlikeVideo_InvalidVideoID(t *testing.T) {
	usecase, _, _, _ := createTestVideoUseCase()

	likeCount, err := usecase.UnlikeVideo(context.Background(), uuid.New().String(), "invalid-uuid")

	assert.Error(t, err)
	assert.Equal(t, int64(0), likeCount)
}

func TestUnlikeVideo_NotLiked(t *testing.T) {
	usecase, _, mockLikeRepository, _ := createTestVideoUseCase()

	userID := uuid.New().String()
	videoID := uuid.New().String()

	userUUID, _ := uuid.Parse(userID)
	videoUUID, _ := uuid.Parse(videoID)

	mockLikeRepository.On("Delete", mock.Anything, userUUID, videoUUID).
		Return(gorm.ErrRecordNotFound)

	likeCount, err := usecase.UnlikeVideo(context.Background(), userID, videoID)

	assert.Error(t, err)
	assert.Equal(t, int64(0), likeCount)
	assert.ErrorIs(t, err, gorm.ErrRecordNotFound)
	mockLikeRepository.AssertExpectations(t)
}

func TestUnlikeVideo_ExistsError(t *testing.T) {
	usecase, _, mockLikeRepository, _ := createTestVideoUseCase()

	userID := uuid.New().String()
	videoID := uuid.New().String()

	userUUID, _ := uuid.Parse(userID)
	videoUUID, _ := uuid.Parse(videoID)

	mockLikeRepository.On("Delete", mock.Anything, userUUID, videoUUID).
		Return(errors.New("database error"))

	likeCount, err := usecase.UnlikeVideo(context.Background(), userID, videoID)

	assert.Error(t, err)
	assert.Equal(t, int64(0), likeCount)
	assert.Contains(t, err.Error(), "database error")
	mockLikeRepository.AssertExpectations(t)
}

func TestUnlikeVideo_DeleteError(t *testing.T) {
	usecase, _, mockLikeRepository, _ := createTestVideoUseCase()

	userID := uuid.New().String()
	videoID := uuid.New().String()

	userUUID, _ := uuid.Parse(userID)
	videoUUID, _ := uuid.Parse(videoID)

	mockLikeRepository.On("Delete", mock.Anything, userUUID, videoUUID).
		Return(errors.New("database error"))

	likeCount, err := usecase.UnlikeVideo(context.Background(), userID, videoID)

	assert.Error(t, err)
	assert.Equal(t, int64(0), likeCount)
	assert.Contains(t, err.Error(), "database error")
	mockLikeRepository.AssertExpectations(t)
}

func TestUnlikeVideo_CountError(t *testing.T) {
	usecase, _, mockLikeRepository, _ := createTestVideoUseCase()

	userID := uuid.New().String()
	videoID := uuid.New().String()

	userUUID, _ := uuid.Parse(userID)
	videoUUID, _ := uuid.Parse(videoID)

	mockLikeRepository.On("Delete", mock.Anything, userUUID, videoUUID).
		Return(nil)
	mockLikeRepository.On("CountByVideoID", mock.Anything, videoUUID).
		Return(int64(0), errors.New("database error"))

	likeCount, err := usecase.UnlikeVideo(context.Background(), userID, videoID)

	assert.Error(t, err)
	assert.Equal(t, int64(0), likeCount)
	assert.Contains(t, err.Error(), "database error")
	mockLikeRepository.AssertExpectations(t)
}
