# TikTok Clone Backend Monorepo

Microservices backend for TikTok Clone application built with Go and C#.

## Service Overview

### Auth Service (C# .NET)
- **Location**: `./auth-service/`
- **Purpose**: User authentication, registration, and authorization
- **Technology**: ASP.NET Core, Entity Framework Core
- **Database**: PostgreSQL
- **Features**: JWT tokens, email verification, password reset

### Auth Service Golang (In Development)
- **Location**: `./auth-service-golang/`
- **Purpose**: Go-based replacement for C# auth service
- **Technology**: Go, JWT, bcrypt
- **Status**: Under development

### Video Service (In Development)
- **Location**: `./video-service/`
- **Purpose**: Video upload, processing, and management
- **Technology**: Go, gRPC, PostgreSQL
- **Status**: Under development

## Communication
- User registration and authentication
- **Inter-service**: gRPC for high-performance communication
- **Client-server**: REST APIs with JSON
- **Authentication**: JWT tokens across all services
- Email verification
## Database

- **Primary**: PostgreSQL and SQL Server
- **Schema**: Each service manages its own database schema
- **Migrations**: Service-specific migration scripts

## Development Setup

1. Install dependencies for each service
2. Configure environment variables
3. Run database migrations
4. Start services individually

See individual service README files for detailed setup instructions.
- Password reset functionality

**Tech Stack:**
- .NET 8
- Entity Framework Core
- SQL Server
- JWT Authentication

###  Video Service
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

