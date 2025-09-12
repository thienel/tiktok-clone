package providers

import (
	"auth-service/internal/errors/apperrors"
	"context"
	"fmt"

	"github.com/coreos/go-oidc"
	"golang.org/x/oauth2"
)

type GoogleProvider struct {
	Config *oauth2.Config
}

func NewGoogleProvider(clientID, clientSecret, redirectURL string) Provider {
	cfg := &oauth2.Config{
		ClientID:     clientID,
		ClientSecret: clientSecret,
		RedirectURL:  redirectURL,
		Scopes:       []string{"openid", "email"},
		Endpoint: oauth2.Endpoint{
			AuthURL:  "https://accounts.google.com/o/oauth2/v2/auth",
			TokenURL: "https://oauth2.googleapis.com/token",
		},
	}
	return &GoogleProvider{Config: cfg}
}

func (g *GoogleProvider) Name() string {
	return "google"
}

func (g *GoogleProvider) GetAuthURL(state string) string {
	url := g.Config.AuthCodeURL(state, oauth2.AccessTypeOnline)
	fmt.Println(url)
	return url
}

func (g *GoogleProvider) ExchangeCode(ctx context.Context, code string) (*oauth2.Token, error) {
	token, err := g.Config.Exchange(ctx, code)
	if err != nil {
		if oauthErr, ok := err.(*oauth2.RetrieveError); ok {
			return nil, apperrors.NewBadRequest(fmt.Sprintf("oauth exchange failed: %s", string(oauthErr.Body)))
		}

		return nil, apperrors.NewBadGateway(fmt.Sprintf("failed to exchange code: %v", err))
	}
	return token, nil
}

func (g *GoogleProvider) GetUserInfo(ctx context.Context, token *oauth2.Token) (*UserInfo, error) {
	rawIDToken, ok := token.Extra("id_token").(string)
	if !ok {
		return nil, apperrors.NewUnauthorized("no id_token field in oauth2 token")
	}

	verifier := oidc.NewVerifier(
		"https://accounts.google.com",
		oidc.NewRemoteKeySet(ctx, "https://www.googleapis.com/oauth2/v3/certs"),
		&oidc.Config{ClientID: g.Config.ClientID},
	)

	idToken, err := verifier.Verify(ctx, rawIDToken)
	if err != nil {
		return nil, apperrors.NewUnauthorized("id token verification failed")
	}

	var claims struct {
		Sub   string `json:"sub"`
		Email string `json:"email"`
	}

	if err = idToken.Claims(&claims); err != nil {
		return nil, apperrors.NewInternal("failed to parse claims from id token", err)
	}

	return &UserInfo{
		Provider:   g.Name(),
		ProviderID: claims.Sub,
		Email:      claims.Email,
	}, nil
}
