using Application.DTOs.Request;
using Application.DTOs.Response;
using Domain.Entities;
using Domain.Monads;

namespace Application.Interfaces;

/// <summary>
/// Add any business logic methods here
/// </summary>
public interface IUserServices : IService<User>
{
    public Task<Maybe<User>> FindByEmailAsync(string email);
    public Task<ApiResult<List<GetUserResponse>>> GetAllUsersAsync();

    // public Task<ApiResult<CreateUserResponse>> CreateUser(CreateUserRequest request);
    // public Task<ApiResult<LoginResponse>> Login(string email, string password);

    // Authentication methods
    Task<ApiResult<LoginResponse>> AuthenticateUserAsync(string email, string password);
    Task<ApiResult<LoginResponse>> RegisterUserAsync(CreateUserRequest request);
    Task<ApiResult<object>> RefreshAuthenticationAsync(string refreshToken);
    Task RevokeAuthenticationAsync(string refreshToken);
    Task<ApiResult<object>> AuthenticateWithGoogleAsync(string idToken);

    // User management methods
    Task<ApiResult<bool>> UpdateUserProfileAsync(EditUserRequest user);
    Task InitiatePasswordResetAsync(string email);
    Task<ApiResult<bool>> ResetPasswordAsync(string token, string newPassword);
    Task<ApiResult<GetUserResponseUnique>> FindByIdAsync(int id);
}
