using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Mappings;
using Domain.Entities;
using Domain.Monads;
using Domain.Monads.Result;
using Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Domain.Monads.Result.ResultExtensions;

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

    public async Task<ApiResult<GetUserResponseUnique>> FindByIdAsync(int id)
    {
        _logger.LogInformation("Finding user by ID: {Id}", id);

        Maybe<User> user = await FindAsync(id).ConfigureAwait(false);

        if (user.IsNone)
        {
            _logger.LogWarning("User not found by ID: {Id}", id);
            return ApiResult<GetUserResponseUnique>.Error("No se encontro al usuario", 501);
        }

        var userDto = user.Value.ToDto();

        _logger.LogInformation("User found by ID: {Id}", id);
        return ApiResult<GetUserResponseUnique>.Success(userDto, "El usuario se encontro", 200);
    }

    // TODO: Implementar los loggers en este método
    public async Task<ApiResult<List<GetUserResponse>>> GetAllUsersAsync()
    {
        try
        {
            var query = Queryable().AsNoTracking().OrderBy(u => u.Id);

            var users = await query.ToListAsync().ConfigureAwait(false);

            var dto = users.Adapt<List<GetUserResponse>>();

            _logger.LogInformation("Retrieved {Count} users", dto.Count);

            return ApiResult<List<GetUserResponse>>.Success(
                dto,
                "Users retrieved successfully",
                200
            );
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error retrieving users");
            return ApiResult<List<GetUserResponse>>.Error("Database error occurred", 500);
        }
    }

    public async Task<Maybe<User>> FindByEmailAsync(string email)
    {
        User? user = await Queryable()
            .FirstOrDefaultAsync(u => u.Email == email)
            .ConfigureAwait(false);
        return Maybe.From(user);
    }

    public async Task<ApiResult<LoginResponse>> AuthenticateUserAsync(string email, string password)
    {
        _logger.LogInformation("Authentication attempt for {Email}", email);

        Maybe<User> maybeUser = await FindByEmailAsync(email);

        if (maybeUser.IsNone)
        {
            _logger.LogWarning("Authentication failed: Invalid credentials for {Email}", email);
            return ApiResult<LoginResponse>.Error("Invalid credentials", 401);
        }

        User user = maybeUser.GetValueOrThrow();

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
                await _unitOfwork.SaveChangesAsync().ConfigureAwait(false);

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
                await _unitOfwork.SaveChangesAsync().ConfigureAwait(false);

                // Message includes current attempt and total allowed attempts
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

        string token = _jwtTokenService.GenerateToken(
            user.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
            user.Email,
            "User"
        );

        Update(user);
        await _unitOfwork.SaveChangesAsync().ConfigureAwait(false);

        LoginResponse userMapper = new(
            user.Email,
            user.FirstName,
            user.LastName,
            user.Phone,
            token
        );

        _logger.LogInformation(
            "User authenticated successfully: {Email}, UserId: {UserId}",
            email,
            user.Id
        );

        return ApiResult<LoginResponse>.Success(userMapper, "Login successful", 200);
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="request">The user registration data.</param>
    /// <returns>ApiResult with LoginResponse containing user data and JWT token.</returns>
    /// <remarks>
    /// Validates request using FluentValidation.
    /// Checks for duplicate email addresses.
    /// Hashes password using BCrypt.
    /// Automatically generates JWT token for immediate login.
    /// </remarks>
    public async Task<ApiResult<LoginResponse>> RegisterUserAsync(CreateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        try
        {
            _logger.LogInformation(
                "User creation attempt for {Email}, Name: {FirstName} {LastName}",
                request.Email,
                request.FirstName,
                request.LastName
            );

            // 1. Validate request
            ValidationResult validatorResult = await _validator
                .ValidateAsync(request)
                .ConfigureAwait(false);

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
            var maybeUser = await FindByEmailAsync(request.Email).ConfigureAwait(false);
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

            userToCreate.CreatedAt = DateTime.UtcNow;
            userToCreate.UpdatedAt = DateTime.UtcNow;
            userToCreate.IsActive = false;

            // 5. Persist to database
            Insert(userToCreate);
            await _unitOfwork.SaveChangesAsync().ConfigureAwait(false);

            // 6. Generate JWT token for automatic login
            string token = _jwtTokenService.GenerateToken(
                userToCreate.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                userToCreate.Email,
                "User"
            );

            LoginResponse response = new(
                userToCreate.Email,
                userToCreate.FirstName,
                userToCreate.LastName,
                userToCreate.Phone,
                token
            );

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
        catch (InvalidOperationException opEx)
        {
            _logger.LogError(opEx, "Operation error creating user {Email}", request.Email);
            return ApiResult<LoginResponse>.Error($"Error creating user: {opEx.Message}", 500);
        }
    }

    // TODO: Implementar la lógica para refrescar la autenticación
    public Task<ApiResult<object>> RefreshAuthenticationAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    // TODO: Implementar la lógica para revocar la autenticación
    public Task RevokeAuthenticationAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    // TODO: Crear la lógica para autenticar con Google
    // TODO: Implementar los loggers en este método
    public Task<ApiResult<object>> AuthenticateWithGoogleAsync(string idToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Generates a password reset token valid for 1 hour.
    /// </summary>
    public async Task InitiatePasswordResetAsync(string email)
    {
        _logger.LogInformation("Password reset requested for {Email}", email);

        var emailResult = Email.Validate(email);
        if (emailResult.IsFailure)
        {
            _logger.LogWarning("Invalid email format for password reset: {Email}", email);
            return;
        }

        var maybeUser = await FindByEmailAsync(email).ConfigureAwait(false);

        if (maybeUser.IsNone)
        {
            _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
            // Don't reveal if user exists for security
            return;
        }

        User user = maybeUser.Value;

        // Generate reset token
        user.PasswordResetToken = Guid.NewGuid().ToString("N");
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        Update(user);
        await _unitOfwork.SaveChangesAsync().ConfigureAwait(false);

        _logger.LogInformation(
            "Password reset token generated for user {UserId}. Token: {Token}",
            user.Id,
            user.PasswordResetToken
        );

        // TODO: Send email with reset link
        // await _emailService.SendPasswordResetEmailAsync(user.Email, user.PasswordResetToken);
    }

    /// <summary>
    /// Updates user profile. Password only updated if provided.
    /// </summary>
    public async Task<ApiResult<bool>> UpdateUserProfileAsync(EditUserRequest user)
    {
        ArgumentNullException.ThrowIfNull(user);
        try
        {
            _logger.LogInformation("Starting user profile update for ID: {}", user.Id);

            var validationResult = new[]
            {
                Email.Validate(user.Email),
                ValidateId(user.Id),
                Password.ValidatePasswordIfProvided(user.Password),
                FirstName.Validate(user.FirstName),
                LastName.Validate(user.LastName),
            };

            // Validate data
            string[] errors = GetErrors(validationResult);

            if (errors.Length > 0)
            {
                _logger.LogWarning(
                    "Validation failed when editing user {UserId}. Errors: {Errors}",
                    user.Id,
                    string.Join(", ", errors)
                );
                return ApiResult<bool>.Error(errors, 400);
            }

            Maybe<User> maybeUser = await FindAsync(user.Id).ConfigureAwait(false);

            if (maybeUser.IsNone)
            {
                _logger.LogWarning("User not found when editing: ID {UserId}", user.Id);
                return ApiResult<bool>.Error("User not found", 404);
            }

            User userEntity = maybeUser.Value;

            // Check if email is already in use by another user
            var maybeUserWithEmail = await FindByEmailAsync(user.Email).ConfigureAwait(false);

            if (maybeUserWithEmail.IsSome && maybeUserWithEmail.Value.Id != userEntity.Id)
            {
                _logger.LogWarning(
                    "Email {Email} is already in use by another user (ID {ExistingId})",
                    user.Email,
                    maybeUserWithEmail.Value.Id
                );
                return ApiResult<bool>.Error("Email already in use", 409);
            }

            // Update entity
            userEntity.UpdateProfile(user.FirstName, user.LastName, user.Phone, user.Email);

            // Only change password when a new one is provided
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                userEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _logger.LogInformation("Password updated for user {UserId}", userEntity.Id);
            }

            Update(userEntity);

            await _unitOfwork.SaveChangesAsync().ConfigureAwait(false);

            _logger.LogInformation("User {UserId} updated successfully", userEntity.Id);

            return ApiResult<bool>.Success(true, "User updated successfully", 200);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error updating user {UserId}", user.Id);
            return ApiResult<bool>.Error("Error saving changes to database", 500);
        }
        catch (InvalidOperationException opEx)
        {
            _logger.LogError(opEx, "Operation error editing user {UserId}", user.Id);
            return ApiResult<bool>.Error("Internal server error", 500);
        }
    }

    /// <summary>
    /// Resets password using token. Token expires after 1 hour.
    /// </summary>
    public async Task<ApiResult<bool>> ResetPasswordAsync(string token, string newPassword)
    {
        try
        {
            _logger.LogInformation("Password reset attempt with token: {Token}", token);

            var passwordResult = Password.Create(newPassword);
            if (passwordResult.IsFailure)
            {
                _logger.LogWarning("Invalid password format for reset");
                return ApiResult<bool>.Error(passwordResult.GetErrorOrThrow().Message, 400);
            }

            var user = await Queryable()
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token)
                .ConfigureAwait(false);

            if (user == null)
            {
                _logger.LogWarning("Invalid password reset token: {Token}", token);
                return ApiResult<bool>.Error("Invalid or expired reset token", 400);
            }

            if (
                user.PasswordResetTokenExpiry == null
                || user.PasswordResetTokenExpiry < DateTime.UtcNow
            )
            {
                _logger.LogWarning("Expired password reset token for user {UserId}", user.Id);
                return ApiResult<bool>.Error("Reset token has expired", 400);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            Update(user);
            await _unitOfwork.SaveChangesAsync().ConfigureAwait(false);

            _logger.LogInformation("Password reset successfully for user {UserId}", user.Id);

            return ApiResult<bool>.Success(true, "Password reset successfully", 200);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error resetting password");
            return ApiResult<bool>.Error("Internal server error", 500);
        }
    }

    // Validation methods that return Result<Unit>
    private static Result<Unit> ValidateId(int id)
    {
        if (id <= 0)
            return Result.Failure<Unit>("User ID must be greater than zero");

        return Result.Unit;
    }
}
