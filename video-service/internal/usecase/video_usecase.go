package usecase

import (
	"context"
	"time"
	"video-service/internal/domain"

	"github.com/google/uuid"
)

type VideoUseCase interface {
	CreateVideo(ctx context.Context, req *CreateVideoRequest) (*domain.Video, error)
	GetVideo(ctx context.Context, id string) (*domain.Video, error)
	ListVideos(ctx context.Context, limit, offset int) ([]*domain.Video, int64, error)
	GetVideosByUser(ctx context.Context, userID string, limit, offset int) ([]*domain.Video, int64, error)
	UpdateVideo(ctx context.Context, req *UpdateVideoRequest) (*domain.Video, error)
	DeleteVideo(ctx context.Context, id string) error
	LikeVideo(ctx context.Context, userID, videoID string) (int64, error)
	UnlikeVideo(ctx context.Context, userID, videoID string) (int64, error)
	CreateView(ctx context.Context, userID, videoID string, watchTime int) (int64, error)
	CheckUserLikedVideo(ctx context.Context, userID, videoID string) (bool, error)
	GetVideoLikeCount(ctx context.Context, videoID string) (int64, error)
}

type videoUseCase struct {
	videoRepo domain.VideoRepository
	likeRepo  domain.UserVideoLikeRepository
	viewRepo  domain.UserVideoViewRepository
}

func NewVideoUseCase(
	videoRepo domain.VideoRepository,
	likeRepo domain.UserVideoLikeRepository,
	viewRepo domain.UserVideoViewRepository,
) VideoUseCase {
	return &videoUseCase{
		videoRepo: videoRepo,
		likeRepo:  likeRepo,
		viewRepo:  viewRepo,
	}
}

type CreateVideoRequest struct {
	UserID       string `json:"user_id"`
	Title        string `json:"title"`
	Description  string `json:"description"`
	VideoURL     string `json:"video_url"`
	ThumbnailURL string `json:"thumbnail_url"`
	Duration     int    `json:"duration"`
	IsPublic     bool   `json:"is_public"`
}

func (usecase *videoUseCase) CreateVideo(ctx context.Context, req *CreateVideoRequest) (
	*domain.Video, error) {

	userID, err := uuid.Parse(req.UserID)
	if err != nil {
		return nil, err
	}

	video := domain.Video{
		UserID:       userID,
		Title:        req.Title,
		Description:  req.Description,
		VideoURL:     req.VideoURL,
		ThumbnailURL: req.ThumbnailURL,
		Duration:     req.Duration,
		IsPublic:     req.IsPublic,
	}
	err = usecase.videoRepo.Create(ctx, &video)
	if err != nil {
		return nil, err
	}

	return &video, nil
}

func (usecase *videoUseCase) GetVideo(ctx context.Context, id string) (
	*domain.Video, error) {

	uuidParsed, err := uuid.Parse(id)
	if err != nil {
		return nil, err
	}
	video, err := usecase.videoRepo.GetByID(ctx, uuidParsed)
	if err != nil {
		return nil, err
	}

	return video, nil
}

func (usecase *videoUseCase) ListVideos(ctx context.Context, limit, offset int) (
	[]*domain.Video, int64, error) {

	videos, err := usecase.videoRepo.GetPublicVideos(ctx, limit, offset)
	if err != nil {
		return nil, 0, err
	}

	totalCount, err := usecase.videoRepo.CountPublicVideos(ctx)
	if err != nil {
		return nil, 0, err
	}

	return videos, totalCount, nil
}

func (usecase *videoUseCase) GetVideosByUser(ctx context.Context, userID string,
	limit, offset int) ([]*domain.Video, int64, error) {
	uuidParsed, err := uuid.Parse(userID)
	if err != nil {
		return nil, 0, err
	}

	videos, err := usecase.videoRepo.GetByUserID(ctx, uuidParsed, limit, offset)
	if err != nil {
		return nil, 0, err
	}

	totalCount, err := usecase.videoRepo.CountByUserID(ctx, uuidParsed)
	if err != nil {
		return nil, 0, err
	}

	return videos, totalCount, nil
}

type UpdateVideoRequest struct {
	ID           string `json:"id"`
	Title        string `json:"title"`
	Description  string `json:"description"`
	ThumbnailURL string `json:"thumbnail_url"`
	IsPublic     bool   `json:"is_public"`
}

func (usecase *videoUseCase) UpdateVideo(ctx context.Context, req *UpdateVideoRequest) (
	*domain.Video, error) {

	uuidParsed, err := uuid.Parse(req.ID)
	if err != nil {
		return nil, err
	}

	video, err := usecase.videoRepo.GetByID(ctx, uuidParsed)
	if err != nil {
		return nil, err
	}

	video.Title = req.Title
	video.Description = req.Description
	video.ThumbnailURL = req.ThumbnailURL
	video.IsPublic = req.IsPublic
	video.UpdatedAt = time.Now()

	err = usecase.videoRepo.Update(ctx, video)
	if err != nil {
		return nil, err
	}

	return video, nil
}

func (usecase *videoUseCase) DeleteVideo(ctx context.Context, id string) error {
	uuidParsed, err := uuid.Parse(id)
	if err != nil {
		return err
	}

	return usecase.videoRepo.Delete(ctx, uuidParsed)
}

func (usecase *videoUseCase) LikeVideo(ctx context.Context, userID, videoID string) (
	int64, error) {
	userUUID, err := uuid.Parse(userID)
	if err != nil {
		return 0, err
	}
	videoUUID, err := uuid.Parse(videoID)
	if err != nil {
		return 0, err
	}

	exists, err := usecase.likeRepo.Exists(ctx, userUUID, videoUUID)
	if err != nil {
		return 0, err
	}
	if exists {
		count, err := usecase.likeRepo.CountByVideoID(ctx, videoUUID)
		if err != nil {
			return 0, err
		}
		return count, nil
	}

	like := &domain.UserVideoLike{
		UserID:  userUUID,
		VideoID: videoUUID,
	}

	err = usecase.likeRepo.Create(ctx, like)
	if err != nil {
		return 0, err
	}

	count, err := usecase.likeRepo.CountByVideoID(ctx, videoUUID)
	if err != nil {
		return 0, err
	}

	return count, nil
}

func (usecase *videoUseCase) UnlikeVideo(ctx context.Context, userID, videoID string) (
	int64, error) {

	userUUID, err := uuid.Parse(userID)
	if err != nil {
		return 0, err
	}

	videoUUID, err := uuid.Parse(videoID)
	if err != nil {
		return 0, err
	}

	err = usecase.likeRepo.Delete(ctx, userUUID, videoUUID)
	if err != nil {
		return 0, err
	}

	count, err := usecase.likeRepo.CountByVideoID(ctx, videoUUID)
	if err != nil {
		return 0, err
	}

	return count, nil
}

func (usecase *videoUseCase) CreateView(ctx context.Context, userID, videoID string,
	watchTime int) (int64, error) {

	userUUID, err := uuid.Parse(userID)
	if err != nil {
		return 0, err
	}
	videoUUID, err := uuid.Parse(videoID)
	if err != nil {
		return 0, err
	}

	view := &domain.UserVideoView{
		UserID:    userUUID,
		VideoID:   videoUUID,
		WatchTime: watchTime,
	}

	err = usecase.viewRepo.Create(ctx, view)
	if err != nil {
		return 0, err
	}

	count, err := usecase.viewRepo.CountByVideoID(ctx, videoUUID)
	if err != nil {
		return 0, err
	}

	return count, nil
}

func (usecase *videoUseCase) CheckUserLikedVideo(ctx context.Context, userID, videoID string) (bool, error) {
	userUUID, err := uuid.Parse(userID)
	if err != nil {
		return false, err
	}

	videoUUID, err := uuid.Parse(videoID)
	if err != nil {
		return false, err
	}

	exists, err := usecase.likeRepo.Exists(ctx, userUUID, videoUUID)
	if err != nil {
		return false, err
	}

	return exists, nil
}

func (usecase *videoUseCase) GetVideoLikeCount(ctx context.Context, videoID string) (int64, error) {
	videoUUID, err := uuid.Parse(videoID)
	if err != nil {
		return 0, err
	}

	count, err := usecase.likeRepo.CountByVideoID(ctx, videoUUID)
	if err != nil {
		return 0, err
	}

	return count, nil
}
