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
	Username     string         `json:"username" validate:"required,min=2,max=24" gorm:"uniqueIndex;size:24"`
	Email        string         `json:"email" validate:"required,email,max=100" gorm:"uniqueIndex;size:100"`
	PasswordHash string         `json:"-" validate:"required" gorm:"size:255"`
	Status       UserStatus     `json:"status" validate:"required,oneof=active inactive suspended pending" gorm:"default:pending"`
	CreatedAt    time.Time      `json:"created_at" gorm:"autoCreateTime"`
	UpdatedAt    time.Time      `json:"updated_at" gorm:"autoUpdateTime"`
	DeletedAt    gorm.DeletedAt `json:"deleted_at" gorm:"index"`
	Tokens       []Token        `json:"tokens,omitempty" gorm:"foreignKey:UserID;constraint:OnDelete:CASCADE"`
}

type PublicUser struct {
	ID        uuid.UUID  `json:"id" validate:"required"`
	Username  string     `json:"username" validate:"required,min=2,max=24"`
	Email     string     `json:"email" validate:"required,email,max=100"`
	Status    UserStatus `json:"status" validate:"required,oneof=active inactive suspended pending"`
	CreatedAt time.Time  `json:"created_at"`
	UpdatedAt time.Time  `json:"updated_at"`
}

type UserCreateRequest struct {
	Username string `json:"username" validate:"required,min=2,max=24"`
	Email    string `json:"email" validate:"required,email,max=100"`
	Password string `json:"password" validate:"required,min=8,max=128"`
}

type UserUpdateRequest struct {
	Username *string     `json:"username,omitempty" validate:"omitempty,min=2,max=24"`
	Email    *string     `json:"email,omitempty" validate:"omitempty,email,max=100"`
	Status   *UserStatus `json:"status,omitempty" validate:"omitempty,oneof=active inactive suspended pending"`
}

type UserLoginRequest struct {
	Username string `json:"username" validate:"required"`
	Password string `json:"password" validate:"required"`
}

func NewUser(request UserCreateRequest, hashedPassword string) *User {
	now := time.Now()
	return &User{
		ID:           uuid.New(),
		Username:     strings.TrimSpace(request.Username),
		Email:        strings.ToLower(strings.TrimSpace(request.Email)),
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

func (user *User) IsActive() bool {
	return user.Status == UserStatusActive && user.DeletedAt.Time.IsZero()
}

func (user *User) CanLogin() bool {
	return user.Status == UserStatusActive || user.Status == UserStatusPending
}

func (user *User) Activate() {
	user.Status = UserStatusActive
}

func (user *User) Suspend() {
	user.Status = UserStatusSuspended
}

func (user *User) Deactivate() {
	user.Status = UserStatusInactive
}

func (us UserStatus) Value() (driver.Value, error) {
	return string(us), nil
}

func (us *UserStatus) Scan(value any) error {
	if value == nil {
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

func (us UserStatus) IsValid() bool {
	switch us {
	case UserStatusActive, UserStatusInactive, UserStatusSuspended, UserStatusPending:
		return true
	default:
		return false
	}
}
