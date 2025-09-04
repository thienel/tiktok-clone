package api

import (
	"auth-service/internal/application"
	"net/http"

	"github.com/gin-gonic/gin"
)

func NewRouter(app *application.App) *gin.Engine {
	router := gin.New()
	router.Use(gin.Recovery(), gin.Logger())

	router.GET("/health", func(c *gin.Context) {
		err := app.DB.Ping()
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

	router.Group("/api/v1")
	return router
}
