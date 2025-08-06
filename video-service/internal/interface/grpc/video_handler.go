package grpc

import (
	"video-service/internal/usecase"
	pb "video-service/proto"
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
