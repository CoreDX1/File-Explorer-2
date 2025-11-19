using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interface;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserServices _userServices;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserServices userServices, ILogger<AuthController> logger)
    {
        _userServices = userServices;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _userServices.AuthenticateUserAsync(request.Email, request.Password);

        if (result.Metadata is null)
        {
            _logger.LogWarning("Login failed for {Email}: missing metadata", request.Email);
            return StatusCode(500, new { message = "Unexpected error" });
        }

        // Propagar exactamente el código y mensaje definidos en el servicio
        if (result.Metadata.StatusCode != 200)
        {
            _logger.LogWarning(
                "Failed login attempt for {Email}. Status: {StatusCode}, Message: {Message}",
                request.Email,
                result.Metadata.StatusCode,
                result.Metadata.Message
            );

            return StatusCode(
                result.Metadata.StatusCode ?? 500,
                new { message = result.Metadata.Message }
            );
        }

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        var result = await _userServices.RegisterUserAsync(request);

        if (result.Metadata?.StatusCode != 201)
            return BadRequest(new { message = result.Metadata?.Message });

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _userServices.RefreshAuthenticationAsync(request.RefreshToken);

        if (result.Metadata?.StatusCode != 200)
            return Unauthorized(new { message = "Invalid refresh token" });

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        await _userServices.RevokeAuthenticationAsync(request.RefreshToken);
        return NoContent();
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthRequest request)
    {
        var result = await _userServices.AuthenticateWithGoogleAsync(request.IdToken);

        if (result.Metadata?.StatusCode != 200)
            return BadRequest(new { message = result.Metadata?.Message });

        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _userServices.InitiatePasswordResetAsync(request.Email);
        return Ok(new { message = "Password reset email sent" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _userServices.ResetPasswordAsync(request.Token, request.NewPassword);
        
        if (result.Metadata?.StatusCode != 200)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPut("editUser")]
    public async Task<IActionResult> Update([FromBody] EditUserRequest request)
    {
        var result = await _userServices.UpdateUserProfileAsync(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> FindByIdAsync(int id)
    {
        var result = await _userServices.FindByIdAsync(id);
        return Ok(result);
    }
}

public record RefreshTokenRequest(string RefreshToken);

public record LogoutRequest(string RefreshToken);

public record GoogleAuthRequest(string IdToken);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Token, string NewPassword);
