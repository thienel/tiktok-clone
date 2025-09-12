package api

import (
	"auth-service/internal/infrastructure/database"
	"net/http"

	"github.com/gin-gonic/gin"
)

func NewRouter(db *database.Database, authHandler AuthHandler, oauthHandler OAuthHandler) *gin.Engine {
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
		auth.POST("/login", authHandler.Login)
		auth.POST("/register", authHandler.Register)
		auth.POST("/logout", authHandler.Logout)
		auth.POST("/token/refresh", authHandler.RefreshToken)
		auth.GET("/token/validate", authHandler.ValidateToken)
		auth.GET("/oauth/:provider", oauthHandler.InitiateOAuth)
		auth.GET("/oauth/:provider/callback", oauthHandler.HandleCallback)
	}
	return router
}
