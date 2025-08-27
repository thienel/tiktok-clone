package main

import (
	"auth-service/internal/config"
	"auth-service/internal/infrastructure/database"
	"auth-service/pkg/logger"
	"net/http"
	"os"

	"github.com/gin-gonic/gin"
	"github.com/joho/godotenv"
)

func main() {
	godotenv.Load(".env")
	cfg := config.Load()
	log := logger.New(cfg.LogLevel)

	db, err := database.New(cfg.DatabaseURL)
	if err != nil {
		log.Error("Failed to connect to database", "error", err)
		os.Exit(1)
	}
	defer db.Close()

	r := gin.Default()
	r.GET("/health", func(c *gin.Context) {
		c.JSON(http.StatusOK, gin.H{
			"status":  "healthy",
			"service": "auth-service",
		})
	})

	log.Info("Server starting on port " + cfg.Port)
	if err := r.Run(":" + cfg.Port); err != nil {
		log.Error("Failed to start server", "error", err)
	}
}
