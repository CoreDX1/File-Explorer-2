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

    // public Task<ApiResult<CreateUserResponse>> CreateUser(CreateUserRequest request);
    // public Task<ApiResult<LoginResponse>> Login(string email, string password);

    // New methods for AuthController
    Task<ApiResult<LoginResponse>> AuthenticateAsync(string email, string password);
    Task<ApiResult<bool>> EditUser(EditUserRequest user);
    Task<ApiResult<LoginResponse>> CreateUserAsync(CreateUserRequest request);
    Task<ApiResult<object>> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
    Task<ApiResult<object>> GoogleAuthAsync(string idToken);
    Task SendPasswordResetAsync(string email);
}
