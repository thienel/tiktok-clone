package config

import (
	"os"
	"time"
)

type Config struct {
	Port            string
	DatabaseURL     string
	LogLevel        string
	RedisURL        string
	PublicKeyPath   string
	PrivateKeyPath  string
	AccessTokenTTL  time.Duration
	RefreshTokenTTL time.Duration
}

func Load() *Config {
	accessTokenTTL, err := time.ParseDuration(os.Getenv("ACCESS_TOKEN_TTL"))
	if err != nil {
		accessTokenTTL = 15 * time.Minute
	}
	refreshTokenTTL, err := time.ParseDuration(os.Getenv("REFRESH_TOKEN_TTL"))
	if err != nil {
		refreshTokenTTL = 168 * time.Hour
	}
	return &Config{
		Port:            getEnv("PORT", "8080"),
		DatabaseURL:     getEnv("DATABASE_URL", "postgres://auth_service:password@localhost:5432/auth_service?sslmode=disable"),
		LogLevel:        getEnv("LOG_LEVEL", "info"),
		RedisURL:        getEnv("REDIS_URL", "redis://localhost:6379"),
		PublicKeyPath:   getEnv("PUBLIC_KEY_PATH", "./keys/public.pem"),
		PrivateKeyPath:  getEnv("PRIVATE_KEY_PATH", "./keys/private.pem"),
		AccessTokenTTL:  accessTokenTTL,
		RefreshTokenTTL: refreshTokenTTL,
	}
}

func getEnv(key string, defaultVal string) string {
	if value := os.Getenv(key); value != "" {
		return value
	}

	return defaultVal
}
