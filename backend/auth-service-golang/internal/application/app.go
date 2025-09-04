package application

import (
	"auth-service/internal/config"
	"auth-service/internal/infrastructure/database"
	"auth-service/pkg/logger"
)

type App struct {
	Cfg *config.Config
	DB  *database.Database
	Log logger.Logger
}
