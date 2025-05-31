using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TikTokClone.Application.DTOs;
using TikTokClone.Application.Interfaces.Services;

namespace TikTokClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", request?.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for registration");
                    return BadRequest(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid input data",
                        ErrorCode = "VALIDATION_ERROR"
                    });
                }

                var result = await _authService.RegisterAsync(request!);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Registration failed for {Email}: {Message}",
                        request!.Email, result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("User registered successfully: {Email}", request!.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request?.Email);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "An internal server error occurred",
                        ErrorCode = "INTERNAL_ERROR"
                    });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                _logger.LogInformation("Login attempt for: {UsernameOrEmail}", request?.UsernameOrEmail);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for login");
                    return BadRequest(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid input data",
                        ErrorCode = "VALIDATION_ERROR"
                    });
                }

                var result = await _authService.LoginAsync(request!);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Login failed for {UsernameOrEmail}: {Message}",
                        request!.UsernameOrEmail, result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("User logged in successfully: {UsernameOrEmail}",
                    request!.UsernameOrEmail);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for: {UsernameOrEmail}",
                    request?.UsernameOrEmail);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "An internal server error occurred",
                        ErrorCode = "INTERNAL_ERROR"
                    });
            }
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                _logger.LogInformation("Token refresh attempt");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for token refresh");
                    return BadRequest(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid input data",
                        ErrorCode = "VALIDATION_ERROR"
                    });
                }

                var result = await _authService.RefreshTokenAsync(request.RefreshToken);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Token refresh failed: {Message}", result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("Tokens refreshed successfully");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "An internal server error occurred",
                        ErrorCode = "INTERNAL_ERROR"
                    });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Logout attempt with invalid token - no user ID found");
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Message = "Invalid token - user ID not found",
                        ErrorCode = "INVALID_TOKEN"
                    });
                }

                _logger.LogInformation("Logout attempt for user: {UserId}", userId);

                var result = await _authService.LogoutAsync(userId);

                if (!result)
                {
                    _logger.LogWarning("Logout failed for user: {UserId}", userId);
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Message = "Logout failed",
                        ErrorCode = "LOGOUT_FAILED"
                    });
                }

                _logger.LogInformation("User logged out successfully: {UserId}", userId);
                return Ok(new
                {
                    IsSuccess = true,
                    Message = "Logged out successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        IsSuccess = false,
                        Message = "An internal server error occurred",
                        ErrorCode = "INTERNAL_ERROR"
                    });
            }
        }

        [HttpPost("confirm-email")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmEmail(
            [FromQuery] string userId,
            [FromQuery] string token)
        {
            try
            {
                _logger.LogInformation("Email confirmation attempt for user: {UserId}", userId);

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Email confirmation attempt with missing parameters");
                    return BadRequest(new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "User ID and token are required",
                        ErrorCode = "VALIDATION_ERROR"
                    });
                }

                var result = await _authService.ConfirmEmailAsync(userId, token);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Email confirmation failed for user {UserId}: {Message}",
                        userId, result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("Email confirmed successfully for user: {UserId}", userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email confirmation for user: {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "An internal server error occurred",
                        ErrorCode = "INTERNAL_ERROR"
                    });
            }
        }

        [HttpPost("resend-confirmation")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendEmailConfirmationDto request)
        {
            try
            {
                _logger.LogInformation("Resend confirmation attempt for email: {Email}", request?.Email);

                if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request?.Email))
                {
                    _logger.LogWarning("Invalid email for resend confirmation");
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Message = "Valid email address is required",
                        ErrorCode = "VALIDATION_ERROR"
                    });
                }

                var result = await _authService.SendEmailConfirmationAsync(request.Email);

                if (!result)
                {
                    _logger.LogWarning("Could not send confirmation email to: {Email}", request.Email);
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Message = "Could not send confirmation email. Email may already be confirmed or not found.",
                        ErrorCode = "EMAIL_SEND_FAILED"
                    });
                }

                _logger.LogInformation("Confirmation email sent successfully to: {Email}", request.Email);
                return Ok(new
                {
                    IsSuccess = true,
                    Message = "Confirmation email sent successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during resend confirmation for email: {Email}", request?.Email);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        IsSuccess = false,
                        Message = "An internal server error occurred",
                        ErrorCode = "INTERNAL_ERROR"
                    });
            }
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var username = User.FindFirst("username")?.Value;
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                var isVerified = User.FindFirst("isVerified")?.Value;

                return Ok(new
                {
                    IsSuccess = true,
                    User = new
                    {
                        Id = userId,
                        Email = email,
                        Username = username,
                        Name = name,
                        IsVerified = bool.TryParse(isVerified, out var verified) && verified
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user info");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        IsSuccess = false,
                        Message = "An internal server error occurred",
                        ErrorCode = "INTERNAL_ERROR"
                    });
            }
        }
    }

    public class ResendEmailConfirmationDto
    {
        public string Email { get; set; } = string.Empty;
    }
}
