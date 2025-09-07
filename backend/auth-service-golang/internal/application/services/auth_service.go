package services

import (
	"auth-service/internal/domain/entities"
	"auth-service/internal/domain/repositories"
	"auth-service/internal/errors/apperrors"
	"auth-service/internal/security"
	"context"
	"strings"

	"github.com/google/uuid"
)

const (
	loginTypeEmail    = "email"
	loginTypeUsername = "username"
)

type AuthService interface {
	Login(ctx context.Context, usernameOrEmail, password string) (string, string, error)
	Register(ctx context.Context, username, email, password string) (*entities.User, error)
	Logout(ctx context.Context, token string) error
	GetUserByID(ctx context.Context, id uuid.UUID) (*entities.User, error)
	GetUserByUsername(ctx context.Context, username string) (*entities.User, error)
}

type authService struct {
	userRepo     repositories.UserRepository
	tokenService TokenService
}

func NewAuthService(userRepo repositories.UserRepository, tokenService TokenService) AuthService {
	return &authService{userRepo, tokenService}
}

func (as *authService) Login(ctx context.Context, usernameOrEmail, password string) (string, string, error) {
	usernameOrEmail = strings.TrimSpace(usernameOrEmail)
	loginType := as.identifyLoginType(usernameOrEmail)
	user, err := as.findUser(ctx, loginType, usernameOrEmail)
	if err != nil {
		return "", "", err
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

func (as *authService) identifyLoginType(usernameOrEmail string) string {
	if entities.IsValidEmail(usernameOrEmail) {
		return loginTypeEmail
	}
	if entities.IsValidUserName(usernameOrEmail) {
		return loginTypeUsername
	}
	return ""
}

func (as *authService) findUser(ctx context.Context, loginType, value string) (*entities.User, error) {
	switch loginType {
	case loginTypeEmail:
		return as.userRepo.FindByEmail(ctx, value)
	case loginTypeUsername:
		return as.userRepo.FindByUsername(ctx, value)
	default:
		return nil, apperrors.ErrInvalidCredentials
	}
}

func (as *authService) GetUserByID(ctx context.Context, id uuid.UUID) (*entities.User, error) {
	return as.userRepo.FindByID(ctx, id)
}

func (as *authService) GetUserByUsername(ctx context.Context, username string) (*entities.User, error) {
	if !entities.IsValidUserName(username) {
		return nil, apperrors.ErrInvalidCredentials
	}
	return as.userRepo.FindByUsername(ctx, username)
}

func (as *authService) Register(ctx context.Context, username, email, password string) (*entities.User, error) {
	email = strings.TrimSpace(email)
	if !entities.IsValidEmail(email) {
		return nil, apperrors.ErrInvalidCredentials
	}
	username = strings.TrimSpace(username)
	if !entities.IsValidUserName(username) {
		return nil, apperrors.ErrInvalidCredentials
	}
	if !entities.IsValidPassword(password) {
		return nil, apperrors.ErrInvalidPassword
	}
	passwordHash, err := security.HashPassword(password)
	if err != nil {
		return nil, apperrors.ErrHashPassword(err)
	}

	user := entities.NewUser(username, email, passwordHash)

	if err = as.userRepo.Create(ctx, user); err != nil {
		return nil, err
	}

	return user, nil
}

func (as *authService) Logout(ctx context.Context, token string) error {
	err := as.tokenService.RevokeRefreshToken(ctx, token)
	if err != nil {
		return err
	}
	return nil
}
