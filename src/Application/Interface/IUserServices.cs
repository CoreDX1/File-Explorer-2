using Application.DTOs.Request;
using Application.DTOs.Response;
using Domain.Entities;

namespace Application.Interface;

/// <summary>
/// Agregar cualquier metodo de logica de negocio aqui
/// </summary>
public interface IUserServices : IService<User>
{
    public Task<User> GetUserByEmail(string email);
    public Task<List<User>> GetAllUsers();
    public Task<ResponseDTO> CreateUser(CreateUserRequest request);
    public Task<ResponseDTO> Login(string email, string password);

    // New methods for AuthController
    Task<AuthResult> AuthenticateAsync(string email, string password);
    Task<AuthResult> CreateUserAsync(CreateUserRequest request);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
    Task<AuthResult> GoogleAuthAsync(string idToken);
    Task SendPasswordResetAsync(string email);
}

public record AuthResult(bool Success, string Message, object? Data = null);
