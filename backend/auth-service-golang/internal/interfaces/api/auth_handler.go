package api

import (
	"auth-service/internal/application/services"
	"auth-service/internal/errors/apperrors"
	"auth-service/internal/interfaces/api/dtos"
	"auth-service/pkg/logger"
	"context"
	"encoding/json"
	"errors"
	"net/http"
	"strings"
	"time"

	"github.com/go-playground/validator/v10"
	"github.com/google/uuid"
)

const (
	requestTimeout = 30 * time.Second
	bearerPrefix   = "Bearer "
)

type AuthHandler interface {
	Login(w http.ResponseWriter, r *http.Request)
	Register(w http.ResponseWriter, r *http.Request)
	Logout(w http.ResponseWriter, r *http.Request)
	RefreshToken(w http.ResponseWriter, r *http.Request)
	ValidateToken(w http.ResponseWriter, r *http.Request)
}

type authHandler struct {
	authService  services.AuthService
	tokenService services.TokenService
	validator    *validator.Validate
	logger       logger.Logger
}

func NewAuthHandler(authService services.AuthService, tokenService services.TokenService, validator *validator.Validate, logger logger.Logger) AuthHandler {
	return &authHandler{
		authService:  authService,
		tokenService: tokenService,
		validator:    validator,
		logger:       logger,
	}
}

func (h *authHandler) Login(w http.ResponseWriter, r *http.Request) {
	ctx, cancel := context.WithTimeout(r.Context(), requestTimeout)
	defer cancel()

	var req dtos.LoginRequest
	if err := h.decodeAndValidateJSON(w, r, &req); err != nil {
		return
	}

	h.logger.Info("attempting login", "username_or_email", req.UsernameOrEmail)

	accessToken, refreshToken, err := h.authService.Login(ctx, req.UsernameOrEmail, req.Password)
	if err != nil {
		h.handleError(w, err, "login failed")
		return
	}

	userID, err := h.extractUserIDFromToken(ctx, accessToken)
	if err != nil {
		h.handleError(w, err, "failed to extract user info")
		return
	}

	user, err := h.authService.GetUserByID(ctx, userID)
	if err != nil {
		h.handleError(w, err, "failed to get user info")
		return
	}

	response := dtos.LoginResponse{
		AccessToken:  accessToken,
		RefreshToken: refreshToken,
		User:         *dtos.GenerateUserDTO(*user),
	}

	h.logger.Info("login successful", "user_id", userID)
	h.writeSuccessResponse(w, http.StatusOK, "login successful", response)
}

func (h *authHandler) Register(w http.ResponseWriter, r *http.Request) {
	ctx, cancel := context.WithTimeout(r.Context(), requestTimeout)
	defer cancel()

	var req dtos.RegisterRequest
	if err := h.decodeAndValidateJSON(w, r, &req); err != nil {
		return
	}

	h.logger.Info("attempting registration", "username", req.Username, "email", req.Email)

	user, err := h.authService.Register(ctx, req.Username, req.Email, req.Password)
	if err != nil {
		h.handleError(w, err, "registration failed")
		return
	}

	accessToken, err := h.tokenService.GenerateAccessToken(ctx, user.ID)
	if err != nil {
		h.handleError(w, err, "failed to generate access token")
		return
	}

	refreshToken, err := h.tokenService.GenerateRefreshToken(ctx, user.ID)
	if err != nil {
		h.handleError(w, err, "failed to generate refresh token")
		return
	}

	response := dtos.RegisterResponse{
		AccessToken:  accessToken,
		RefreshToken: refreshToken,
		User:         *dtos.GenerateUserDTO(*user),
	}

	h.logger.Info("registration successful", "user_id", user.ID)
	h.writeSuccessResponse(w, http.StatusCreated, "registration successful", response)
}

func (h *authHandler) Logout(w http.ResponseWriter, r *http.Request) {
	ctx, cancel := context.WithTimeout(r.Context(), requestTimeout)
	defer cancel()

	refreshToken := h.extractRefreshTokenFromBody(w, r)
	if refreshToken == "" {
		return
	}

	h.logger.Info("attempting logout")

	if err := h.authService.Logout(ctx, refreshToken); err != nil {
		h.handleError(w, err, "logout failed")
		return
	}

	h.logger.Info("logout successful")
	h.writeSuccessResponse(w, http.StatusOK, "logout successful", nil)
}

func (h *authHandler) RefreshToken(w http.ResponseWriter, r *http.Request) {
	ctx, cancel := context.WithTimeout(r.Context(), requestTimeout)
	defer cancel()

	refreshToken := h.extractRefreshTokenFromBody(w, r)
	if refreshToken == "" {
		return
	}

	h.logger.Info("attempting token refresh")

	newAccessToken, err := h.tokenService.RefreshAccessToken(ctx, refreshToken)
	if err != nil {
		h.handleError(w, err, "token refresh failed")
		return
	}

	response := map[string]string{
		"access_token": newAccessToken,
	}

	h.logger.Info("token refresh successful")
	h.writeSuccessResponse(w, http.StatusOK, "token refreshed successfully", response)
}

func (h *authHandler) ValidateToken(w http.ResponseWriter, r *http.Request) {
	ctx, cancel := context.WithTimeout(r.Context(), requestTimeout)
	defer cancel()

	token := h.extractBearerToken(r)
	if token == "" {
		h.writeErrorResponse(w, http.StatusUnauthorized, "missing or invalid authorization header")
		return
	}

	h.logger.Info("attempting token validation")

	claims, err := h.tokenService.ValidateAccessToken(ctx, token)
	if err != nil {
		h.handleError(w, err, "token validation failed")
		return
	}

	userID, err := uuid.Parse(claims.UserID)
	if err != nil {
		h.writeErrorResponse(w, http.StatusUnauthorized, "invalid user ID in token")
		return
	}

	user, err := h.authService.GetUserByID(ctx, userID)
	if err != nil {
		h.handleError(w, err, "failed to get user info")
		return
	}

	response := map[string]any{
		"valid": true,
		"user":  dtos.GenerateUserDTO(*user),
	}

	h.logger.Info("token validation successful", "user_id", userID)
	h.writeSuccessResponse(w, http.StatusOK, "token is valid", response)
}

func (h *authHandler) decodeAndValidateJSON(w http.ResponseWriter, r *http.Request, dst any) error {
	if r.Header.Get("Content-Type") != "application/json" {
		h.writeErrorResponse(w, http.StatusBadRequest, "Content-Type must be application/json")
		return errors.New("invalid content type")
	}

	if err := json.NewDecoder(r.Body).Decode(dst); err != nil {
		h.logger.Error("failed to decode JSON", "error", err)
		h.writeErrorResponse(w, http.StatusBadRequest, "invalid JSON format")
		return err
	}

	if err := h.validator.Struct(dst); err != nil {
		h.logger.Error("validation failed", "error", err)
		h.writeErrorResponse(w, http.StatusBadRequest, "validation failed: "+err.Error())
		return err
	}

	return nil
}

func (h *authHandler) extractBearerToken(r *http.Request) string {
	authHeader := r.Header.Get("Authorization")
	if authHeader == "" || !strings.HasPrefix(authHeader, bearerPrefix) {
		return ""
	}
	return strings.TrimPrefix(authHeader, bearerPrefix)
}

func (h *authHandler) extractRefreshTokenFromBody(w http.ResponseWriter, r *http.Request) string {
	var tokenReq struct {
		RefreshToken string `json:"refresh_token" validate:"required"`
	}

	if err := h.decodeAndValidateJSON(w, r, &tokenReq); err != nil {
		return ""
	}

	return tokenReq.RefreshToken
}

func (h *authHandler) extractUserIDFromToken(ctx context.Context, accessToken string) (uuid.UUID, error) {
	claims, err := h.tokenService.ValidateAccessToken(ctx, accessToken)
	if err != nil {
		return uuid.Nil, err
	}

	userID, err := uuid.Parse(claims.UserID)
	if err != nil {
		return uuid.Nil, apperrors.NewBadRequest("invalid user ID format")
	}

	return userID, nil
}

func (h *authHandler) handleError(w http.ResponseWriter, err error, message string) {
	h.logger.Error(message, "error", err)

	var appErr *apperrors.AppError
	if errors.As(err, &appErr) {
		h.writeErrorResponse(w, appErr.Code, appErr.Message)
		return
	}

	h.writeErrorResponse(w, http.StatusInternalServerError, "internal server error")
}

func (h *authHandler) writeSuccessResponse(w http.ResponseWriter, statusCode int, message string, data any) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(statusCode)

	response := dtos.APIResponse{
		Success: true,
		Message: message,
		Data:    data,
	}

	if err := json.NewEncoder(w).Encode(response); err != nil {
		h.logger.Error("failed to encode success response", "error", err)
	}
}

func (h *authHandler) writeErrorResponse(w http.ResponseWriter, statusCode int, message string) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(statusCode)

	response := dtos.APIResponse{
		Success: false,
		Message: "error",
		Error:   message,
	}

	if err := json.NewEncoder(w).Encode(response); err != nil {
		h.logger.Error("failed to encode error response", "error", err)
	}
}
