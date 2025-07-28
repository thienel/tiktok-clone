# 🎬 TikTok Clone - Backend API

A modern, scalable backend API for a TikTok-like social media platform built with .NET 9, following Clean Architecture principles and best practices.

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

### 🔐 Authentication & Authorization
- **JWT-based authentication** with refresh tokens
- **Email verification** flow
- **Password reset** functionality
- **Account lockout** after failed login attempts
- **Rate limiting** for auth endpoints
- **Social media login** support (Google, Facebook)

### 👤 User Management
- **User registration** and profile management
- **Avatar upload** and management
- **Bio and profile customization**
- **Account verification** system
- **User search** and discovery

### 🎥 Video Features
- **Video upload** and processing
- **Video metadata** management
- **Video streaming** support
- **Like/Unlike** functionality
- **Comment system**
- **Video sharing** capabilities

### 🛡️ Security & Performance
- **Global exception handling**
- **Input validation** and sanitization
- **Rate limiting** middleware
- **CORS configuration**
- **Request/Response logging**
- **Health checks**

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

### Authentication & Security
- **JWT Bearer Tokens** - Authentication
- **ASP.NET Core Identity** - User management
- **BCrypt** - Password hashing
- **Rate Limiting** - API protection

### Cloud Services
- **Google Cloud Storage** - File storage
- **Firebase** - Push notifications
- **SendGrid/SMTP** - Email services

### Testing
- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library
- **AutoFixture** - Test data generation
- **Entity Framework InMemory** - Database testing

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

- **Users** - User accounts and profiles
- **Videos** - Video content and metadata
- **RefreshTokens** - JWT refresh token management
- **EmailCodes** - Email verification codes

## 📚 API Documentation

### Swagger/OpenAPI

When running in development mode, navigate to:
- **Swagger UI**: `https://localhost:7001/swagger`

### Authentication Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | User login |
| POST | `/api/auth/refresh` | Refresh JWT token |
| POST | `/api/auth/verify-email` | Verify email address |
| POST | `/api/auth/forgot-password` | Request password reset |
| POST | `/api/auth/reset-password` | Reset password |
| POST | `/api/auth/logout` | User logout |

### User Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users/me` | Get current user profile |
| PUT | `/api/users/me` | Update user profile |
| POST | `/api/users/upload-avatar` | Upload user avatar |
| GET | `/api/users/{id}` | Get user by ID |
| GET | `/api/users/search` | Search users |

### Video Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/videos` | Get videos feed |
| POST | `/api/videos` | Upload new video |
| GET | `/api/videos/{id}` | Get video by ID |
| PUT | `/api/videos/{id}` | Update video |
| DELETE | `/api/videos/{id}` | Delete video |
| POST | `/api/videos/{id}/like` | Like/unlike video |

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
│   ├── Services/                    # Application services
│   ├── Constants/                   # Application constants
│   ├── Exceptions/                  # Application exceptions
│   └── Mappers/                     # Object mapping
│
├── TikTokClone.Domain/              # 🎯 Domain Layer
│   ├── Entities/                    # Domain entities
│   ├── Events/                      # Domain events
│   └── Exceptions/                  # Domain exceptions
│
├── TikTokClone.Infrastructure/      # 🔧 Infrastructure Layer
│   ├── Data/                        # DbContext and configurations
│   ├── Repositories/                # Data repositories
│   ├── Services/                    # External services
│   ├── Migrations/                  # EF Core migrations
│   └── Settings/                    # Configuration settings
│
└── TikTokClone.Tests/               # 🧪 Test Layer
    ├── Domain/                      # Domain layer tests
    ├── Application/                 # Application layer tests
    ├── Infrastructure/              # Infrastructure tests
    └── API/                         # API integration tests
```

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests in watch mode
dotnet watch test
```

### Test Structure

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test component interactions
- **API Tests**: Test HTTP endpoints end-to-end

### Test Categories

- **Domain Tests**: Business logic validation
- **Service Tests**: Application service testing
- **Repository Tests**: Data access testing
- **Controller Tests**: API endpoint testing

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

## 🤝 Contributing

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/AmazingFeature`)
3. **Commit** your changes (`git commit -m 'Add some AmazingFeature'`)
4. **Push** to the branch (`git push origin feature/AmazingFeature`)
5. **Open** a Pull Request

### Development Guidelines

- Follow Clean Architecture principles
- Write comprehensive tests
- Use meaningful commit messages
- Update documentation
- Follow C# coding conventions

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

If you encounter any issues or have questions:

1. **Check** the [Issues](https://github.com/thienel/tiktok-clone-api/issues) page
2. **Create** a new issue if your problem isn't already reported
3. **Provide** detailed information about the issue
4. **Include** steps to reproduce the problem

## 🚀 Roadmap

### Current Version (v1.0)
- ✅ User authentication and authorization
- ✅ Basic user profile management
- ✅ Video upload and management
- ✅ Like/unlike functionality

### Future Versions
- 📋 **v1.1**: Comment system and replies
- 📋 **v1.2**: Follow/unfollow functionality
- 📋 **v1.3**: Real-time notifications
- 📋 **v1.4**: Video recommendations algorithm
- 📋 **v1.5**: Live streaming capability
- 📋 **v2.0**: Advanced analytics and reporting

---

**Built with ❤️ using .NET 9 and Clean Architecture**
