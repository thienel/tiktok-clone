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
