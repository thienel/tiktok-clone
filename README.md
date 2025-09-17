# TikTok Clone

A full-stack TikTok clone application with microservices architecture.

## Architecture

- **Frontend**: React web application
- **Backend**: Microservices architecture with Go and C#
- **Database**: PostgreSQL
- **Authentication**: JWT-based auth service

## Project Structure

- `auth-service`: User authentication and authorization (C# .NET)
- `auth-service-golang`: Go-based auth service replacement (in development)
- `video-service`: Video upload, processing, and management (in development)
- `frontend`: React web client with video feed and user interface

## Technology Stack

### Backend
- C# .NET 9 (Auth Service)
- Go 1.19+ (Video Service, Auth Service v2)
- PostgreSQL database, SQL Server
- JWT authentication
- gRPC communication

### Frontend
- React 18
- JavaScript/CSS
- Responsive design



## Development

Each service contains its own README with detailed setup instructions, API documentation, and development guidelines.
- **Features**: Video upload, likes, views, comments
- **Location**: `backend/video-service/`


## Getting Started


### Backend Setup

#### Auth Service
```bash
cd backend/auth-service
dotnet restore
dotnet run --project TikTokClone.API
````


### Frontend Setup
```bash
cd frontend
npm install
npm start
```
