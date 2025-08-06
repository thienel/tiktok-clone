package db

import (
	"database/sql"
	"errors"
	"testing"
	"video-service/config"

	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/mock"
)

type MockDatabase struct {
	mock.Mock
}

func (m *MockDatabase) AutoMigrate(dst ...any) error {
	args := m.Called(dst)
	return args.Error(0)
}

func (m *MockDatabase) DB() (*sql.DB, error) {
	args := m.Called()
	if args.Get(0) == nil {
		return nil, args.Error(1)
	}
	return args.Get(0).(*sql.DB), args.Error(1)
}

type UnsupportedDatabase struct {
	mock.Mock
}

func (u *UnsupportedDatabase) AutoMigrate(dst ...any) error {
	args := u.Called(dst)
	return args.Error(0)
}

func (u *UnsupportedDatabase) DB() (*sql.DB, error) {
	args := u.Called()
	if args.Get(0) == nil {
		return nil, args.Error(1)
	}
	return args.Get(0).(*sql.DB), args.Error(1)
}

func TestSetupDatabase_MigrationFailure(t *testing.T) {
	mockDB := &MockDatabase{}

	sqlDB := &sql.DB{}
	mockDB.On("DB").Return(sqlDB, nil)

	migrationError := errors.New("migration failed: column type conflict")
	mockDB.On("AutoMigrate", mock.Anything).Return(migrationError)

	_, err := setupDatabase(mockDB)

	assert.Error(t, err)
	assert.Contains(t, err.Error(), "failed to migrate database")
	assert.Contains(t, err.Error(), "migration failed: column type conflict")

	mockDB.AssertExpectations(t)
}

func TestSetupDatabase_DBInstanceFailure(t *testing.T) {
	mockDB := &MockDatabase{}

	dbError := errors.New("failed to get sql.DB instance")
	mockDB.On("DB").Return(nil, dbError)

	_, err := setupDatabase(mockDB)

	assert.Error(t, err)
	assert.Contains(t, err.Error(), "failed to get database instance")
	assert.Contains(t, err.Error(), "failed to get sql.DB instance")

	mockDB.AssertExpectations(t)
}

func TestSetupDatabase_UnsupportedDatabaseType(t *testing.T) {
	unsupportedDB := &UnsupportedDatabase{}

	sqlDB := &sql.DB{}
	unsupportedDB.On("DB").Return(sqlDB, nil)
	unsupportedDB.On("AutoMigrate", mock.Anything).Return(nil)

	_, err := setupDatabase(unsupportedDB)

	assert.Error(t, err)
	assert.Contains(t, err.Error(), "unsupported database type")

	unsupportedDB.AssertExpectations(t)
}

func TestCreateConnection_ConnectionFailure(t *testing.T) {
	cfg := &config.DatabaseConfig{
		Host:     "invalid-host-that-does-not-exist",
		User:     "invalid_user",
		Password: "wrong_password",
		DBName:   "nonexistent_db",
		Port:     "9999", // Invalid port
		SSLMode:  "disable",
	}

	_, err := createConnection(cfg)

	assert.Error(t, err)
	assert.Contains(t, err.Error(), "failed to connect to database")
}

func TestNewConnection_InvalidConfig(t *testing.T) {
	cfg := &config.DatabaseConfig{
		Host:     "",
		User:     "",
		Password: "",
		DBName:   "",
		Port:     "",
		SSLMode:  "",
	}

	_, err := NewConnection(cfg)

	assert.Error(t, err)
	assert.Contains(t, err.Error(), "failed to connect to database")
}

func TestSetupDatabase_Success(t *testing.T) {
	mockDB := &MockDatabase{}
	sqlDB := &sql.DB{}
	mockDB.On("DB").Return(sqlDB, nil)
	mockDB.On("AutoMigrate", mock.Anything).Return(nil)
}
