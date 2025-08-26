package config

import "os"

type Config struct {
	Database DatabaseConfig
	Server   ServerConfig
	Kafka    KafkaConfig
}

type DatabaseConfig struct {
	Host     string
	Port     string
	User     string
	Password string
	DBName   string
	SSLMode  string
}

type ServerConfig struct {
	GRPCPort string
	Host     string
}

type KafkaConfig struct {
	Brokers []string
	Topic   string
}

func LoadConfig() (*Config, error) {
	return &Config{
		Database: DatabaseConfig{
			Host:     os.Getenv("DB_HOST"),
			Port:     os.Getenv("DB_PORT"),
			User:     os.Getenv("DB_USER"),
			Password: os.Getenv("DB_PASSWORD"),
			DBName:   os.Getenv("DB_NAME"),
			SSLMode:  os.Getenv("DB_SSLMODE"),
		},
		Server: ServerConfig{
			GRPCPort: os.Getenv("GRPC_PORT"),
			Host:     os.Getenv("SERVER_HOST"),
		},
		Kafka: KafkaConfig{
			Brokers: []string{os.Getenv("KAFKA_BROKERS")},
			Topic:   os.Getenv("KAFKA_TOPIC"),
		},
	}, nil
}
