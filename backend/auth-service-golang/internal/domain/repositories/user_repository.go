package repositories

import (
	"auth-service/internal/domain/entities"
	"context"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type UserRepository interface {
	Create(ctx context.Context, user *entities.User) error
	FindByID(ctx context.Context, id uuid.UUID) (*entities.User, error)
	FindByEmail(ctx context.Context, email string) (*entities.User, error)
	Update(ctx context.Context, user *entities.User) error
	SoftDelete(ctx context.Context, id uuid.UUID) error
	List(ctx context.Context, limit, offset int) ([]entities.User, error)
}

type userRepository struct {
	DB *gorm.DB
}

func NewUserRepository(db *gorm.DB) UserRepository {
	return &userRepository{db}
}

func (u userRepository) Create(ctx context.Context, user *entities.User) error {
	//TODO implement me
	panic("implement me")
}

func (u userRepository) FindByID(ctx context.Context, id uuid.UUID) (*entities.User, error) {
	//TODO implement me
	panic("implement me")
}

func (u userRepository) FindByEmail(ctx context.Context, email string) (*entities.User, error) {
	//TODO implement me
	panic("implement me")
}

func (u userRepository) Update(ctx context.Context, user *entities.User) error {
	//TODO implement me
	panic("implement me")
}

func (u userRepository) SoftDelete(ctx context.Context, id uuid.UUID) error {
	//TODO implement me
	panic("implement me")
}

func (u userRepository) List(ctx context.Context, limit, offset int) ([]entities.User, error) {
	//TODO implement me
	panic("implement me")
}
