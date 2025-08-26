package main

import (
	"context"
	"log"
	"net"
	"os"
	"os/signal"
	"syscall"
	"video-service/config"
	"video-service/internal/infrastructure/db"
	grpcHandler "video-service/internal/interface/grpc"
	"video-service/internal/pkg/logger"
	"video-service/internal/usecase"
	pb "video-service/proto"

	"go.uber.org/zap"
	"google.golang.org/grpc"
	"google.golang.org/grpc/reflection"
)

func main() {
	cfg, err := config.LoadConfig()
	if err != nil {
		log.Fatal("Failed to load config:", err)
	}

	environment := os.Getenv("ENVIRONMENT")
	if environment == "" {
		environment = "development"
	}

	var logConfig *logger.Config
	if environment == "production" {
		logConfig = logger.NewProductionConfig()
	} else {
		logConfig = logger.NewDevelopmentConfig()
	}

	if err := logger.Init(*logConfig); err != nil {
		log.Fatal("Failed to initialize logger:", err)
	}
	defer logger.Sync()

	logger.Info("Environment detected",
		zap.String("environment", environment),
	)

	logger.Info("Application starting up",
		zap.String("environment", logConfig.Environment),
		zap.String("log_level", logConfig.Level),
	)

	logger.Info("Connecting to database",
		zap.String("host", cfg.Database.Host),
		zap.String("database", cfg.Database.DBName),
		zap.String("port", cfg.Database.Port),
	)

	database, err := db.NewConnection(&cfg.Database)
	if err != nil {
		logger.Fatal("Failed to connect to database",
			zap.String("host", cfg.Database.Host),
			zap.String("database", cfg.Database.DBName),
			zap.Error(err),
		)
	}

	logger.Info("Database connection established successfully")

	logger.Info("Initializing repositories")

	videoRepo := db.NewVideoRepository(database)
	likeRepo := db.NewUserVideoLikeRepository(database)
	viewRepo := db.NewUserVideoViewRepository(database)

	logger.Info("Repositories initialized successfully")

	logger.Info("Initializing use cases")

	videoUseCase := usecase.NewVideoUseCase(videoRepo, likeRepo, viewRepo)

	logger.Info("Use cases initialized successfully")

	logger.Info("Initializing gRPC server",
		zap.String("port", cfg.Server.GRPCPort),
	)

	lis, err := net.Listen("tcp", ":"+cfg.Server.GRPCPort)
	if err != nil {
		logger.Fatal("Failed to listen on gRPC port",
			zap.String("port", cfg.Server.GRPCPort),
			zap.Error(err),
		)
	}

	s := grpc.NewServer()

	logger.Info("gRPC server configured successfully",
		zap.String("address", lis.Addr().String()),
	)

	videoHandler := grpcHandler.NewVideoHandler(videoUseCase)

	pb.RegisterVideoServiceServer(s, videoHandler)

	reflection.Register(s)

	logger.Info("gRPC reflection enabled for development")

	_, cancel := context.WithCancel(context.Background())
	defer cancel()

	c := make(chan os.Signal, 1)
	signal.Notify(c, os.Interrupt, syscall.SIGTERM)

	go func() {
		logger.Info("Starting gRPC server",
			zap.String("address", lis.Addr().String()),
		)
		if err := s.Serve(lis); err != nil {
			logger.Error("Failed to serve gRPC", zap.Error(err))
			cancel()
		}
	}()

	logger.Info("Video service is running. Press Ctrl+C to exit.")

	<-c

	logger.Info("Shutting down gracefully...")

	if sqlDB, err := database.DB(); err == nil {
		sqlDB.Close()
		logger.Info("Database connection closed")
	}

	s.GracefulStop()

	logger.Info("Server stopped")
}
