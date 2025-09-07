package api

import (
	"auth-service/internal/infrastructure/database"
	"net/http"

	"github.com/gin-gonic/gin"
)

func NewRouter(db *database.Database, handler AuthHandler) *gin.Engine {
	router := gin.New()
	router.Use(gin.Recovery(), gin.Logger())

	router.GET("/health", func(c *gin.Context) {
		err := db.Ping()
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

	api := router.Group("/api/v1")
	auth := api.Group("/auth")
	{
		auth.POST("/login", handler.Login)
		auth.POST("/register", handler.Register)
		auth.POST("/logout", handler.Logout)
		auth.POST("/token/refresh", handler.RefreshToken)
		auth.GET("/token/validate", handler.ValidateToken)
	}
	return router
}
