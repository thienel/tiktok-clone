#!/bin/bash

# TikTok Clone Backend Development Scripts

case "$1" in
  "auth")
    echo "🔐 Starting Auth Service..."
    cd auth-service
    dotnet run
    ;;
  "video")
    echo "🎥 Starting Video Service..."
    cd video-service
    go run cmd/main.go
    ;;
  "install-auth")
    echo "📦 Installing Auth Service dependencies..."
    cd auth-service
    dotnet restore
    ;;
  "install-video")
    echo "📦 Installing Video Service dependencies..."
    cd video-service
    go mod tidy
    ;;
  "build-auth")
    echo "🔨 Building Auth Service..."
    cd auth-service
    dotnet build
    ;;
  "build-video")
    echo "🔨 Building Video Service..."
    cd video-service
    go build -o bin/video-service cmd/main.go
    ;;
  "test-auth")
    echo "🧪 Running Auth Service tests..."
    cd auth-service
    dotnet test
    ;;
  "test-video")
    echo "🧪 Running Video Service tests..."
    cd video-service
    go test ./...
    ;;
  "clean")
    echo "🧹 Cleaning build artifacts..."
    cd auth-service
    dotnet clean
    cd ../video-service
    go clean
    rm -rf bin/
    ;;
  *)
    echo "TikTok Clone Backend Development Commands:"
    echo ""
    echo "Service Management:"
    echo "  ./dev.sh auth          - Start auth service"
    echo "  ./dev.sh video         - Start video service"
    echo ""
    echo "Dependencies:"
    echo "  ./dev.sh install-auth  - Install auth service dependencies"
    echo "  ./dev.sh install-video - Install video service dependencies"
    echo ""
    echo "Build:"
    echo "  ./dev.sh build-auth    - Build auth service"
    echo "  ./dev.sh build-video   - Build video service"
    echo ""
    echo "Testing:"
    echo "  ./dev.sh test-auth     - Run auth service tests"
    echo "  ./dev.sh test-video    - Run video service tests"
    echo ""
    echo "Maintenance:"
    echo "  ./dev.sh clean         - Clean build artifacts"
    ;;
esac
