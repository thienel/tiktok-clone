package database

import (
	"database/sql"
	"fmt"
	"time"

	_ "github.com/lib/pq"
)

type Database struct {
	DB *sql.DB
}

func New(databaseURL string) (*Database, error) {
	db, err := sql.Open("postgres", databaseURL)
	if err != nil {
		return nil, fmt.Errorf("error connecting to database: %w", err)
	}

	db.SetMaxOpenConns(25)
	db.SetMaxIdleConns(25)
	db.SetConnMaxLifetime(5 * time.Minute)

	if err := db.Ping(); err != nil {
		err := db.Close()
		if err != nil {
			return nil, fmt.Errorf("error closing database after pinging failed: %w", err)
		}
		return nil, fmt.Errorf("error pinging database: %w", err)
	}

	return &Database{DB: db}, nil
}

func (d *Database) Ping() error {
	return d.DB.Ping()
}

func (d *Database) Close() error {
	if d.DB != nil {
		return d.DB.Close()
	}
	return nil
}
