package entities

import (
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
	UserName     string     `json:"user_name" db:"user_name" validate:"required,min=3,max=24"`
	Email        string     `json:"email" db:"email" validate:"required,email"`
	PasswordHash string     `json:"-" db:"password_hash" validate:"required,min=60"`
	Status       UserStatus `json:"status" db:"status" validate:"required,oneof=active inactive suspended pending deleted"`
	CreatedAt    time.Time  `json:"created_at" db:"created_at"`
	UpdatedAt    time.Time  `json:"updated_at" db:"updated_at"`
	Tokens       []Token    `json:"tokens,omitempty" db:"-"`
}
