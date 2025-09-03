package main

import (
	"auth-service/internal/config"
	"auth-service/internal/crypto"
	"auth-service/internal/infrastructure/database"
	"auth-service/pkg/logger"
	"context"
	"errors"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/joho/godotenv"
)

func main() {
	_ = godotenv.Load(".env")
	cfg := config.Load()
	log := logger.New(cfg.LogLevel)

	err := crypto.InitRSAKeys(cfg.PublicKeyPath, cfg.PrivateKeyPath)
	if err != nil {
		log.Error("Failed to initialize RSA keys", "error", err)
		os.Exit(1)
	}

	db, err := database.New(cfg.DatabaseURL)
	if err != nil {
		log.Error("Failed to connect to database", "error", err)
		os.Exit(1)
	}
	defer func(db *database.Database) {
		err := db.Close()
		if err != nil {
			log.Error("Failed to close database connection", "error", err)
		}
	}(db)

	r := gin.Default()
	r.Use(gin.Recovery())
	r.Use(gin.Logger())
	r.GET("/health", func(c *gin.Context) {
		err := db.DB.Ping()
		if err != nil {
			c.JSON(http.StatusInternalServerError, gin.H{
				"status":  "unhealthy",
				"service": "auth-service",
				"db":      "down",
			})
			return
		}

		c.JSON(http.StatusOK, gin.H{
			"status":  "healthy",
			"service": "auth-service",
			"db":      "up",
		})
	})

	log.Info("Server starting on port " + cfg.Port)
	srv := &http.Server{
		Addr:    ":" + cfg.Port,
		Handler: r,
	}

	go func() {
		if err := srv.ListenAndServe(); err != nil && !errors.Is(err, http.ErrServerClosed) {
			log.Error("Failed to start server", "error", err)
			os.Exit(1)
		}
	}()

	quit := make(chan os.Signal, 1)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)
	<-quit

	log.Info("Shutting down server...")
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()
	if err = srv.Shutdown(ctx); err != nil {
		log.Error("Server forced to shutdown", "error", err)
	}
}
