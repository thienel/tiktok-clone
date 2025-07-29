# TikTok Clone - Backend API

A backend API for a TikTok-like social media platform built with .NET 9, following Clean Architecture principles.

## Project Status

**Current Version**: v0.5 (Authentication & User Management Complete)

### Completed Features
- JWT-based authentication and authorization
- User registration, login, and password reset
- Email verification system
- User profile management and search
- Clean Architecture implementation
- Database structure with Entity Framework migrations
- Global exception handling and rate limiting

### Not Yet Implemented
- Video upload and management
- Video streaming
- Firebase/Cloud storage integration
- Like/unlike functionality
- Comment system
- Testing infrastructure

> **Note**: This project focuses on authentication and user management. Video functionality is planned but not implemented.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Security](#security)

## Features

### Authentication & Authorization
- JWT-based authentication with access and refresh tokens
- User registration with email verification
- Password reset functionality
- Rate limiting for security
- Secure password hashing with BCrypt

### User Management
- User profile creation and management
- Username validation and availability checking
- Age verification through birthdate validation
- User search functionality
- Account management endpoints

### Security & Infrastructure
- Global exception handling middleware
- Rate limiting middleware
- CORS configuration
- Input validation and sanitization
- Structured logging with dependency injection
- Clean Architecture with clear separation of concerns

### Database & Data Access
- Entity Framework Core with SQL Server
- Code-first migrations
- Repository pattern implementation
- Proper entity relationships and constraints

**Note**: Video functionality, Firebase integration, and cloud storage features are not implemented. The project currently provides a complete authentication and user management system.

## Architecture

This project follows Clean Architecture principles with clear separation of concerns:

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

- **Domain Layer**: Core business entities (User, Video, RefreshToken, EmailCode)
- **Application Layer**: Business logic, DTOs, and service interfaces
- **Infrastructure Layer**: Data access, external services, and technical implementations
- **Presentation Layer**: API controllers, middleware, and HTTP handling

## Technology Stack

### Core Framework
- .NET 9 - Latest .NET framework
- ASP.NET Core - Web API framework
- Entity Framework Core - ORM with code-first migrations
- ASP.NET Core Identity - Authentication framework

### Database
- SQL Server - Primary database
- Entity Framework Core - Database access and migrations

### Authentication & Security
- JWT Bearer Tokens - Authentication
- BCrypt - Password hashing
- Rate Limiting - API protection
- CORS - Cross-origin resource sharing

### Development Tools
- OpenAPI/Swagger - API documentation
- Serilog - Structured logging
- AutoMapper - Object mapping

### Not Implemented
- Firebase/Cloud Storage integration (interfaces defined but not implemented)
- Testing framework (no test projects exist)
- Push notifications

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB for development)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Optional
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/)
- [Postman](https://www.postman.com/) for API testing

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/thienel/tiktok-clone-api.git
cd tiktok-clone-api/backend
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Settings

Copy the template and update with your settings:

```bash
cp TikTokClone.API/appsettings.Development.json.template TikTokClone.API/appsettings.Development.json
```

### 4. Set Up Database

```bash
dotnet ef database update --project TikTokClone.Infrastructure --startup-project TikTokClone.API
```

### 5. Run the Application

```bash
cd TikTokClone.API
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5001`
- Swagger UI: `https://localhost:7001/swagger`

## Configuration

### Required Settings

Update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TikTokCloneDb;Trusted_Connection=true;"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-32-characters-minimum",
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
  }
}
```

### Environment Variables (Production)

```bash
export ConnectionStrings__DefaultConnection="your-production-connection-string"
export JwtSettings__SecretKey="your-production-jwt-secret"
export EmailSettings__Password="your-email-password"
```

## API Documentation

### Swagger/OpenAPI

Access the interactive API documentation at: `https://localhost:7001/swagger`

### Implemented Endpoints

#### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | User login |
| POST | `/api/auth/refresh` | Refresh JWT token |
| POST | `/api/auth/reset-password` | Reset password |
| POST | `/api/auth/logout` | User logout |
| POST | `/api/auth/send-verification-code` | Send email verification |

#### User Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users/me` | Get current user profile |
| POST | `/api/users/check-username` | Check username availability |
| POST | `/api/users/change-username` | Change username |
| POST | `/api/users/check-birthdate` | Validate birthdate |
| POST | `/api/users/search` | Search users |

### Not Implemented
- Video endpoints (upload, streaming, management)
- Like/unlike functionality
- Comment system
- Follow/unfollow features


## Security

### Authentication System
- JWT-based authentication with access and refresh tokens
- Email verification for account activation
- Password reset functionality with secure tokens
- BCrypt password hashing with salt
- Rate limiting to prevent brute force attacks

### Security Features
- Input validation and sanitization
- CORS configuration for frontend integration
- Global exception handling middleware
- Secure token storage and rotation
- HTTPS enforcement in production

### Best Practices
- Principle of least privilege
- Secure password policies and validation
- Token expiration and automatic rotation
- Comprehensive error handling without information leakage
- Structured logging for security monitoring

---

**Made by [thienel](https://github.com/thienel)**  
*This project is for learning and experimental purposes.*
