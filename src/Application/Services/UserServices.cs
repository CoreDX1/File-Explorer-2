using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interface;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Interface;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class UserServices : Service<User>, IUserServices
{
    private readonly ILogger<UserServices> _logger;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWorkAsync _unitOfwork;
    private readonly IValidator<CreateUserRequest> _validator;

    public UserServices(
        IRepositoryAsync<User> repository,
        IJwtTokenService jwtTokenService,
        IUnitOfWorkAsync unitOfwork,
        IValidator<CreateUserRequest> validator,
        ILogger<UserServices> logger
    )
        : base(repository)
    {
        _jwtTokenService = jwtTokenService;
        _unitOfwork = unitOfwork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await Queryable().ToListAsync();
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await Queryable().FirstOrDefaultAsync(u => u.Email == email);
    }

    // public async Task<ApiResult<CreateUserResponse>> CreateUser(CreateUserRequest request)
    // {
    //     var existingEmail = await GetUserByEmail(request.Email);

    //     if (existingEmail == null)
    //     {
    //         return ApiResult<CreateUserResponse>.Error("Error", 401);
    //     }

    //     var user = request.Adapt<User>();
    //     user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

    //     Insert(user);
    //     await _unitOfwork.SaveChangesAsync();

    //     var userMapper = user.Adapt<CreateUserResponse>();

    //     return ApiResult<CreateUserResponse>.Success(userMapper, "User created successfully", 201);
    // }

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

    public async Task<ApiResult<bool>> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            _logger.LogInformation("Creating user: {Email}", request.Email);

            var validatorResult = await _validator.ValidateAsync(request);

            if (!validatorResult.IsValid)
            {
                var errors = string.Join(", ", validatorResult.Errors.Select(e => e.ErrorMessage));

                return ApiResult<bool>.Error(errors, 400);
            }

            var user = await GetUserByEmail(request.Email);

            if (user != null)
                return ApiResult<bool>.Error("User already exists", 409);

            var newUser = request.Adapt<User>();
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            Insert(newUser);

            await _unitOfwork.SaveChangesAsync(); // **

            _logger.LogInformation("User created successfully: {Email}", request.Email);

            return ApiResult<bool>.Success(true, "User created successfully", 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", request.Email);

            return ApiResult<bool>.Error($"Error creating user: {ex.Message}", 500);
        }
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

    public async Task<ApiResult<bool>> EditUser(EditUserRequest userRequest)
    {
        try
        {
            var existingUser = await GetUserByEmail(userRequest.Email);
            if (existingUser == null)
            {
                return ApiResult<bool>.Error("User not found", 404);
            }

            var userEdit = userRequest.Adapt<User>();
            Update(userEdit);
            await _unitOfwork.SaveChangesAsync();

            return ApiResult<bool>.Success(true, "User Edit", 200);
        }
        catch (Exception ex)
        {
            return ApiResult<bool>.Error($"Error updating user: {ex.Message}", 500);
        }
    }
}
