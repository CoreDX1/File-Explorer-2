using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interface;
using Application.Services;
using Domain.Entities;
using Infrastructure.Interface;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserServices : Service<User>, IUserServices
{
    private readonly IJwtTokenService _jwtTokenService;

    public UserServices(IRepositoryAsync<User> repository, IJwtTokenService jwtTokenService)
        : base(repository)
    {
        _jwtTokenService = jwtTokenService;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await Queryable().ToListAsync();
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await Queryable().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<ApiResult<CreateUserResponse>> CreateUser(CreateUserRequest request)
    {
        var user = request.Adapt<User>();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        Insert(user);

        var userMapper = user.Adapt<CreateUserResponse>();

        return ApiResult<CreateUserResponse>.Success(userMapper, "User created successfully", 201);
    }

    public async Task<ApiResult<LoginResponse>> Login(string email, string password)
    {
        var user = await GetUserByEmail(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return ApiResult<LoginResponse>.Error("Invalid credentials", 401);
        }

        var userMapper = user.Adapt<LoginResponse>();

        user.LastLoginAt = DateTime.UtcNow;
        Update(user);

        return ApiResult<LoginResponse>.Success(userMapper, "Login successful", 200);
    }

    public async Task<ApiResult<LoginResponse>> AuthenticateAsync(string email, string password)
    {
        var user = await GetUserByEmail(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return ApiResult<LoginResponse>.Error("Invalid credentials", 401);
        }

        var token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Email, "User");
        var userMapper = user.Adapt<LoginResponse>();
        userMapper.Token = token;

        return ApiResult<LoginResponse>.Success(userMapper, "Login successful", 200);
    }

    public async Task<ApiResult<object>> CreateUserAsync(CreateUserRequest request)
    {
        var user = await GetUserByEmail(request.Email);
        if (user != null)
            return ApiResult<object>.Error("User already exists", 400);

        var newUser = request.Adapt<User>();
        newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        Insert(newUser);
        return ApiResult<object>.Success(null, "User created successfully", 201);
    }

    public Task<ApiResult<object>> RefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task RevokeTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<object>> GoogleAuthAsync(string idToken)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetAsync(string email)
    {
        throw new NotImplementedException();
    }
}
