package db

import (
	"database/sql"
	"fmt"
	"time"
	"video-service/config"
	"video-service/internal/domain"

	"gorm.io/driver/postgres"
	"gorm.io/gorm"
)

type Database interface {
	AutoMigrate(dst ...any) error
	DB() (*sql.DB, error)
}

type GormDB struct {
	db *gorm.DB
}

func (g *GormDB) DB() (*sql.DB, error) {
	return g.db.DB()
}

func (g *GormDB) AutoMigrate(dst ...any) error {
	return g.db.AutoMigrate(dst...)
}

func NewConnection(cfg *config.DatabaseConfig) (*gorm.DB, error) {
	db, err := createConnection(cfg)
	if err != nil {
		return nil, err
	}

	gormDB := &GormDB{db: db}
	return setupDatabase(gormDB)
}

func createConnection(cfg *config.DatabaseConfig) (*gorm.DB, error) {
	dsn := fmt.Sprintf("host=%s user=%s password=%s dbname=%s port=%s sslmode=%s TimeZone=UTC",
		cfg.Host,
		cfg.User,
		cfg.Password,
		cfg.DBName,
		cfg.Port,
		cfg.SSLMode,
	)

	db, err := gorm.Open(postgres.Open(dsn), &gorm.Config{})
	if err != nil {
		return nil, fmt.Errorf("failed to connect to database: %w", err)
	}

	return db, nil
}

func setupDatabase(database Database) (*gorm.DB, error) {
	sqlDB, err := database.DB()
	if err != nil {
		return nil, fmt.Errorf("failed to get database instance: %w", err)
	}

	sqlDB.SetMaxIdleConns(10)
	sqlDB.SetMaxOpenConns(100)
	sqlDB.SetConnMaxLifetime(time.Hour)

	err = database.AutoMigrate(
		&domain.Video{},
		&domain.UserVideoLike{},
		&domain.UserVideoView{},
	)

	if err != nil {
		return nil, fmt.Errorf("failed to migrate database: %w", err)
	}

	if gormDB, ok := database.(*GormDB); ok {
		return gormDB.db, nil
	}

	return nil, fmt.Errorf("unsupported database type")
}
