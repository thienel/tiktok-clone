package services

import (
	"auth-service/internal/domain/entities"
	"auth-service/internal/domain/repositories"
	"auth-service/internal/errors/apperrors"
	"auth-service/internal/security"
	"context"
)

const (
	loginTypeEmail    = "email"
	loginTypeUsername = "username"
)

type AuthService interface {
	Login(ctx context.Context, usernameOrEmail, password string) (string, string, error)
}

type authService struct {
	userRepo     repositories.UserRepository
	tokenService TokenService
}

func NewAuthService(userRepo repositories.UserRepository, tokenService TokenService) AuthService {
	return &authService{userRepo, tokenService}
}

func (as *authService) Login(ctx context.Context, usernameOrEmail, password string) (string, string, error) {
	loginType := identifyLoginType(usernameOrEmail)
	var user *entities.User
	switch loginType {
	case loginTypeEmail:
		foundUser, err := as.userRepo.FindByEmail(ctx, usernameOrEmail)
		user = foundUser
		if err != nil {
			return "", "", err
		}
	case loginTypeUsername:
		foundUser, err := as.userRepo.FindByUsername(ctx, usernameOrEmail)
		user = foundUser
		if err != nil {
			return "", "", err
		}
	default:
		return "", "", apperrors.ErrInvalidCredentials
	}

	if user == nil {
		return "", "", apperrors.ErrInvalidCredentials
	}
	if !user.IsActive() {
		return "", "", apperrors.ErrUserInactive
	}

	if !security.VerifyPassword(password, user.PasswordHash) {
		return "", "", apperrors.ErrInvalidCredentials
	}

	accessToken, err := as.tokenService.GenerateAccessToken(ctx, user.ID)
	if err != nil {
		return "", "", err
	}
	refreshToken, err := as.tokenService.GenerateRefreshToken(ctx, user.ID)
	if err != nil {
		return "", "", err
	}

	return accessToken, refreshToken, nil
}

func identifyLoginType(usernameOrEmail string) string {
	if entities.IsValidEmail(usernameOrEmail) {
		return loginTypeEmail
	}
	if entities.IsValidUserName(usernameOrEmail) {
		return loginTypeUsername
	}
	return ""
}
