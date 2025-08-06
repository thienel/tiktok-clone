package db

import (
	"testing"
	"video-service/config"

	"github.com/stretchr/testify/require"
	"gorm.io/gorm"
)

func getTestConfig() *config.DatabaseConfig {
	return &config.DatabaseConfig{
		Host:     "localhost",
		Port:     "5432",
		User:     "video_user",
		Password: "password",
		DBName:   "video_service_test",
		SSLMode:  "disable",
	}
}

func setupTestDB(t *testing.T) (*gorm.DB, func()) {
	t.Helper()
	if testing.Short() {
		t.Skip("skipping database test in short mode")
	}

	cfg := getTestConfig()
	db, err := NewConnection(cfg)
	require.NoError(t, err)

	sqlDB, err := db.DB()
	require.NoError(t, err)

	return db, func() {
		sqlDB.Close()
	}
}
