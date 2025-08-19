package grpc

import (
	"context"
	"errors"
	"testing"
	"time"
	"video-service/internal/domain"
	"video-service/internal/pkg/logger"
	"video-service/internal/usecase"
	pb "video-service/proto"

	"github.com/google/uuid"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/mock"
	"github.com/stretchr/testify/require"
	"google.golang.org/grpc/codes"
	"google.golang.org/grpc/status"
	"gorm.io/gorm"
)

type MockVideoUseCase struct {
	mock.Mock
}

func (m *MockVideoUseCase) CreateVideo(ctx context.Context, req *usecase.CreateVideoRequest) (*domain.Video, error) {
	args := m.Called(ctx, req)
	if args.Get(0) == nil {
		return nil, args.Error(1)
	}
	return args.Get(0).(*domain.Video), args.Error(1)
}

func (m *MockVideoUseCase) GetVideo(ctx context.Context, id string) (*domain.Video, error) {
	args := m.Called(ctx, id)
	if args.Get(0) == nil {
		return nil, args.Error(1)
	}
	return args.Get(0).(*domain.Video), args.Error(1)
}

func (m *MockVideoUseCase) ListVideos(ctx context.Context, limit, offset int) ([]*domain.Video, int64, error) {
	args := m.Called(ctx, limit, offset)
	if args.Get(0) == nil {
		return nil, 0, args.Error(2)
	}
	return args.Get(0).([]*domain.Video), args.Get(1).(int64), args.Error(2)
}

func (m *MockVideoUseCase) GetVideosByUser(ctx context.Context, userID string, limit, offset int) ([]*domain.Video, int64, error) {
	args := m.Called(ctx, userID, limit, offset)
	if args.Get(0) == nil {
		return nil, 0, args.Error(2)
	}
	return args.Get(0).([]*domain.Video), args.Get(1).(int64), args.Error(2)
}

func (m *MockVideoUseCase) UpdateVideo(ctx context.Context, req *usecase.UpdateVideoRequest) (*domain.Video, error) {
	args := m.Called(ctx, req)
	if args.Get(0) == nil {
		return nil, args.Error(1)
	}
	return args.Get(0).(*domain.Video), args.Error(1)
}

func (m *MockVideoUseCase) DeleteVideo(ctx context.Context, id string) error {
	args := m.Called(ctx, id)
	return args.Error(0)
}

func (m *MockVideoUseCase) LikeVideo(ctx context.Context, userID, videoID string) (int64, error) {
	args := m.Called(ctx, userID, videoID)
	return args.Get(0).(int64), args.Error(1)
}

func (m *MockVideoUseCase) UnlikeVideo(ctx context.Context, userID, videoID string) (int64, error) {
	args := m.Called(ctx, userID, videoID)
	return args.Get(0).(int64), args.Error(1)
}

func (m *MockVideoUseCase) CreateView(ctx context.Context, userID, videoID string, watchTime int) (int64, error) {
	args := m.Called(ctx, userID, videoID, watchTime)
	return args.Get(0).(int64), args.Error(1)
}

func createTestVideoHandler() (*VideoHandler, *MockVideoUseCase) {
	logConfig := logger.NewDevelopmentConfig()
	logger.Init(*logConfig)

	mockUseCase := &MockVideoUseCase{}
	handler := NewVideoHandler(mockUseCase)

	return handler, mockUseCase
}

func createTestDomainVideo() *domain.Video {
	return &domain.Video{
		ID:           uuid.New(),
		UserID:       uuid.New(),
		Title:        "Test Video",
		Description:  "Test Description",
		VideoURL:     "https://example.com/video.mp4",
		ThumbnailURL: "https://example.com/thumb.jpg",
		Duration:     120,
		ViewCount:    0,
		LikeCount:    0,
		ShareCount:   0,
		IsPublic:     true,
		CreatedAt:    time.Now(),
		UpdatedAt:    time.Now(),
	}
}

func createTestCreateVideoRequest() *pb.CreateVideoRequest {
	return &pb.CreateVideoRequest{
		UserId:       uuid.New().String(),
		Title:        "Test Video",
		Description:  "Test Description",
		VideoUrl:     "https://example.com/video.mp4",
		ThumbnailUrl: "https://example.com/thumb.jpg",
		Duration:     120,
		IsPublic:     true,
	}
}

func TestCreateVideo_Success(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	req := createTestCreateVideoRequest()
	expectedVideo := createTestDomainVideo()

	userID, _ := uuid.Parse(req.UserId)
	expectedVideo.UserID = userID

	mockUseCase.On("CreateVideo", mock.Anything, mock.MatchedBy(func(
		ucReq *usecase.CreateVideoRequest) bool {
		return ucReq.UserID == req.UserId &&
			ucReq.Title == req.Title &&
			ucReq.VideoURL == req.VideoUrl
	})).Return(expectedVideo, nil)

	resp, err := handler.CreateVideo(context.Background(), req)

	require.NoError(t, err)
	require.NotNil(t, resp)
	assert.Equal(t, expectedVideo.ID.String(), resp.Video.Id)
	assert.Equal(t, expectedVideo.Title, resp.Video.Title)
	assert.Equal(t, expectedVideo.UserID.String(), resp.Video.UserId)
	assert.Equal(t, req.UserId, resp.Video.UserId)

	mockUseCase.AssertExpectations(t)
}

func TestCreateVideo_UserIdEmpty(t *testing.T) {
	handler, _ := createTestVideoHandler()
	req := createTestCreateVideoRequest()
	req.UserId = ""

	resp, err := handler.CreateVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "user_id is required", st.Message())
}

func TestCreateVideo_InvalidUserId(t *testing.T) {
	handler, _ := createTestVideoHandler()
	req := createTestCreateVideoRequest()
	req.UserId = "Invalid UserId"

	resp, err := handler.CreateVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "user_id must be a valid UUID", st.Message())
}

func TestCreateVideo_TitleEmpty(t *testing.T) {
	handler, _ := createTestVideoHandler()
	req := createTestCreateVideoRequest()
	req.Title = ""

	resp, err := handler.CreateVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "title is required", st.Message())
}

func TestCreateVideo_VideoUrlEmpty(t *testing.T) {
	handler, _ := createTestVideoHandler()
	req := createTestCreateVideoRequest()
	req.VideoUrl = ""

	resp, err := handler.CreateVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "video_url is required", st.Message())
}

func TestCreateVideo_InvalidDuration(t *testing.T) {
	handler, _ := createTestVideoHandler()
	req := createTestCreateVideoRequest()
	req.Duration = 0

	resp, err := handler.CreateVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "duration must be greater than 0", st.Message())
}

func TestCreateVideo_UseCaseError(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	req := createTestCreateVideoRequest()
	expectedVideo := createTestDomainVideo()

	userID, _ := uuid.Parse(req.UserId)
	expectedVideo.UserID = userID

	mockUseCase.On("CreateVideo", mock.Anything, mock.MatchedBy(func(
		ucReq *usecase.CreateVideoRequest) bool {
		return ucReq.UserID == req.UserId &&
			ucReq.Title == req.Title &&
			ucReq.VideoURL == req.VideoUrl
	})).Return(nil, errors.New("usecase error"))

	resp, err := handler.CreateVideo(context.Background(), req)

	require.Error(t, err)
	require.Nil(t, resp)
	assert.Contains(t, err.Error(), "usecase error")

	mockUseCase.AssertExpectations(t)
}

func TestGetVideo_Success(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	expectedVideo := createTestDomainVideo()
	videoID := expectedVideo.ID.String()

	mockUseCase.On("GetVideo", mock.Anything, videoID).Return(expectedVideo, nil)

	req := &pb.GetVideoRequest{Id: videoID}
	resp, err := handler.GetVideo(context.Background(), req)

	require.NoError(t, err)
	require.NotNil(t, resp)
	assert.Equal(t, videoID, resp.Video.Id)
	assert.Equal(t, expectedVideo.Title, resp.Video.Title)
	assert.Equal(t, expectedVideo.UserID.String(), resp.Video.UserId)
	assert.Equal(t, expectedVideo.Description, resp.Video.Description)
	assert.Equal(t, expectedVideo.VideoURL, resp.Video.VideoUrl)
	assert.Equal(t, expectedVideo.ThumbnailURL, resp.Video.ThumbnailUrl)
	assert.Equal(t, int32(expectedVideo.Duration), resp.Video.Duration)
	assert.Equal(t, expectedVideo.IsPublic, resp.Video.IsPublic)

	mockUseCase.AssertExpectations(t)
}

func TestGetVideo_NotFound(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	videoID := uuid.New().String()

	mockUseCase.On("GetVideo", mock.Anything, videoID).Return(nil, gorm.ErrRecordNotFound)

	req := &pb.GetVideoRequest{Id: videoID}
	resp, err := handler.GetVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.NotFound, st.Code())
	assert.Equal(t, "video not found", st.Message())

	mockUseCase.AssertExpectations(t)
}

func TestGetVideo_UseCaseError(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	videoID := uuid.New().String()

	mockUseCase.On("GetVideo", mock.Anything, videoID).Return(nil, errors.New("database connection error"))

	req := &pb.GetVideoRequest{Id: videoID}
	resp, err := handler.GetVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.Internal, st.Code())
	assert.Contains(t, st.Message(), "database connection error")

	mockUseCase.AssertExpectations(t)
}

func TestListVideos_Success(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()

	limit := 5
	offset := 5
	domainVideos := []*domain.Video{
		createTestDomainVideo(),
		createTestDomainVideo(),
		createTestDomainVideo(),
	}
	mockUseCase.On("ListVideos", mock.Anything, limit, offset).Return(
		domainVideos, int64(10), nil)

	req := &pb.ListVideosRequest{Limit: int32(limit), Offset: int32(offset)}
	resp, err := handler.ListVideos(context.Background(), req)

	assert.NoError(t, err)
	assert.NotNil(t, resp)
	assert.Len(t, resp.Videos, len(domainVideos))
	for i, video := range resp.Videos {
		assert.Equal(t, domainVideos[i].ID.String(), video.Id)
		assert.Equal(t, domainVideos[i].Title, video.Title)
		assert.Equal(t, domainVideos[i].UserID.String(), video.UserId)
		assert.Equal(t, domainVideos[i].Description, video.Description)
		assert.Equal(t, domainVideos[i].VideoURL, video.VideoUrl)
		assert.Equal(t, domainVideos[i].ThumbnailURL, video.ThumbnailUrl)
		assert.Equal(t, int32(domainVideos[i].Duration), video.Duration)
		assert.Equal(t, domainVideos[i].IsPublic, video.IsPublic)
	}
	assert.Equal(t, int64(10), resp.Total)

	mockUseCase.AssertExpectations(t)
}

func TestListVideos_UseCaseError(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()

	limit := 5
	offset := 5
	mockUseCase.On("ListVideos", mock.Anything, limit, offset).Return(
		nil, int64(0), errors.New("database connection error"))

	req := &pb.ListVideosRequest{Limit: int32(limit), Offset: int32(offset)}
	resp, err := handler.ListVideos(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.Internal, st.Code())
	assert.Contains(t, st.Message(), "database connection error")

	mockUseCase.AssertExpectations(t)
}

func TestGetVideosByUser_Success(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	userID := uuid.New().String()
	limit := 10
	offset := 0

	domainVideos := []*domain.Video{
		createTestDomainVideo(),
		createTestDomainVideo(),
	}
	total := int64(15)

	mockUseCase.On("GetVideosByUser", mock.Anything, userID, limit, offset).Return(domainVideos, total, nil)

	req := &pb.GetVideosByUserRequest{
		UserId: userID,
		Limit:  int32(limit),
		Offset: int32(offset),
	}
	resp, err := handler.GetVideosByUser(context.Background(), req)

	require.NoError(t, err)
	require.NotNil(t, resp)
	assert.Len(t, resp.Videos, len(domainVideos))
	assert.Equal(t, total, resp.Total)

	for i, video := range resp.Videos {
		assert.Equal(t, domainVideos[i].ID.String(), video.Id)
		assert.Equal(t, domainVideos[i].Title, video.Title)
		assert.Equal(t, domainVideos[i].UserID.String(), video.UserId)
	}

	mockUseCase.AssertExpectations(t)
}

func TestGetVideosByUser_InvalidUserID(t *testing.T) {
	handler, _ := createTestVideoHandler()

	req := &pb.GetVideosByUserRequest{
		UserId: "invalid-uuid",
		Limit:  10,
		Offset: 0,
	}
	resp, err := handler.GetVideosByUser(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "user_id must be a valid UUID", st.Message())
}

func TestGetVideosByUser_EmptyUserID(t *testing.T) {
	handler, _ := createTestVideoHandler()

	req := &pb.GetVideosByUserRequest{
		UserId: "",
		Limit:  10,
		Offset: 0,
	}
	resp, err := handler.GetVideosByUser(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "user_id is required", st.Message())
}

func TestGetVideosByUser_UseCaseError(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	userID := uuid.New().String()
	limit := 10
	offset := 0

	mockUseCase.On("GetVideosByUser", mock.Anything, userID, limit, offset).Return(nil, int64(0), errors.New("database error"))

	req := &pb.GetVideosByUserRequest{
		UserId: userID,
		Limit:  int32(limit),
		Offset: int32(offset),
	}
	resp, err := handler.GetVideosByUser(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.Internal, st.Code())
	assert.Equal(t, "Failed to get videos", st.Message())

	mockUseCase.AssertExpectations(t)
}

func TestUpdateVideo_Success(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	videoID := uuid.New().String()
	expectedVideo := createTestDomainVideo()

	mockUseCase.On("UpdateVideo", mock.Anything, mock.MatchedBy(func(req *usecase.UpdateVideoRequest) bool {
		return req.ID == videoID &&
			req.Title == "Updated Title" &&
			req.Description == "Updated Description" &&
			req.ThumbnailURL == "https://example.com/new-thumb.jpg" &&
			req.IsPublic == false
	})).Return(expectedVideo, nil)

	req := &pb.UpdateVideoRequest{
		Id:           videoID,
		Title:        "Updated Title",
		Description:  "Updated Description",
		ThumbnailUrl: "https://example.com/new-thumb.jpg",
		IsPublic:     false,
	}
	resp, err := handler.UpdateVideo(context.Background(), req)

	require.NoError(t, err)
	require.NotNil(t, resp)
	assert.Equal(t, expectedVideo.ID.String(), resp.Video.Id)
	assert.Equal(t, expectedVideo.Title, resp.Video.Title)

	mockUseCase.AssertExpectations(t)
}

func TestUpdateVideo_InvalidVideoID(t *testing.T) {
	handler, _ := createTestVideoHandler()

	req := &pb.UpdateVideoRequest{
		Id:          "invalid-uuid",
		Title:       "Updated Title",
		Description: "Updated Description",
		IsPublic:    true,
	}
	resp, err := handler.UpdateVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "id must be a valid UUID", st.Message())
}

func TestUpdateVideo_EmptyVideoID(t *testing.T) {
	handler, _ := createTestVideoHandler()

	req := &pb.UpdateVideoRequest{
		Id:          "",
		Title:       "Updated Title",
		Description: "Updated Description",
		IsPublic:    true,
	}
	resp, err := handler.UpdateVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "id is required", st.Message())
}

func TestUpdateVideo_EmptyTitle(t *testing.T) {
	handler, _ := createTestVideoHandler()
	videoID := uuid.New().String()

	req := &pb.UpdateVideoRequest{
		Id:          videoID,
		Title:       "",
		Description: "Updated Description",
		IsPublic:    true,
	}
	resp, err := handler.UpdateVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "title is required", st.Message())
}

func TestUpdateVideo_UseCaseError(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	videoID := uuid.New().String()

	mockUseCase.On("UpdateVideo", mock.Anything, mock.AnythingOfType("*usecase.UpdateVideoRequest")).Return(nil, errors.New("database error"))

	req := &pb.UpdateVideoRequest{
		Id:          videoID,
		Title:       "Updated Title",
		Description: "Updated Description",
		IsPublic:    true,
	}
	resp, err := handler.UpdateVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.Internal, st.Code())
	assert.Equal(t, "Failed to update video", st.Message())

	mockUseCase.AssertExpectations(t)
}

func TestDeleteVideo_Success(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	videoID := uuid.New().String()

	mockUseCase.On("DeleteVideo", mock.Anything, videoID).Return(nil)

	req := &pb.DeleteVideoRequest{Id: videoID}
	resp, err := handler.DeleteVideo(context.Background(), req)

	require.NoError(t, err)
	require.NotNil(t, resp)
	assert.True(t, resp.Success)

	mockUseCase.AssertExpectations(t)
}

func TestDeleteVideo_InvalidVideoID(t *testing.T) {
	handler, _ := createTestVideoHandler()

	req := &pb.DeleteVideoRequest{Id: "invalid-uuid"}
	resp, err := handler.DeleteVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "id must be a valid UUID", st.Message())
}

func TestDeleteVideo_EmptyVideoID(t *testing.T) {
	handler, _ := createTestVideoHandler()

	req := &pb.DeleteVideoRequest{Id: ""}
	resp, err := handler.DeleteVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "id is required", st.Message())
}

func TestDeleteVideo_UseCaseError(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	videoID := uuid.New().String()

	mockUseCase.On("DeleteVideo", mock.Anything, videoID).Return(errors.New("database error"))

	req := &pb.DeleteVideoRequest{Id: videoID}
	resp, err := handler.DeleteVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.Internal, st.Code())
	assert.Equal(t, "Failed to delete video", st.Message())

	mockUseCase.AssertExpectations(t)
}

func TestLikeVideo_Success(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	userID := uuid.New().String()
	videoID := uuid.New().String()
	expectedLikeCount := int64(15)

	mockUseCase.On("LikeVideo", mock.Anything, userID, videoID).Return(expectedLikeCount, nil)

	req := &pb.LikeVideoRequest{
		UserId:  userID,
		VideoId: videoID,
	}
	resp, err := handler.LikeVideo(context.Background(), req)

	require.NoError(t, err)
	require.NotNil(t, resp)
	assert.True(t, resp.Success)
	assert.Equal(t, expectedLikeCount, resp.LikeCount)

	mockUseCase.AssertExpectations(t)
}

func TestLikeVideo_InvalidUserID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	videoID := uuid.New().String()

	req := &pb.LikeVideoRequest{
		UserId:  "invalid-uuid",
		VideoId: videoID,
	}
	resp, err := handler.LikeVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "user_id must be a valid UUID", st.Message())
}

func TestLikeVideo_InvalidVideoID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	userID := uuid.New().String()

	req := &pb.LikeVideoRequest{
		UserId:  userID,
		VideoId: "invalid-uuid",
	}
	resp, err := handler.LikeVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "video_id must be a valid UUID", st.Message())
}

func TestLikeVideo_EmptyUserID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	videoID := uuid.New().String()

	req := &pb.LikeVideoRequest{
		UserId:  "",
		VideoId: videoID,
	}
	resp, err := handler.LikeVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "user_id is required", st.Message())
}

func TestLikeVideo_EmptyVideoID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	userID := uuid.New().String()

	req := &pb.LikeVideoRequest{
		UserId:  userID,
		VideoId: "",
	}
	resp, err := handler.LikeVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "video_id is required", st.Message())
}

func TestLikeVideo_UseCaseError(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	userID := uuid.New().String()
	videoID := uuid.New().String()

	mockUseCase.On("LikeVideo", mock.Anything, userID, videoID).Return(int64(0), errors.New("database error"))

	req := &pb.LikeVideoRequest{
		UserId:  userID,
		VideoId: videoID,
	}
	resp, err := handler.LikeVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.Internal, st.Code())
	assert.Equal(t, "Failed to like video", st.Message())

	mockUseCase.AssertExpectations(t)
}

func TestUnlikeVideo_Success(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	userID := uuid.New().String()
	videoID := uuid.New().String()
	expectedLikeCount := int64(10)

	mockUseCase.On("UnlikeVideo", mock.Anything, userID, videoID).Return(expectedLikeCount, nil)

	req := &pb.UnlikeVideoRequest{
		UserId:  userID,
		VideoId: videoID,
	}
	resp, err := handler.UnlikeVideo(context.Background(), req)

	require.NoError(t, err)
	require.NotNil(t, resp)
	assert.True(t, resp.Success)
	assert.Equal(t, expectedLikeCount, resp.LikeCount)

	mockUseCase.AssertExpectations(t)
}

func TestUnlikeVideo_InvalidUserID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	videoID := uuid.New().String()

	req := &pb.UnlikeVideoRequest{
		UserId:  "invalid-uuid",
		VideoId: videoID,
	}
	resp, err := handler.UnlikeVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "user_id must be a valid UUID", st.Message())
}

func TestUnlikeVideo_InvalidVideoID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	userID := uuid.New().String()

	req := &pb.UnlikeVideoRequest{
		UserId:  userID,
		VideoId: "invalid-uuid",
	}
	resp, err := handler.UnlikeVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "video_id must be a valid UUID", st.Message())
}

func TestUnlikeVideo_EmptyUserID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	videoID := uuid.New().String()

	req := &pb.UnlikeVideoRequest{
		UserId:  "",
		VideoId: videoID,
	}
	resp, err := handler.UnlikeVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "user_id is required", st.Message())
}

func TestUnlikeVideo_EmptyVideoID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	userID := uuid.New().String()

	req := &pb.UnlikeVideoRequest{
		UserId:  userID,
		VideoId: "",
	}
	resp, err := handler.UnlikeVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "video_id is required", st.Message())
}

func TestUnlikeVideo_UseCaseError(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	userID := uuid.New().String()
	videoID := uuid.New().String()

	mockUseCase.On("UnlikeVideo", mock.Anything, userID, videoID).Return(int64(0), errors.New("database error"))

	req := &pb.UnlikeVideoRequest{
		UserId:  userID,
		VideoId: videoID,
	}
	resp, err := handler.UnlikeVideo(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.Internal, st.Code())
	assert.Equal(t, "Failed to unlike video", st.Message())

	mockUseCase.AssertExpectations(t)
}

func TestCreateView_Success(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	userID := uuid.New().String()
	videoID := uuid.New().String()
	watchTime := int32(120)
	expectedTotalViews := int64(50)

	mockUseCase.On("CreateView", mock.Anything, userID, videoID, int(watchTime)).Return(expectedTotalViews, nil)

	req := &pb.CreateViewRequest{
		UserId:    userID,
		VideoId:   videoID,
		WatchTime: watchTime,
	}
	resp, err := handler.CreateView(context.Background(), req)

	require.NoError(t, err)
	require.NotNil(t, resp)
	assert.True(t, resp.Success)
	assert.Equal(t, expectedTotalViews, resp.TotalViews)

	mockUseCase.AssertExpectations(t)
}

func TestCreateView_InvalidUserID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	videoID := uuid.New().String()

	req := &pb.CreateViewRequest{
		UserId:    "invalid-uuid",
		VideoId:   videoID,
		WatchTime: 120,
	}
	resp, err := handler.CreateView(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "user_id must be a valid UUID", st.Message())
}

func TestCreateView_InvalidVideoID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	userID := uuid.New().String()

	req := &pb.CreateViewRequest{
		UserId:    userID,
		VideoId:   "invalid-uuid",
		WatchTime: 120,
	}
	resp, err := handler.CreateView(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "video_id must be a valid UUID", st.Message())
}

func TestCreateView_EmptyUserID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	videoID := uuid.New().String()

	req := &pb.CreateViewRequest{
		UserId:    "",
		VideoId:   videoID,
		WatchTime: 120,
	}
	resp, err := handler.CreateView(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "user_id is required", st.Message())
}

func TestCreateView_EmptyVideoID(t *testing.T) {
	handler, _ := createTestVideoHandler()
	userID := uuid.New().String()

	req := &pb.CreateViewRequest{
		UserId:    userID,
		VideoId:   "",
		WatchTime: 120,
	}
	resp, err := handler.CreateView(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "video_id is required", st.Message())
}

func TestCreateView_NegativeWatchTime(t *testing.T) {
	handler, _ := createTestVideoHandler()
	userID := uuid.New().String()
	videoID := uuid.New().String()

	req := &pb.CreateViewRequest{
		UserId:    userID,
		VideoId:   videoID,
		WatchTime: -10,
	}
	resp, err := handler.CreateView(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.InvalidArgument, st.Code())
	assert.Equal(t, "watch_time must be non-negative", st.Message())
}

func TestCreateView_ZeroWatchTime(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	userID := uuid.New().String()
	videoID := uuid.New().String()
	expectedTotalViews := int64(25)

	mockUseCase.On("CreateView", mock.Anything, userID, videoID, 0).Return(expectedTotalViews, nil)

	req := &pb.CreateViewRequest{
		UserId:    userID,
		VideoId:   videoID,
		WatchTime: 0,
	}
	resp, err := handler.CreateView(context.Background(), req)

	require.NoError(t, err)
	require.NotNil(t, resp)
	assert.True(t, resp.Success)
	assert.Equal(t, expectedTotalViews, resp.TotalViews)

	mockUseCase.AssertExpectations(t)
}

func TestCreateView_UseCaseError(t *testing.T) {
	handler, mockUseCase := createTestVideoHandler()
	userID := uuid.New().String()
	videoID := uuid.New().String()
	watchTime := int32(120)

	mockUseCase.On("CreateView", mock.Anything, userID, videoID, int(watchTime)).Return(int64(0), errors.New("database error"))

	req := &pb.CreateViewRequest{
		UserId:    userID,
		VideoId:   videoID,
		WatchTime: watchTime,
	}
	resp, err := handler.CreateView(context.Background(), req)

	assert.Error(t, err)
	assert.Nil(t, resp)

	st, ok := status.FromError(err)
	require.True(t, ok)
	assert.Equal(t, codes.Internal, st.Code())
	assert.Equal(t, "Failed to create view", st.Message())

	mockUseCase.AssertExpectations(t)
}
