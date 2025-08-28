package entities

import (
	"strings"
	"time"

	"github.com/google/uuid"
)

type UserStatus string

const (
	UserStatusActive    UserStatus = "active"
	UserStatusInactive  UserStatus = "inactive"
	UserStatusSuspended UserStatus = "suspended"
	UserStatusPending   UserStatus = "pending"
	UserStatusDeleted   UserStatus = "deleted"
)

type User struct {
	ID           uuid.UUID  `json:"id" db:"id" validate:"required"`
	UserName     string     `json:"user_name" db:"user_name" validate:"required,min=2,max=24"`
	Email        string     `json:"email" db:"email" validate:"required,email"`
	PasswordHash string     `json:"-" db:"password_hash" validate:"required"`
	Status       UserStatus `json:"status" db:"status" validate:"required,oneof=active inactive suspended pending deleted"`
	CreatedAt    time.Time  `json:"created_at" db:"created_at"`
	UpdatedAt    time.Time  `json:"updated_at" db:"updated_at"`
	DeletedAt    *time.Time `json:"deleted_at" db:"deleted_at"`
	Tokens       []Token    `json:"tokens,omitempty" db:"-"`
}

type PublicUser struct {
	ID        uuid.UUID  `json:"id"  validate:"required"`
	UserName  string     `json:"user_name"  validate:"required,min=2,max=24"`
	Email     string     `json:"email"  validate:"required,email"`
	Status    UserStatus `json:"status"  validate:"required,oneof=active inactive suspended pending deleted"`
	CreatedAt time.Time  `json:"created_at"`
	UpdatedAt time.Time  `json:"updated_at"`
}

type UserCreateRequest struct {
	UserName string `json:"user_name"  validate:"required,min=3,max=24"`
	Email    string `json:"email"  validate:"required,email"`
	Password string `json:"password"  validate:"required,min=8"`
}

func NewUser(request UserCreateRequest, hashedPassword string) *User {
	now := time.Now()
	return &User{
		ID:           uuid.New(),
		UserName:     request.UserName,
		Email:        strings.ToLower(request.Email),
		PasswordHash: hashedPassword,
		Status:       UserStatusPending,
		CreatedAt:    now,
		UpdatedAt:    now,
		DeletedAt:    nil,
	}
}

func (user *User) ToPublicUser() PublicUser {
	return PublicUser{
		ID:        user.ID,
		UserName:  user.UserName,
		Email:     user.Email,
		Status:    user.Status,
		CreatedAt: user.CreatedAt,
		UpdatedAt: user.UpdatedAt,
	}
}

func (user *User) SoftDelete() {
	now := time.Now()
	user.DeletedAt = &now
	user.Status = UserStatusDeleted
	user.UpdatedAt = now
}
