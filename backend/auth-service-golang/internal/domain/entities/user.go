package entities

import (
	"auth-service/internal/errors/apperrors"
	"database/sql/driver"
	"net/mail"
	"regexp"
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
	ID           uuid.UUID      `gorm:"primaryKey"`
	Username     string         `gorm:"uniqueIndex;size:24"`
	Email        string         `gorm:"uniqueIndex;size:100"`
	PasswordHash string         `gorm:"size:255"`
	Status       UserStatus     `gorm:"default:pending"`
	CreatedAt    time.Time      `gorm:"autoCreateTime"`
	UpdatedAt    time.Time      `gorm:"autoUpdateTime"`
	DeletedAt    gorm.DeletedAt `gorm:"index"`
	Tokens       []RefreshToken `gorm:"foreignKey:UserID;constraint:OnDelete:CASCADE"`
}

func NewUser(username, email, passwordHash string) *User {
	now := time.Now()
	return &User{
		ID:           uuid.New(),
		Username:     strings.TrimSpace(username),
		Email:        strings.ToLower(strings.TrimSpace(email)),
		PasswordHash: passwordHash,
		Status:       UserStatusPending,
		CreatedAt:    now,
		UpdatedAt:    now,
	}
}

func IsValidEmail(email string) bool {
	_, err := mail.ParseAddress(email)
	return err == nil
}

func IsValidUserName(username string) bool {
	pattern := `^[A-Za-z0-9._]{2,24}$`
	re := regexp.MustCompile(pattern)
	return re.MatchString(username)
}

func IsValidPassword(password string) bool {
	pattern := `^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$`
	var re = regexp.MustCompile(pattern)
	return re.MatchString(password)
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
