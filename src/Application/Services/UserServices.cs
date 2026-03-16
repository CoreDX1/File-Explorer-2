using Application.Configuration;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Mappings;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Monads;
using Domain.Monads.Result;
using Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrackableEntities.Common.Core;
using static Domain.Monads.Result.ResultExtensions;

namespace Application.Services;

public class UserServices : Service<User>, IUserServices
{
    private readonly ILogger<UserServices> _logger;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWorkAsync _unitOfwork;
    private readonly IValidator<CreateUserRequest> _validator;
    private readonly IOptions<LockoutOptions> _lockoutOptions;

    public UserServices(
        IRepositoryAsync<User> repository,
        IJwtTokenService jwtTokenService,
        IUnitOfWorkAsync unitOfwork,
        IValidator<CreateUserRequest> validator,
        ILogger<UserServices> logger,
        IOptions<LockoutOptions> lockoutOptions
    )
        : base(repository)
    {
        _jwtTokenService = jwtTokenService;
        _unitOfwork = unitOfwork;
        _validator = validator;
        _logger = logger;
        _lockoutOptions = lockoutOptions ?? throw new ArgumentNullException(nameof(lockoutOptions));
    }

    public async Task<ApiResult<UserResponse>> FindByIdAsync(int id)
    {
        _logger.LogInformation("Finding user by ID: {Id}", id);

        Maybe<User> user = await FindAsync(id).ConfigureAwait(false);

        if (user.IsNone)
        {
            _logger.LogWarning("User not found by ID: {Id}", id);
            return ApiResult<UserResponse>.Error("No se encontro al usuario", 501);
        }

        var userDto = user.Value.ToDto();

        _logger.LogInformation("User found by ID: {Id}", id);
        return ApiResult<UserResponse>.Success(userDto, "El usuario se encontro", 200);
    }

    // TODO: Implementar los loggers en este método
    public async Task<ApiResult<List<UserResponse>>> GetAllUsersAsync()
    {
        try
        {
            var query = Queryable().AsNoTracking().OrderBy(u => u.Id);

            List<User> users = await query.ToListAsync().ConfigureAwait(false);

            List<UserResponse> dto = users.ToDtos();

            _logger.LogInformation("Retrieved {Count} users", dto.Count);

            return ApiResult<List<UserResponse>>.Success(dto, "Users retrieved successfully", 200);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error retrieving users");
            return ApiResult<List<UserResponse>>.Error("Database error occurred", 500);
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

        // Validate user has password hash
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            _logger.LogError("User {Email} has no password hash set", email);
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

            var lockoutConfig = _lockoutOptions?.Value ?? new LockoutOptions();

            if (user.FailedLoginAttemts >= lockoutConfig.MaxFailedAccessAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.Add(lockoutConfig.LockoutTimeSpan);
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
                    $"Account locked due to too many failed attempts. Attempt {user.FailedLoginAttemts} of {lockoutConfig.MaxFailedAccessAttempts}. Try again later.",
                    403
                );
            }
            else
            {
                int remainingAttempts =
                    lockoutConfig.MaxFailedAccessAttempts - user.FailedLoginAttemts;

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
                    $"Invalid credentials. Attempt {user.FailedLoginAttemts} of {lockoutConfig.MaxFailedAccessAttempts}.",
                    401
                );
            }
        }

        // Successful authentication: reset counters
        user.FailedLoginAttemts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;

        // 1. Generar Access Token
        string accessToken = _jwtTokenService.GenerateToken(user.Id, user.Email, "User");

        // 2. Generar Refresh Token
        string refreshTokenValue = GenerateSecureRefreshToken();

        // 3. Crear entidad Refresh Token para guardar en DB

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Expire = DateTime.UtcNow.AddDays(7), // Duración larga (ej. 7 días)
            Created = DateTime.UtcNow,
            Revoked = null,
            ReasonRevoked = null,
            ReplacedByToken = null,
            TrackingState = TrackingState.Added,
        };

        _unitOfwork.RefreshTokenRepository.Insert(newRefreshToken);

        user.TrackingState = TrackingState.Modified;

        Update(user);

        await _unitOfwork.SaveChangesAsync().ConfigureAwait(false);

        LoginResponse userMapper = new(
            user.Email,
            user.FirstName,
            user.LastName,
            user.Phone,
            accessToken,
            refreshTokenValue
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
            userToCreate.Id = Guid.NewGuid();
            userToCreate.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            userToCreate.CreatedAt = DateTime.UtcNow;
            userToCreate.UpdatedAt = DateTime.UtcNow;
            userToCreate.IsActive = false;

            // 5. Persist to database
            Insert(userToCreate);
            await _unitOfwork.SaveChangesAsync().ConfigureAwait(false);

            // 6. Generate JWT token for automatic login
            string accessToken = _jwtTokenService.GenerateToken(
                userToCreate.Id,
                userToCreate.Email,
                "User"
            );

            // 7. Generate Refresh Token
            string refreshTokenValue = GenerateSecureRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                UserId = userToCreate.Id,
                Token = refreshTokenValue,
                Expire = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                TrackingState = TrackingState.Added,
            };

            _unitOfwork.RefreshTokenRepository.Insert(newRefreshToken);
            await _unitOfwork.SaveChangesAsync().ConfigureAwait(false);

            LoginResponse response = new(
                userToCreate.Email,
                userToCreate.FirstName,
                userToCreate.LastName,
                userToCreate.Phone,
                accessToken,
                refreshTokenValue
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
    public async Task<ApiResult<LoginResponse>> RefreshAuthenticationAsync(
        string refreshToken,
        Guid userId
    )
    {
        _logger.LogInformation(
            "Refresh authentication attempt for UserId: {UserId} with RefreshToken: {RefreshToken}",
            userId,
            refreshToken
        );

        var maybeCurrenToken = await FindRefreshTokenActiveAsync(userId, refreshToken);
        if (maybeCurrenToken.IsNone)
        {
            _logger.LogWarning(
                "Refresh failed: Invalid or inactive token for UserId {UserId}",
                userId
            );

            return ApiResult<LoginResponse>.Error("Invalid or expired refresh token", 401);
        }

        RefreshToken currentToken = maybeCurrenToken.GetValueOrThrow();
        if (currentToken.IsActive)
        {
            // Caso especial: ¿Fue revocado por rotación previa? (Posible ataque de replay)
            if (currentToken.IsRevoked && !string.IsNullOrEmpty(currentToken.ReplacedByToken))
            {
                _logger.LogError(
                    "SECURITY ALERT: Reuse of revoked refresh token detected for UserId {UserId}. Token ID: {TokenId}",
                    userId,
                    currentToken.Id
                );
                // Opcional: Aquí podrías revocar TODA la familia de tokens de este usuario por seguridad.
            }

            return ApiResult<LoginResponse>.Error("Refresh token has been revoked or expired", 403);
        }

        try
        {
            // --- PASO A: Revocar el token actual ---
            var newRefreshTokenValue = GenerateSecureRefreshToken();

            currentToken.Revoked = DateTime.UtcNow;
            currentToken.ReasonRevoked = "Rotated";
            currentToken.ReplacedByToken = newRefreshTokenValue;
            currentToken.TrackingState = TrackingState.Modified;
            _unitOfwork.RefreshTokenRepository.ApplyChanges(currentToken);

            // --- PASO B: Crear el nuevo Refresh Token ---
            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = userId,
                Token = newRefreshTokenValue,
                Expire = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                Revoked = null,
                TrackingState = TrackingState.Added,
            };

            _unitOfwork.RefreshTokenRepository.Insert(newRefreshTokenEntity);

            // PASO C: Generar nuevo Access Token ---
            User? user = currentToken.User;
            if (user == null)
            {
                user = await FindAsync(userId);
                if (user == null)
                    throw new Exception("Usuario no encontrado durante refresh");
            }

            string newAccessToken = _jwtTokenService.GenerateToken(user.Id, user.Email, "User");

            // --- PASO D: Persistir cambios ---
            await _unitOfwork.SaveChangesAsync();

            _logger.LogInformation(
                "Token refreshed successfully for UserId {UserId}. New Token Expires: {Expire}",
                userId,
                newRefreshTokenEntity.Expire
            );

            // Mapear respuesta (igual que en Login)
            var response = new LoginResponse(
                user.Email,
                user.FirstName,
                user.LastName,
                user.Phone,
                newAccessToken,
                newRefreshTokenValue // Asegúrate que LoginResponse tenga esta propiedad
            );

            return ApiResult<LoginResponse>.Success(response, "Token refreshed successfully", 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while refreshing token for UserId {UserId}",
                userId
            );
            // El 'await using' de la transacción hará Rollback automático
            return ApiResult<LoginResponse>.Error(
                "Internal server error during token refresh",
                500
            );
        }
    }

    public async Task<Maybe<RefreshToken>> FindRefreshTokenActiveAsync(
        Guid userId,
        string tokenValue
    )
    {
        var token = await _unitOfwork
            .RefreshTokenRepository.Queryable()
            .Where(rt => rt.UserId == userId && rt.Token == tokenValue && rt.IsActive)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return token != null ? Maybe.From(token) : Maybe.None<RefreshToken>();
    }

    // Generador criptografico seguro

    public static string GenerateSecureRefreshToken()
    {
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var bytes = new byte[64];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
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
