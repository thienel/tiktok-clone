# TikTok Clone

A full-stack TikTok clone application with microservices architecture.

## Project Structure

```
├── backend/                 # Backend services
│   ├── auth-service/        # C# .NET authentication service
│   └── video-service/       # Go video management service
└── frontend/                # React frontend application
```

## Backend Services

### Auth Service (C# .NET)
- **Technology**: ASP.NET Core, Entity Framework Core
- **Database**: PostgreSQL
- **Features**: User authentication, JWT tokens, email verification
- **Location**: `backend/auth-service/`

### Video Service (Go)
- **Technology**: Go, gRPC, PostgreSQL
- **Features**: Video upload, likes, views, comments
- **Location**: `backend/video-service/`

## Frontend Application

### React App
- **Technology**: React, JavaScript, CSS
- **Features**: Video feed, user profiles, authentication UI
- **Location**: `frontend/`

## Getting Started

### Prerequisites
- .NET 6.0 or later
- Go 1.19 or later
- Node.js 16 or later
- PostgreSQL
- Docker (optional)

### Backend Setup

#### Auth Service
```bash
cd backend/auth-service
dotnet restore
dotnet run --project TikTokClone.API
```

#### Video Service
```bash
cd backend/video-service
go mod tidy
go run cmd/main.go
```

### Frontend Setup
```bash
cd frontend
npm install
npm start
```

## Development

Each service can be developed and deployed independently:
- Auth Service runs on port 5000
- Video Service runs on port 8080  
- Frontend runs on port 3000
