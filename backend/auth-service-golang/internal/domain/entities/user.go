package entities

import (
	"auth-service/internal/errors/apperrors"
	"database/sql/driver"
	"strings"
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type UserStatus string

const (
	UserStatusActive    UserStatus = "active"
	UserStatusInactive  UserStatus = "inactive"
	UserStatusSuspended UserStatus = "suspended"
	UserStatusPending   UserStatus = "pending"
)

type User struct {
	ID           uuid.UUID      `json:"id" validate:"required" gorm:"primaryKey"`
	Username     string         `json:"username" validate:"required,min=2,max=24"`
	Email        string         `json:"email" validate:"required,email"`
	PasswordHash string         `json:"-" validate:"required"`
	Status       UserStatus     `json:"status" validate:"required,oneof=active inactive suspended pending"`
	CreatedAt    time.Time      `json:"created_at" gorm:"autoCreateTime"`
	UpdatedAt    time.Time      `json:"updated_at" gorm:"autoUpdateTime"`
	DeletedAt    gorm.DeletedAt `json:"deleted_at" gorm:"index"`
	Tokens       []Token        `json:"tokens,omitempty" gorm:"foreignKey:UserID;constraint:OnDelete:CASCADE"`
}

type PublicUser struct {
	ID        uuid.UUID  `json:"id"  validate:"required"`
	Username  string     `json:"username"  validate:"required,min=2,max=24"`
	Email     string     `json:"email"  validate:"required,email"`
	Status    UserStatus `json:"status"  validate:"required,oneof=active inactive suspended pending"`
	CreatedAt time.Time  `json:"created_at"`
	UpdatedAt time.Time  `json:"updated_at"`
}

type UserCreateRequest struct {
	Username string `json:"username"  validate:"required,min=2,max=24"`
	Email    string `json:"email"  validate:"required,email"`
	Password string `json:"password"  validate:"required,min=8"`
}

func NewUser(request UserCreateRequest, hashedPassword string) *User {
	now := time.Now()
	return &User{
		ID:           uuid.New(),
		Username:     request.Username,
		Email:        strings.ToLower(request.Email),
		PasswordHash: hashedPassword,
		Status:       UserStatusPending,
		CreatedAt:    now,
		UpdatedAt:    now,
	}
}

func (user *User) ToPublicUser() *PublicUser {
	return &PublicUser{
		ID:        user.ID,
		Username:  user.Username,
		Email:     user.Email,
		Status:    user.Status,
		CreatedAt: user.CreatedAt,
		UpdatedAt: user.UpdatedAt,
	}
}

func (us UserStatus) Value() (driver.Value, error) {
	return string(us), nil
}

func (us *UserStatus) Scan(value any) error {
	if us == nil {
		return nil
	}

	switch s := value.(type) {
	case string:
		*us = UserStatus(s)
		return nil
	case []byte:
		*us = UserStatus(s)
		return nil
	default:
		return apperrors.ErrScanValue
	}
}
