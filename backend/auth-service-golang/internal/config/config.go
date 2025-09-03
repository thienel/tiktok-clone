package config

import (
	"os"
)

type Config struct {
	Port           string
	DatabaseURL    string
	LogLevel       string
	RedisURL       string
	PublicKeyPath  string
	PrivateKeyPath string
}

func Load() *Config {
	return &Config{
		Port:           getEnv("PORT", "8080"),
		DatabaseURL:    getEnv("DATABASE_URL", "postgres://auth_service:password@localhost:5432/auth_service?sslmode=disable"),
		LogLevel:       getEnv("LOG_LEVEL", "info"),
		RedisURL:       getEnv("REDIS_URL", "redis://localhost:6379"),
		PublicKeyPath:  getEnv("PUBLIC_KEY_PATH", "./keys/public.pem"),
		PrivateKeyPath: getEnv("PRIVATE_KEY_PATH", "./keys/private.pem"),
	}
}

func getEnv(key string, defaultVal string) string {
	if value := os.Getenv(key); value != "" {
		return value
	}

	return defaultVal
}
