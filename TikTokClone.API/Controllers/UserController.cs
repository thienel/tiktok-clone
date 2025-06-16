using Microsoft.AspNetCore.Mvc;
using TikTokClone.Application.Constants;
using TikTokClone.Application.DTOs;
using TikTokClone.Application.Interfaces.Services;

namespace TikTokClone.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("check-username")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckUsername([FromBody] CheckUsernameDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid input data",
                    ErrorCode = ErrorCodes.VALIDATION_ERROR
                });
            }

            var result = await _userService.CheckValidUsernameAsync(request.Username);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("change-username")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameDto request)
        {
            _logger.LogInformation("ChangeUsername called with Type: {Type}, IdOrEmail: {IdOrEmail}, Username: {Username}",
                request.Type, request.IdOrEmail, request.Username);

            if (!ModelState.IsValid || !(request.Type == "Email" || request.Type == "Id"))
            {
                _logger.LogWarning("ChangeUsername failed due to invalid input");
                return BadRequest(new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid input data",
                    ErrorCode = ErrorCodes.VALIDATION_ERROR
                });
            }

            var result = new UserResponseDto();
            if (request.Type == "Email")
            {
                result = await _userService.ChangeUsernameByEmailAsync(request.IdOrEmail, request.Username);
            }
            else if (request.Type == "Id")
            {
                result = await _userService.ChangeUsernameByIdAsync(request.IdOrEmail, request.Username);
            }

            if (!result.IsSuccess)
            {
                _logger.LogWarning("ChangeUsername failed: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("ChangeUsername succeeded for {Type}: {IdOrEmail}", request.Type, request.IdOrEmail);
            return Ok(result);
        }

        [HttpPost("check-birthdate")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CheckBirthdate([FromBody] CheckBirthdateDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid input data",
                    ErrorCode = ErrorCodes.VALIDATION_ERROR
                });
            }

            var result = _userService.CheckValidBirthDate(request.BirthDate);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
