using Domain.Monads.Result;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Builder para construir instancias de User de forma fluida y validada
/// </summary>
public class UserBuilder
{
    private int _id;
    private string? _firstName;
    private string? _lastName;
    private string? _phone;
    private string? _email;
    private string? _passwordHash;
    private long _storageQuotaBytes = 5368709120; // 5 GB default
    private bool _isActive = true;
    private DateTime? _createdAt;
    private DateTime? _updatedAt;
    private DateTime? _lastLoginAt;

    private readonly List<string> _errors = [];

    public UserBuilder() { }

    /// <summary>
    /// Crea un builder a partir de un usuario existente
    /// </summary>
    public static UserBuilder FromExisting(User user)
    {
        return new UserBuilder
        {
            _id = user.Id,
            _firstName = user.FirstName,
            _lastName = user.LastName,
            _phone = user.Phone,
            _email = user.Email,
            _passwordHash = user.PasswordHash,
            _storageQuotaBytes = user.StorageQuotaBytes,
            _isActive = user.IsActive,
            _createdAt = user.CreatedAt,
            _updatedAt = user.UpdatedAt,
            _lastLoginAt = user.LastLoginAt,
        };
    }

    public UserBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithFirstName(string firstName)
    {
        var result = FirstName.Validate(firstName);
        if (result.IsFailure)
            _errors.Add(result.GetErrorOrThrow().Message);
        else
            _firstName = firstName;

        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        var result = LastName.Validate(lastName);
        if (result.IsFailure)
            _errors.Add(result.GetErrorOrThrow().Message);
        else
            _lastName = lastName;

        return this;
    }

    public UserBuilder WithName(string firstName, string lastName)
    {
        WithFirstName(firstName);
        WithLastName(lastName);
        return this;
    }

    public UserBuilder WithPhone(string phone)
    {
        var result = Phone.Validate(phone);
        if (result.IsFailure)
            _errors.Add(result.GetErrorOrThrow().Message);
        else
            _phone = phone;

        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        var result = Email.Validate(email);
        if (result.IsFailure)
            _errors.Add(result.GetErrorOrThrow().Message);
        else
            _email = email;

        return this;
    }

    public UserBuilder WithPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            _errors.Add("Password hash cannot be empty");
        else
            _passwordHash = passwordHash;

        return this;
    }

    public UserBuilder WithStorageQuota(long bytes)
    {
        var result = StorageQuota.Create(bytes);
        if (result.IsFailure)
            _errors.Add(result.GetErrorOrThrow().Message);
        else
            _storageQuotaBytes = bytes;

        return this;
    }

    public UserBuilder WithStorageQuota(StorageQuota quota)
    {
        _storageQuotaBytes = quota.Bytes;
        return this;
    }

    public UserBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public UserBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public UserBuilder WithUpdatedAt(DateTime updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    public UserBuilder WithLastLoginAt(DateTime lastLoginAt)
    {
        _lastLoginAt = lastLoginAt;
        return this;
    }

    /// <summary>
    /// Construye el usuario validando que todos los campos requeridos estén presentes
    /// </summary>
    public Result<User> Build()
    {
        // Validar campos requeridos
        if (string.IsNullOrWhiteSpace(_firstName))
            _errors.Add("First name is required");

        if (string.IsNullOrWhiteSpace(_lastName))
            _errors.Add("Last name is required");

        if (string.IsNullOrWhiteSpace(_email))
            _errors.Add("Email is required");

        if (string.IsNullOrWhiteSpace(_passwordHash))
            _errors.Add("Password hash is required");

        // Si hay errores, retornar fallo
        if (_errors.Count > 0)
            return Result.Failure<User>(string.Join("; ", _errors));

        var now = DateTime.UtcNow;

        var user = new User
        {
            Id = _id,
            FirstName = _firstName!,
            LastName = _lastName!,
            Phone = _phone ?? string.Empty,
            Email = _email!,
            PasswordHash = _passwordHash!,
            StorageQuotaBytes = _storageQuotaBytes,
            IsActive = _isActive,
            CreatedAt = _createdAt ?? now,
            UpdatedAt = _updatedAt ?? now,
            LastLoginAt = _lastLoginAt ?? now,
        };

        return Result.Success(user);
    }

    /// <summary>
    /// Construye el usuario sin validaciones estrictas (útil para actualización parcial)
    /// </summary>
    public User BuildUnsafe()
    {
        var now = DateTime.UtcNow;

        return new User
        {
            Id = _id,
            FirstName = _firstName ?? string.Empty,
            LastName = _lastName ?? string.Empty,
            Phone = _phone ?? string.Empty,
            Email = _email ?? string.Empty,
            PasswordHash = _passwordHash ?? string.Empty,
            StorageQuotaBytes = _storageQuotaBytes,
            IsActive = _isActive,
            CreatedAt = _createdAt ?? now,
            UpdatedAt = _updatedAt ?? now,
            LastLoginAt = _lastLoginAt ?? now,
        };
    }

    /// <summary>
    /// Limpia los errores acumulados
    /// </summary>
    public UserBuilder ClearErrors()
    {
        _errors.Clear();
        return this;
    }

    /// <summary>
    /// Obtiene los errores de validación acumulados
    /// </summary>
    public IReadOnlyList<string> GetErrors() => _errors.AsReadOnly();
}
