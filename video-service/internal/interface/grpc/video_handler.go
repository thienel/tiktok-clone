package grpc

import (
	"context"
	"video-service/internal/domain"
	"video-service/internal/pkg/logger"
	"video-service/internal/usecase"
	pb "video-service/proto"

	"github.com/google/uuid"
	"go.uber.org/zap"
	"google.golang.org/grpc/codes"
	"google.golang.org/grpc/status"
	"google.golang.org/protobuf/types/known/timestamppb"
	"gorm.io/gorm"
)

type VideoHandler struct {
	pb.UnimplementedVideoServiceServer
	videoUseCase usecase.VideoUseCase
}

func NewVideoHandler(videoUseCase usecase.VideoUseCase) *VideoHandler {
	return &VideoHandler{
		videoUseCase: videoUseCase,
	}
}

func domainVideoToProto(video *domain.Video) *pb.Video {
	return &pb.Video{
		Id:           video.ID.String(),
		UserId:       video.UserID.String(),
		Title:        video.Title,
		Description:  video.Description,
		VideoUrl:     video.VideoURL,
		ThumbnailUrl: video.ThumbnailURL,
		Duration:     int32(video.Duration),
		ViewCount:    video.ViewCount,
		LikeCount:    video.LikeCount,
		ShareCount:   video.ShareCount,
		IsPublic:     video.IsPublic,
		CreatedAt:    timestamppb.New(video.CreatedAt),
		UpdatedAt:    timestamppb.New(video.UpdatedAt),
	}
}

func listVideosToProto(videos []*domain.Video) []*pb.Video {
	protoVideos := make([]*pb.Video, len(videos))
	for i, video := range videos {
		protoVideos[i] = domainVideoToProto(video)
	}

	return protoVideos
}

func protoCreateRequestToUseCase(req *pb.CreateVideoRequest) *usecase.CreateVideoRequest {
	return &usecase.CreateVideoRequest{
		UserID:       req.UserId,
		Title:        req.Title,
		Description:  req.Description,
		VideoURL:     req.VideoUrl,
		ThumbnailURL: req.ThumbnailUrl,
		Duration:     int(req.Duration),
		IsPublic:     req.IsPublic,
	}
}

func validateCreateVideoRequest(req *pb.CreateVideoRequest) error {
	if req.UserId == "" {
		return status.Error(codes.InvalidArgument, "user_id is required")
	}
	if _, err := uuid.Parse(req.UserId); err != nil {
		return status.Error(codes.InvalidArgument, "user_id must be a valid UUID")
	}
	if req.Title == "" {
		return status.Error(codes.InvalidArgument, "title is required")
	}
	if req.VideoUrl == "" {
		return status.Error(codes.InvalidArgument, "video_url is required")
	}
	if req.Duration <= 0 {
		return status.Error(codes.InvalidArgument, "duration must be greater than 0")
	}
	return nil
}

func validateUUID(id, fieldName string) error {
	if id == "" {
		return status.Errorf(codes.InvalidArgument, "%s is required", fieldName)
	}
	if _, err := uuid.Parse(id); err != nil {
		return status.Errorf(codes.InvalidArgument, "%s must be a valid UUID", fieldName)
	}
	return nil
}

func validateUserVideoRequest(userID, videoID string) error {
	if err := validateUUID(userID, "user_id"); err != nil {
		return err
	}
	if err := validateUUID(videoID, "video_id"); err != nil {
		return err
	}
	return nil
}

func protoUpdateRequestToUseCase(req *pb.UpdateVideoRequest) *usecase.UpdateVideoRequest {
	return &usecase.UpdateVideoRequest{
		ID:           req.Id,
		Title:        req.Title,
		Description:  req.Description,
		ThumbnailURL: req.ThumbnailUrl,
		IsPublic:     req.IsPublic,
	}
}

func validateUpdateVideoRequest(req *pb.UpdateVideoRequest) error {
	if err := validateUUID(req.Id, "id"); err != nil {
		return err
	}
	if req.Title == "" {
		return status.Error(codes.InvalidArgument, "title is required")
	}
	return nil
}

func (h *VideoHandler) CreateVideo(ctx context.Context, req *pb.CreateVideoRequest) (*pb.CreateVideoResponse, error) {
	logger.Info("CreateVideo request received",
		zap.String("user_id", req.UserId),
		zap.String("title", req.Title),
		zap.String("video_url", req.VideoUrl),
		zap.Int32("duration", req.Duration),
		zap.Bool("is_public", req.IsPublic),
	)

	if err := validateCreateVideoRequest(req); err != nil {
		logger.Error("CreateVideo validation failed",
			zap.String("user_id", req.UserId),
			zap.Error(err),
		)
		return nil, err
	}

	createReq := protoCreateRequestToUseCase(req)

	video, err := h.videoUseCase.CreateVideo(ctx, createReq)
	if err != nil {
		logger.Error("Failed to create video",
			zap.String("user_id", req.UserId),
			zap.String("title", req.Title),
			zap.Error(err),
		)
		return nil, status.Errorf(codes.Internal, "failed to create video: %v", err)
	}

	logger.Info("Video created successfully",
		zap.String("video_id", video.ID.String()),
		zap.String("user_id", req.UserId),
		zap.String("title", video.Title),
	)

	return &pb.CreateVideoResponse{
		Video: domainVideoToProto(video),
	}, nil
}

func (h *VideoHandler) GetVideo(ctx context.Context, req *pb.GetVideoRequest) (*pb.GetVideoResponse, error) {
	logger.Info("GetVideo request received",
		zap.String("video_id", req.Id),
	)

	video, err := h.videoUseCase.GetVideo(ctx, req.Id)
	if err != nil {
		logger.Error("Failed to get video",
			zap.String("video_id", req.Id),
			zap.Error(err),
		)

		if err == gorm.ErrRecordNotFound {
			return nil, status.Errorf(codes.NotFound, "video not found")
		}
		return nil, status.Errorf(codes.Internal, "failed to get video: %v", err)
	}

	logger.Info("Video retrieved successfully",
		zap.String("video_id", req.Id),
	)

	return &pb.GetVideoResponse{
		Video: domainVideoToProto(video),
	}, nil
}

func (h *VideoHandler) ListVideos(ctx context.Context, req *pb.ListVideosRequest) (*pb.ListVideosResponse, error) {
	logger.Info("ListVideos request received",
		zap.Int32("limit", req.Limit),
		zap.Int32("offset", req.Offset),
	)

	videos, total, err := h.videoUseCase.ListVideos(ctx, int(req.Limit), int(req.Offset))
	if err != nil {
		logger.Error("Failed to list videos",
			zap.Error(err),
		)

		return nil, status.Errorf(codes.Internal, "failed to list videos: %v", err)
	}

	logger.Info("Videos retrieved successfully",
		zap.Any("videos", videos),
		zap.Int64("total", total),
	)

	return &pb.ListVideosResponse{
		Videos: listVideosToProto(videos),
		Total:  total,
	}, nil
}

func (h *VideoHandler) GetVideosByUser(ctx context.Context, req *pb.GetVideosByUserRequest) (*pb.GetVideosByUserResponse, error) {
	logger.Info("GetVideosByUser request received",
		zap.String("user_id", req.UserId),
		zap.Int32("limit", req.Limit),
		zap.Int32("offset", req.Offset))

	if err := validateUUID(req.UserId, "user_id"); err != nil {
		logger.Error("Invalid user_id in GetVideosByUser request", zap.Error(err))
		return nil, err
	}

	videos, total, err := h.videoUseCase.GetVideosByUser(ctx, req.UserId, int(req.Limit), int(req.Offset))
	if err != nil {
		logger.Error("Failed to get videos by user", zap.Error(err), zap.String("user_id", req.UserId))
		return nil, status.Error(codes.Internal, "Failed to get videos")
	}

	protoVideos := listVideosToProto(videos)
	logger.Info("GetVideosByUser request completed successfully",
		zap.String("user_id", req.UserId),
		zap.Int("video_count", len(protoVideos)),
		zap.Int64("total", total))

	return &pb.GetVideosByUserResponse{Videos: protoVideos, Total: total}, nil
}

func (h *VideoHandler) UpdateVideo(ctx context.Context, req *pb.UpdateVideoRequest) (*pb.UpdateVideoResponse, error) {
	logger.Info("UpdateVideo request received", zap.String("video_id", req.Id))

	if err := validateUpdateVideoRequest(req); err != nil {
		logger.Error("Invalid UpdateVideo request", zap.Error(err))
		return nil, err
	}

	updateReq := protoUpdateRequestToUseCase(req)
	video, err := h.videoUseCase.UpdateVideo(ctx, updateReq)
	if err != nil {
		logger.Error("Failed to update video", zap.Error(err), zap.String("video_id", req.Id))
		return nil, status.Error(codes.Internal, "Failed to update video")
	}

	protoVideo := domainVideoToProto(video)
	logger.Info("UpdateVideo request completed successfully", zap.String("video_id", req.Id))

	return &pb.UpdateVideoResponse{Video: protoVideo}, nil
}

func (h *VideoHandler) DeleteVideo(ctx context.Context, req *pb.DeleteVideoRequest) (*pb.DeleteVideoResponse, error) {
	logger.Info("DeleteVideo request received", zap.String("video_id", req.Id))

	if err := validateUUID(req.Id, "id"); err != nil {
		logger.Error("Invalid video_id in DeleteVideo request", zap.Error(err))
		return nil, err
	}

	err := h.videoUseCase.DeleteVideo(ctx, req.Id)
	if err != nil {
		logger.Error("Failed to delete video", zap.Error(err), zap.String("video_id", req.Id))
		return nil, status.Error(codes.Internal, "Failed to delete video")
	}

	logger.Info("DeleteVideo request completed successfully", zap.String("video_id", req.Id))
	return &pb.DeleteVideoResponse{Success: true}, nil
}

func (h *VideoHandler) LikeVideo(ctx context.Context, req *pb.LikeVideoRequest) (*pb.LikeVideoResponse, error) {
	logger.Info("LikeVideo request received",
		zap.String("user_id", req.UserId),
		zap.String("video_id", req.VideoId))

	if err := validateUserVideoRequest(req.UserId, req.VideoId); err != nil {
		logger.Error("Invalid LikeVideo request", zap.Error(err))
		return nil, err
	}

	likeCount, err := h.videoUseCase.LikeVideo(ctx, req.UserId, req.VideoId)
	if err != nil {
		logger.Error("Failed to like video", zap.Error(err),
			zap.String("user_id", req.UserId),
			zap.String("video_id", req.VideoId))
		return nil, status.Error(codes.Internal, "Failed to like video")
	}

	logger.Info("LikeVideo request completed successfully",
		zap.String("user_id", req.UserId),
		zap.String("video_id", req.VideoId),
		zap.Int64("like_count", likeCount))

	return &pb.LikeVideoResponse{Success: true, LikeCount: likeCount}, nil
}

func (h *VideoHandler) UnlikeVideo(ctx context.Context, req *pb.UnlikeVideoRequest) (*pb.UnlikeVideoResponse, error) {
	logger.Info("UnlikeVideo request received",
		zap.String("user_id", req.UserId),
		zap.String("video_id", req.VideoId))

	if err := validateUserVideoRequest(req.UserId, req.VideoId); err != nil {
		logger.Error("Invalid UnlikeVideo request", zap.Error(err))
		return nil, err
	}

	likeCount, err := h.videoUseCase.UnlikeVideo(ctx, req.UserId, req.VideoId)
	if err != nil {
		logger.Error("Failed to unlike video", zap.Error(err),
			zap.String("user_id", req.UserId),
			zap.String("video_id", req.VideoId))
		return nil, status.Error(codes.Internal, "Failed to unlike video")
	}

	logger.Info("UnlikeVideo request completed successfully",
		zap.String("user_id", req.UserId),
		zap.String("video_id", req.VideoId),
		zap.Int64("like_count", likeCount))

	return &pb.UnlikeVideoResponse{Success: true, LikeCount: likeCount}, nil
}

func (h *VideoHandler) CreateView(ctx context.Context, req *pb.CreateViewRequest) (*pb.CreateViewResponse, error) {
	logger.Info("CreateView request received",
		zap.String("user_id", req.UserId),
		zap.String("video_id", req.VideoId),
		zap.Int32("watch_time", req.WatchTime))

	if err := validateUserVideoRequest(req.UserId, req.VideoId); err != nil {
		logger.Error("Invalid CreateView request", zap.Error(err))
		return nil, err
	}

	if req.WatchTime < 0 {
		logger.Error("Invalid watch_time in CreateView request", zap.Int32("watch_time", req.WatchTime))
		return nil, status.Error(codes.InvalidArgument, "watch_time must be non-negative")
	}

	totalViews, err := h.videoUseCase.CreateView(ctx, req.UserId, req.VideoId, int(req.WatchTime))
	if err != nil {
		logger.Error("Failed to create view", zap.Error(err),
			zap.String("user_id", req.UserId),
			zap.String("video_id", req.VideoId))
		return nil, status.Error(codes.Internal, "Failed to create view")
	}

	logger.Info("CreateView request completed successfully",
		zap.String("user_id", req.UserId),
		zap.String("video_id", req.VideoId),
		zap.Int64("total_views", totalViews))

	return &pb.CreateViewResponse{Success: true, TotalViews: totalViews}, nil
}
