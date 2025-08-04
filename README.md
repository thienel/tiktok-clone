# TikTok Clone Backend Monorepo

This repository contains the backend services for the TikTok Clone application.

## Services

### 🔐 Auth Service
Location: `./auth-service/`

.NET Core authentication and user management service.

**Features:**
- User registration and authentication
- JWT token management
- Email verification
- Password reset functionality

**Tech Stack:**
- .NET 8
- Entity Framework Core
- SQL Server
- JWT Authentication

### 🎥 Video Service
Location: `./video-service/`

Go-based video processing and management service.

**Features:**
- Video upload and processing
- Video metadata management
- Video streaming
- Video recommendations

**Tech Stack:**
- Go
- gRPC
- Protocol Buffers

## Getting Started

### Prerequisites
- .NET 8 SDK
- Go 1.21+
- Docker (optional)

### Running the Services

#### Auth Service
```bash
cd auth-service
dotnet restore
dotnet run
```

#### Video Service
```bash
cd video-service
go mod tidy
go run cmd/main.go
```

## Development

Each service maintains its own development workflow while being part of the monorepo structure.

## Architecture

```
backend/
├── auth-service/          # .NET authentication service
├── video-service/         # Go video processing service
├── docker-compose.yml     # Multi-service deployment
└── README.md             # This file
```
