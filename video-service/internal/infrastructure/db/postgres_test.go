package db

import (
	"testing"
	"video-service/config"
	"video-service/internal/domain"

	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
)

func TestNewConnection_Success(t *testing.T) {
	if testing.Short() {
		t.Skip("skipping database test in short mode")
	}

	cfg := getTestConfig()

	db, err := NewConnection(cfg)

	require.NoError(t, err)
	require.NotNil(t, db)

	sqlDB, err := db.DB()
	require.NoError(t, err)

	err = sqlDB.Ping()
	assert.NoError(t, err)

	sqlDB.Close()
}

func TestNewConnection_InvalidConfig(t *testing.T) {
	cfg := &config.DatabaseConfig{
		Port:     "1234",
		User:     "invalid",
		Password: "invalid",
		DBName:   "invalid",
		SSLMode:  "disable",
	}

	db, err := NewConnection(cfg)

	assert.Error(t, err)
	assert.Nil(t, db)
	assert.Contains(t, err.Error(), "failed to connect to database")
}

func TestAutoMigration(t *testing.T) {
	if testing.Short() {
		t.Skip("skipping database test in short mode")
	}

	cfg := getTestConfig()
	db, err := NewConnection(cfg)

	require.NoError(t, err)
	assert.True(t, db.Migrator().HasTable(&domain.Video{}))
	assert.True(t, db.Migrator().HasTable(&domain.UserVideoLike{}))
	assert.True(t, db.Migrator().HasTable(&domain.UserVideoView{}))

	sqlDB, _ := db.DB()
	sqlDB.Close()
}
