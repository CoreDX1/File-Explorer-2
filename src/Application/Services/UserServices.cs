using System.Text.RegularExpressions;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interface;
using Domain.Entities;
using Domain.Monads;
using Domain.Monads.Result;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Interface;
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
        User? user = await FindAsync(id);

        GetUserResponseUnique userDto = new()
        {
            Email = user.Email,
            FirstName = user.FirstName,
            Id = user.Id,
            LastName = user.LastName,
            Phone = user.Phone,
        };

        if (user == null)
            return ApiResult<GetUserResponseUnique>.Error("No se encontro al usuario", 501);

        return ApiResult<GetUserResponseUnique>.Success(userDto, "El usuario se encontro", 200);
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

    private Result<Unit> ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            return Result.Failure<Unit>("Invalid email adress");

        return Result.Unit;
    }

    private Result<Unit> ValidatePassword(string pass)
    {
        if (pass.Length < 8)
            return Result.Failure<Unit>("Password must be at least 8 characters");

        return Result.Unit;
    }

    public async Task<Maybe<User>> FindByEmailAsync(string email)
    {
        User? user = await Queryable().FirstOrDefaultAsync(u => u.Email == email);
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

        // var demo = ValidateEmail(email)
        //     .Bind(() => ValidatePassword(password))
        //     .Map(_ => new CreateUserRequest(email, password));

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

    public async Task<ApiResult<LoginResponse>> RegisterUserAsync(CreateUserRequest request)
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

    public Task<ApiResult<object>> RefreshAuthenticationAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task RevokeAuthenticationAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<object>> AuthenticateWithGoogleAsync(string idToken)
    {
        throw new NotImplementedException();
    }

    public Task InitiatePasswordResetAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResult<bool>> UpdateUserProfileAsync(EditUserRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando de edicion del usuario ID: {}", request.Id);

            var validationResult = new[]
            {
                ValidateEmail(request.Email),
                ValidateId(request.Id),
                ValidatePasswordIfProvided(request.Password),
                ValidateFirstName(request.FirstName),
                ValidateLastName(request.LastName),
            };

            // Validando los datos
            string[] errors = GetErrors(validationResult);

            if (errors.Length > 0)
            {
                _logger.LogWarning(
                    "Validación fallida al editar usuario {UserId}. Errores: {Errors}",
                    request.Id,
                    string.Join(", ", errors)
                );
                return ApiResult<bool>.Error(errors, 400);
            }

            Maybe<User> maybeUser = await FindAsync(request.Id);

            if (maybeUser.IsNone)
            {
                _logger.LogWarning("Usuario no encontrado al editar: ID {UserId}", request.Id);
                return ApiResult<bool>.Error("User not found", 404);
            }

            User user = maybeUser.Value;

            // Verificando si el email esta duplicado
            var maybeUserWithEmail = await FindByEmailAsync(request.Email);

            if (maybeUserWithEmail.IsSome && maybeUserWithEmail.Value.Id != user.Id)
            {
                _logger.LogWarning(
                    "Email {Email} ya está en uso por otro usuario (ID {ExistingId})",
                    request.Email,
                    maybeUserWithEmail.Value.Id
                );
                return ApiResult<bool>.Error("Email duplicado", 409);
            }

            // Actualizando la entidad
            user.UpdateProfile(request.FirstName, request.LastName, request.Phone, request.Email);

            // Solo cambiar la contraseña cuando se proporcione una nueva
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                _logger.LogInformation("Contraseña actualizada para usuario {UserId}", user.Id);
            }

            Update(user);

            await _unitOfwork.SaveChangesAsync();

            _logger.LogInformation("Usuario {UserId} actualizado correctamente", user.Id);

            return ApiResult<bool>.Success(true, "Usuario actualizado correctamente", 200);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(
                dbEx,
                "Error de base de datos al actualizar usuario {UserId}",
                request.Id
            );
            return ApiResult<bool>.Error("Error al guardar los cambios en la base de datos", 500);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al editar usuario {UserId}", request.Id);
            return ApiResult<bool>.Error("Error interno del servidor", 500);
        }
    }

    private static Result<Unit> ValidatePasswordIfProvided(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return Result.Unit; // No se valida si no se envía

        if (password.Length < 8)
            return Result.Failure<Unit>("La contraseña debe tener al menos 8 caracteres");

        if (password.Length > 100)
            return Result.Failure<Unit>("La contraseña no puede exceder los 100 caracteres");

        if (!password.Any(char.IsUpper))
            return Result.Failure<Unit>("La contraseña debe contener al menos una mayúscula");

        if (!password.Any(char.IsLower))
            return Result.Failure<Unit>("La contraseña debe contener al menos una minúscula");

        if (!password.Any(char.IsDigit))
            return Result.Failure<Unit>("La contraseña debe contener al menos un número");

        if (!password.Any(c => "@$!%*?&#^_-".Contains(c)))
            return Result.Failure<Unit>(
                "La contraseña debe contener al menos un carácter especial (@$!%*?&#^_-)"
            );

        return Result.Unit;
    }

    // Métodos de validación que devuelven Result<Unit>
    private static Result<Unit> ValidateId(int id)
    {
        if (id <= 0)
            return Result.Failure<Unit>("User ID must be greater than zero");

        return Result.Unit;
    }

    private static Result<Unit> ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<Unit>("First name is required");

        if (firstName.Length > 50)
            return Result.Failure<Unit>("First name must not exceed 50 characters");

        if (!System.Text.RegularExpressions.Regex.IsMatch(firstName, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            return Result.Failure<Unit>("First name can only contain letters");

        return Result.Unit;
    }

    private static Result<Unit> ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<Unit>("Last name is required");

        if (lastName.Length > 50)
            return Result.Failure<Unit>("Last name must not exceed 50 characters");

        if (!Regex.IsMatch(lastName, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            return Result.Failure<Unit>("Last name can only contain letters");

        return Result.Unit;
    }

    private static Result<Unit> ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Result.Failure<Unit>("Phone is required");

        if (!Regex.IsMatch(phone, @"^\+?[1-9]\d{1,14}$"))
            return Result.Failure<Unit>("Invalid phone format (use international format)");

        if (phone.Length > 15)
            return Result.Failure<Unit>("Phone must not exceed 15 characters");

        return Result.Unit;
    }

    // Método auxiliar para validar email
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
