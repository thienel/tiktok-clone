package logger

type Config struct {
	Level       string   `json:"level"`
	Environment string   `json:"environment"`
	OutputPaths []string `json:"output_paths"`
}

func NewProductionConfig() *Config {
	return &Config{
		Level:       "info",
		Environment: "production",
		OutputPaths: []string{"stdout"},
	}
}

func NewDevelopmentConfig() *Config {
	return &Config{
		Level:       "debug",
		Environment: "development",
		OutputPaths: []string{"stdout"},
	}
}
