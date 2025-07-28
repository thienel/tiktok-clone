# 🎬 TikTok Clone - Backend API

A modern, scalable backend API for a TikTok-like social media platform built with .NET 9, following Clean Architecture principles and best practices.

## 🚧 Project Status

**Current Version**: v0.5 (Foundation Phase)

### ✅ **Completed Features**
- User Authentication & Authorization (JWT-based)
- User Registration, Login, Password Reset
- Email Verification System
- User Profile Management & Search
- Clean Architecture Implementation
- Database Structure & Migrations
- Basic Firebase Service Configuration

### 🚧 **In Development**
- Video Upload & Management System
- Complete Firebase/Cloud Storage Integration

### ❌ **Not Yet Implemented**
- Video Controller & Endpoints
- Video Streaming Capabilities
- Like/Unlike Functionality
- Comment System
- Testing Infrastructure
- Push Notifications

> **Note**: This project is actively under development. The core authentication and user management features are complete and functional, while video-related features are currently being implemented.

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Technology Stack](#-technology-stack)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [Database Setup](#-database-setup)
- [API Documentation](#-api-documentation)
- [Project Structure](#-project-structure)
- [Testing](#-testing)
- [Security](#-security)
- [Deployment](#-deployment)
- [Contributing](#-contributing)
- [License](#-license)

## ✨ Features

### 🔐 Authentication & Authorization ✅
- **JWT-based authentication** with refresh tokens
- **Email verification** flow with verification codes
- **Password reset** functionality
- **Rate limiting** for auth endpoints
- **Account registration** and login

### 👤 User Management ✅
- **User registration** and profile management
- **Username validation** and availability checking
- **Birthdate validation** for age verification
- **User search** functionality
- **Profile retrieval** for current user

### 🎥 Video Features 🚧 (In Development)
- **Video entity** structure defined
- **Video service interface** created
- **Firebase integration** for file storage (configured but not fully implemented)
- ❌ **Video upload** endpoint (not yet implemented)
- ❌ **Video streaming** (not yet implemented)
- ❌ **Like/Unlike** functionality (not yet implemented)
- ❌ **Comment system** (not yet implemented)

### 🛡️ Security & Performance ✅
- **Global exception handling**
- **Input validation** and sanitization
- **Rate limiting** middleware
- **CORS configuration**
- **Structured logging** with dependency injection

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────┐
│   Presentation      │  ← Controllers, Middleware
│   (TikTokClone.API) │
└─────────────────────┘
           │
┌─────────────────────┐
│    Application      │  ← Services, DTOs, Interfaces
│(TikTokClone.App)    │
└─────────────────────┘
           │
┌─────────────────────┐
│     Domain          │  ← Entities, Domain Logic
│(TikTokClone.Domain) │
└─────────────────────┘
           │
┌─────────────────────┐
│  Infrastructure     │  ← Data Access, External Services
│(TikTokClone.Infra)  │
└─────────────────────┘
```

### Layer Responsibilities

- **🎯 Domain Layer**: Core business entities, domain events, and business rules
- **⚙️ Application Layer**: Use cases, application services, DTOs, and interfaces
- **🌐 Infrastructure Layer**: Data access, external services, and technical concerns
- **📱 Presentation Layer**: API controllers, middleware, and HTTP concerns

## 🛠️ Technology Stack

### Core Framework
- **.NET 9** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for data access
- **ASP.NET Core Identity** - Authentication and authorization

### Database
- **SQL Server** - Primary database
- **Entity Framework Core** - Code-first migrations

### Authentication & Security ✅
- **JWT Bearer Tokens** - Authentication
- **BCrypt** - Password hashing
- **Rate Limiting** - API protection

### Cloud Services 🚧 (Configured but Not Implemented)
- **Firebase** - File storage service (interface and basic setup ready)
- ❌ **Push notifications** (not implemented)
- **SMTP** - Email services (configured for email verification)

### Testing ❌ (Not Implemented)
- ❌ **xUnit** - Testing framework (not set up)
- ❌ **Moq** - Mocking framework (not implemented)
- ❌ **FluentAssertions** - Assertion library (not implemented)
- ❌ **Test projects** (no test projects created yet)

### Development Tools
- **OpenAPI/Swagger** - API documentation
- **Serilog** - Structured logging
- **AutoMapper** - Object mapping

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **[.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)** (9.0 or later)
- **[SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)** (LocalDB for development)
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** or **[VS Code](https://code.visualstudio.com/)**
- **[Git](https://git-scm.com/)**

### Optional but Recommended
- **[SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/)**
- **[Postman](https://www.postman.com/)** or **[Insomnia](https://insomnia.rest/)** for API testing
- **[Docker](https://www.docker.com/)** for containerized deployment

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/thienel/tiktok-clone-api.git
cd tiktok-clone-api/backend
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Set Up Configuration

Copy the configuration template and update with your settings:

```bash
cp TikTokClone.API/appsettings.Development.json.template TikTokClone.API/appsettings.Development.json
```

Edit `appsettings.Development.json` with your specific configuration values.

### 4. Update Database

Run Entity Framework migrations to set up the database:

```bash
dotnet ef database update --project TikTokClone.Infrastructure --startup-project TikTokClone.API
```

### 5. Run the Application

```bash
cd TikTokClone.API
dotnet run
```

The API will be available at:
- **HTTPS**: `https://localhost:7001`
- **HTTP**: `http://localhost:5001`
- **Swagger UI**: `https://localhost:7001/swagger`

## ⚙️ Configuration

### appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TikTokCloneDb;Trusted_Connection=true;"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-jwt-key-32-chars-min",
    "Issuer": "TikTokClone.API",
    "Audience": "TikTokClone.Client",
    "ExpiryInMinutes": 60,
    "RefreshTokenExpiryInDays": 7
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@gmail.com",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  },
  "GoogleCloudSettings": {
    "ProjectId": "your-firebase-project-id",
    "ServiceAccountKeyPath": "path/to/service-account.json",
    "StorageBucketName": "your-storage-bucket"
  }
}
```

**Note**: Firebase/Google Cloud integration is configured but not fully implemented. Video upload and cloud storage features are planned for future development.
```

### Environment Variables

For production, use environment variables:

```bash
export ConnectionStrings__DefaultConnection="your-production-connection-string"
export JwtSettings__SecretKey="your-production-jwt-secret"
export EmailSettings__Password="your-email-password"
```

## 🗄️ Database Setup

### Using Entity Framework Migrations

1. **Add Migration** (when making schema changes):
```bash
dotnet ef migrations add MigrationName --project TikTokClone.Infrastructure --startup-project TikTokClone.API
```

2. **Update Database**:
```bash
dotnet ef database update --project TikTokClone.Infrastructure --startup-project TikTokClone.API
```

3. **Remove Last Migration** (if needed):
```bash
dotnet ef migrations remove --project TikTokClone.Infrastructure --startup-project TikTokClone.API
```

### Database Schema

The application uses the following main entities:

- **Users** ✅ - User accounts and profiles (fully implemented)
- **RefreshTokens** ✅ - JWT refresh token management (fully implemented)
- **EmailCodes** ✅ - Email verification codes (fully implemented)
- **Videos** 🚧 - Video content and metadata (entity defined, implementation pending)

## 📚 API Documentation

### Swagger/OpenAPI

When running in development mode, navigate to:
- **Swagger UI**: `https://localhost:7001/swagger`

### Authentication Endpoints ✅

| Method | Endpoint | Description | Status |
|--------|----------|-------------|---------|
| POST | `/api/auth/register` | Register new user | ✅ Implemented |
| POST | `/api/auth/login` | User login | ✅ Implemented |
| POST | `/api/auth/refresh` | Refresh JWT token | ✅ Implemented |
| POST | `/api/auth/reset-password` | Reset password | ✅ Implemented |
| POST | `/api/auth/logout` | User logout | ✅ Implemented |
| POST | `/api/auth/send-verification-code` | Send email verification | ✅ Implemented |

### User Endpoints ✅

| Method | Endpoint | Description | Status |
|--------|----------|-------------|---------|
| GET | `/api/users/me` | Get current user profile | ✅ Implemented |
| POST | `/api/users/check-username` | Check username availability | ✅ Implemented |
| POST | `/api/users/change-username` | Change username | ✅ Implemented |
| POST | `/api/users/check-birthdate` | Validate birthdate | ✅ Implemented |
| POST | `/api/users/search` | Search users | ✅ Implemented |

### Video Endpoints ❌ (Not Implemented)

| Method | Endpoint | Description | Status |
|--------|----------|-------------|---------|
| GET | `/api/videos` | Get videos feed | ❌ Not implemented |
| POST | `/api/videos` | Upload new video | ❌ Not implemented |
| GET | `/api/videos/{id}` | Get video by ID | ❌ Not implemented |
| PUT | `/api/videos/{id}` | Update video | ❌ Not implemented |
| DELETE | `/api/videos/{id}` | Delete video | ❌ Not implemented |
| POST | `/api/videos/{id}/like` | Like/unlike video | ❌ Not implemented |

## 📁 Project Structure

```
TikTokClone.Backend/
├── TikTokClone.API/                 # 🌐 Presentation Layer
│   ├── Controllers/                 # API Controllers
│   ├── Middleware/                  # Custom Middleware
│   ├── Properties/                  # Launch settings
│   └── Program.cs                   # Application entry point
│
├── TikTokClone.Application/         # ⚙️ Application Layer
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Interfaces/                  # Application interfaces
│   ├── Services/                    # Application services (Auth, User)
│   ├── Constants/                   # Application constants
│   ├── Exceptions/                  # Application exceptions
│   └── Mappers/                     # Object mapping
│
├── TikTokClone.Domain/              # 🎯 Domain Layer
│   ├── Entities/                    # Domain entities (User, Video, etc.)
│   ├── Events/                      # Domain events
│   └── Exceptions/                  # Domain exceptions
│
└── TikTokClone.Infrastructure/      # 🔧 Infrastructure Layer
    ├── Data/                        # DbContext and configurations
    ├── Repositories/                # Data repositories
    ├── Services/                    # External services (Email, Firebase, Token)
    ├── Migrations/                  # EF Core migrations
    └── Settings/                    # Configuration settings

```


## 🛡️ Security

### Authentication Flow

1. **Registration**: User registers with email and password
2. **Email Verification**: User verifies email address
3. **Login**: User logs in with credentials
4. **JWT Token**: Server returns access and refresh tokens
5. **API Access**: Client uses access token for API calls
6. **Token Refresh**: Client refreshes tokens when expired

### Security Features

- **Password Hashing**: BCrypt with salt
- **JWT Tokens**: Short-lived access tokens
- **Refresh Tokens**: Secure token rotation
- **Rate Limiting**: Prevent brute force attacks
- **Input Validation**: Comprehensive validation
- **CORS**: Configured for frontend domains
- **HTTPS**: Enforced in production

### Best Practices Implemented

- ✅ Principle of least privilege
- ✅ Input sanitization and validation
- ✅ Secure password policies
- ✅ Token expiration and rotation
- ✅ Error message sanitization
- ✅ Audit logging
- ✅ Rate limiting and throttling

## 🚀 Deployment

### Docker Deployment

1. **Build Docker Image**:
```bash
docker build -t tiktok-clone-api .
```

2. **Run Container**:
```bash
docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Production tiktok-clone-api
```

### Production Configuration

1. **Environment Variables**: Set all sensitive configuration via environment variables
2. **Database**: Use production SQL Server instance
3. **Logging**: Configure structured logging (Serilog)
4. **Monitoring**: Implement health checks and monitoring
5. **SSL**: Use valid SSL certificates
6. **Scaling**: Consider load balancing for multiple instances

### Cloud Deployment Options

- **Azure App Service**: Easy .NET hosting
- **AWS Elastic Beanstalk**: Managed platform
- **Google Cloud Run**: Containerized deployment
- **Docker**: Self-hosted container deployment

## 🏗️ Development Workflow

### Adding New Features

1. **Domain First**: Start with domain entities and business logic
2. **Application Layer**: Add DTOs, interfaces, and services
3. **Infrastructure**: Implement data access and external services
4. **API Layer**: Create controllers and configure routing
5. **Tests**: Write comprehensive tests for all layers

### Code Quality

- **Code Reviews**: All changes require review
- **Linting**: Follow C# coding standards
- **Testing**: Maintain high test coverage
- **Documentation**: Update docs with changes

## 🚀 Roadmap

### ✅ Current Version (v0.5) - Foundation Complete
- ✅ User authentication and authorization (JWT)
- ✅ Basic user profile management
- ✅ Email verification system
- ✅ Password reset functionality
- ✅ User search and validation
- ✅ Clean Architecture implementation
- ✅ Database structure and migrations
- ✅ Basic Firebase service setup

### 🚧 Next Version (v0.8) - Video Infrastructure
- 📋 Complete video upload implementation
- 📋 Video controller and endpoints
- 📋 Video streaming capabilities
- 📋 File storage integration with Firebase
- 📋 Video metadata management

### 📋 Future Versions
- **v1.0**: Core video features (upload, view, basic interactions)
- **v1.1**: Like/unlike functionality and basic engagement
- **v1.2**: Comment system and replies
- **v1.3**: Follow/unfollow functionality
- **v1.4**: Testing infrastructure and comprehensive test coverage
- **v1.5**: Push notifications and real-time features
- **v2.0**: Advanced features (recommendations, analytics, live streaming)

### 🎯 Areas for Development
- **Testing**: Comprehensive unit, integration, and API tests
- **Video Features**: Complete video upload and streaming functionality
- **Cloud Integration**: Full Firebase/cloud storage implementation
- **Performance**: Caching, optimization, and scalability improvements
- **Security**: Enhanced security features and audit logging

---

**Made with ❤️ by [thienel](https://github.com/thienel)**
_This project is intended solely for learning and experimental purposes._
