package dtos

import (
	"auth-service/internal/domain/entities"

	"github.com/google/uuid"
)

type LoginRequest struct {
	UsernameOrEmail string `json:"username_or_email" validate:"required"`
	Password        string `json:"password" validate:"required"`
}

type LoginResponse struct {
	AccessToken  string  `json:"access_token"`
	RefreshToken string  `json:"refresh_token"`
	User         UserDTO `json:"user"`
}

type RegisterRequest struct {
	Username string `json:"username" validate:"required,min=2,max=24"`
	Email    string `json:"email" validate:"required,email,max=100"`
	Password string `json:"password" validate:"required,min=8,max=128"`
}

type RegisterResponse struct {
	AccessToken  string  `json:"access_token"`
	RefreshToken string  `json:"refresh_token"`
	User         UserDTO `json:"user"`
}

type UserDTO struct {
	ID       uuid.UUID           `json:"id" validate:"required"`
	Username string              `json:"username" validate:"required,min=2,max=24"`
	Email    string              `json:"email" validate:"required,email,max=100"`
	Status   entities.UserStatus `json:"status" validate:"required,oneof=active inactive suspended pending"`
}

func GenerateUserDTO(user entities.User) *UserDTO {
	return &UserDTO{
		ID:       user.ID,
		Username: user.Username,
		Email:    user.Email,
		Status:   user.Status,
	}
}
