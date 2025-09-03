package apperrors

import "errors"

var (
	ErrScanValue          = errors.New("scan value error")
	ErrDuplicateKey       = errors.New("duplicate key")
	ErrNotFound           = errors.New("entity not found")
	ErrInvalidCredentials = errors.New("invalid credentials")
	ErrEmailAlreadyExists = errors.New("email already exists")
	ErrInvalidToken       = errors.New("invalid token")
	ErrTokenExpired       = errors.New("token expired")
)
