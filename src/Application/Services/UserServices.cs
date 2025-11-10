using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interface;
using Domain.Entities;
using Domain.Monads;
using FluentValidation;
using FluentValidation.Results;
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
    private readonly LockoutOptions _lockoutOptions = new();

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

    public async Task<ApiResult<List<GetUserResponse>>> GetAllUsersAsync()
    {
        try
        {
            var query = Queryable().AsNoTracking().OrderBy(u => u.Id);

            var users = await query.ToListAsync();

            var dto = users.Adapt<List<GetUserResponse>>();

            _logger.LogInformation("Retrieved {Count} users", dto.Count);

            return ApiResult<List<GetUserResponse>>.Success(
                dto,
                "Users retrieved successfully",
                200
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return ApiResult<List<GetUserResponse>>.Error("Error retrieving users", 500);
        }
    }

    public async Task<Maybe<User>> FindByEmailAsync(string email)
    {
        var user = await Queryable().FirstOrDefaultAsync(u => u.Email == email);
        return Maybe.From(user);
    }

    public async Task<ApiResult<LoginResponse>> AuthenticateAsync(string email, string password)
    {
        _logger.LogInformation("Authentication attempt for {Email}", email);

        var maybeUser = await FindByEmailAsync(email);
        User? user = maybeUser.Value;

        if (user == null)
        {
            _logger.LogWarning("Authentication failed: Invalid credentials for {Email}", email);
            return ApiResult<LoginResponse>.Error("Invalid credentials", 401);
        }

        // Check if user is currently locked out
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            _logger.LogWarning(
                "Authentication blocked: User {Email} is locked until {LockoutEnd}",
                email,
                user.LockoutEnd.Value
            );

            return ApiResult<LoginResponse>.Error("User is locked. Try again later.", 403);
        }

        // Validate password
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            user.FailedLoginAttemts++;

            if (user.FailedLoginAttemts >= _lockoutOptions.MaxFailedAccessAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.Add(_lockoutOptions.DefaultLockoutTimeSpan);
                _logger.LogWarning(
                    "User {Email} locked out until {LockoutEnd} after {Attempts} failed attempts",
                    email,
                    user.LockoutEnd.Value,
                    user.FailedLoginAttemts
                );

                Update(user);
                await _unitOfwork.SaveChangesAsync();

                // Usuario ya quedó bloqueado: informamos lockout con intento final
                return ApiResult<LoginResponse>.Error(
                    $"Account locked due to too many failed attempts. Attempt {user.FailedLoginAttemts} of {_lockoutOptions.MaxFailedAccessAttempts}. Try again later.",
                    403
                );
            }
            else
            {
                int remainingAttempts =
                    _lockoutOptions.MaxFailedAccessAttempts - user.FailedLoginAttemts;

                _logger.LogWarning(
                    "Authentication failed: Invalid password for {Email}. Failed attempts: {Attempts}, Remaining before lockout: {Remaining}",
                    email,
                    user.FailedLoginAttemts,
                    remainingAttempts
                );

                Update(user);
                await _unitOfwork.SaveChangesAsync();

                // Mensaje incluye en qué intento estás y cuántos tienes en total
                return ApiResult<LoginResponse>.Error(
                    $"Invalid credentials. Attempt {user.FailedLoginAttemts} of {_lockoutOptions.MaxFailedAccessAttempts}.",
                    401
                );
            }
        }

        // Successful authentication: reset counters
        user.FailedLoginAttemts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;

        string token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Email, "User");

        Update(user);
        await _unitOfwork.SaveChangesAsync();

        LoginResponse userMapper = user.Adapt<LoginResponse>();
        userMapper.Token = token;

        _logger.LogInformation(
            "User authenticated successfully: {Email}, UserId: {UserId}",
            email,
            user.Id
        );

        return ApiResult<LoginResponse>.Success(userMapper, "Login successful", 200);
    }

    private static bool ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return true;
    }

    public async Task<ApiResult<LoginResponse>> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            _logger.LogInformation(
                "User creation attempt for {Email}, Name: {FirstName} {LastName}",
                request.Email,
                request.FirstName,
                request.LastName
            );

            // 1. Validate request
            ValidationResult validatorResult = await _validator.ValidateAsync(request);

            if (!validatorResult.IsValid)
            {
                var errors = string.Join(", ", validatorResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning(
                    "User creation validation failed for {Email}. Errors: {ValidationErrors}",
                    request.Email,
                    errors
                );
                return ApiResult<LoginResponse>.Error(errors, 400);
            }

            // 2. Check if user already exists
            var maybeUser = await FindByEmailAsync(request.Email);
            if (maybeUser.IsSome)
            {
                _logger.LogWarning(
                    "User creation failed: Email {Email} already exists",
                    request.Email
                );
                return ApiResult<LoginResponse>.Error("User already exists", 409);
            }

            // 3. Create user entity
            User userToCreate = request.Adapt<User>();
            userToCreate.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 4. Set audit fields
            userToCreate.CreatedAt = DateTime.UtcNow;
            userToCreate.UpdatedAt = DateTime.UtcNow;
            userToCreate.IsActive = false;

            // 5. Persist to database
            Insert(userToCreate);
            await _unitOfwork.SaveChangesAsync();

            // 6. Generate JWT token for automatic login
            string token = _jwtTokenService.GenerateToken(
                userToCreate.Id.ToString(),
                userToCreate.Email,
                "User"
            );

            LoginResponse response = userToCreate.Adapt<LoginResponse>();

            response.Token = token;

            _logger.LogInformation(
                "User created successfully: {Email}, UserId: {UserId}",
                request.Email,
                userToCreate.Id
            );

            return ApiResult<LoginResponse>.Success(response, "User created successfully", 201);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error creating user {Email}", request.Email);
            return ApiResult<LoginResponse>.Error("Database error occurred", 500);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating user {Email}", request.Email);
            return ApiResult<LoginResponse>.Error($"Error creating user: {ex.Message}", 500);
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
            var maybeUser = await FindByEmailAsync(userRequest.Email);

            if (maybeUser.IsNone)
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
