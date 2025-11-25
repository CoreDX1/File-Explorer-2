using Domain.Monads;
using Domain.Monads.Result;
using Domain.ValueObjects;

namespace Domain.Entities;

public class User : Entity
{
    // Propiedades primitivas para EF Core
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public long StorageQuotaBytes { get; set; } = 5368709120; // 5 GB default quota

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime LastLoginAt { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }

    // Propiedades calculadas
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

    // Value Objects (propiedades de solo lectura)
    public Result<ValueObjects.Email> GetEmail() => ValueObjects.Email.Create(Email);
    public Result<ValueObjects.FirstName> GetFirstName() => ValueObjects.FirstName.Create(FirstName);
    public Result<ValueObjects.LastName> GetLastName() => ValueObjects.LastName.Create(LastName);
    public Result<ValueObjects.Phone> GetPhone() => ValueObjects.Phone.Create(Phone);
    public Result<StorageQuota> GetStorageQuota() => StorageQuota.Create(StorageQuotaBytes);

    /// <summary>
    /// Actualiza el perfil del usuario con validación
    /// </summary>
    public Result<Unit> UpdateProfile(string firstName, string lastName, string phone, string email)
    {
        var firstNameResult = ValueObjects.FirstName.Validate(firstName);
        if (firstNameResult.IsFailure)
            return firstNameResult;

        var lastNameResult = ValueObjects.LastName.Validate(lastName);
        if (lastNameResult.IsFailure)
            return lastNameResult;

        var phoneResult = ValueObjects.Phone.Validate(phone);
        if (phoneResult.IsFailure)
            return phoneResult;

        var emailResult = ValueObjects.Email.Validate(email);
        if (emailResult.IsFailure)
            return emailResult;

        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
        UpdatedAt = DateTime.UtcNow;

        return Result.Unit;
    }

    /// <summary>
    /// Actualiza la contraseña del usuario
    /// </summary>
    public Result<Unit> UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            return Result.Failure<Monads.Unit>("Password hash cannot be empty");

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;

        return Result.Unit;
    }

    /// <summary>
    /// Registra un intento de login fallido
    /// </summary>
    public void RecordFailedLogin(LockoutOptions options)
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= options.MaxFailedAccessAttempts)
        {
            LockoutEnd = DateTime.UtcNow.Add(options.DefaultLockoutTimeSpan);
        }
    }

    /// <summary>
    /// Resetea los intentos de login fallidos
    /// </summary>
    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }

    /// <summary>
    /// Actualiza el último login
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        ResetFailedLoginAttempts();
    }

    /// <summary>
    /// Genera un token de reseteo de contraseña
    /// </summary>
    public void GeneratePasswordResetToken(TimeSpan expiryDuration)
    {
        PasswordResetToken = Guid.NewGuid().ToString("N");
        PasswordResetTokenExpiry = DateTime.UtcNow.Add(expiryDuration);
    }

    /// <summary>
    /// Valida si el token de reseteo es válido
    /// </summary>
    public bool IsPasswordResetTokenValid(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(PasswordResetToken))
            return false;

        if (PasswordResetToken != token)
            return false;

        if (!PasswordResetTokenExpiry.HasValue || PasswordResetTokenExpiry.Value < DateTime.UtcNow)
            return false;

        return true;
    }

    /// <summary>
    /// Limpia el token de reseteo de contraseña
    /// </summary>
    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
    }

    /// <summary>
    /// Verifica si el usuario puede asignar espacio de almacenamiento
    /// </summary>
    public bool CanAllocateStorage(long usedBytes, long requestedBytes)
    {
        var quotaResult = GetStorageQuota();
        if (quotaResult.IsFailure)
            return false;

        return quotaResult.Value.CanAllocate(usedBytes, requestedBytes);
    }

    /// <summary>
    /// Actualiza la cuota de almacenamiento
    /// </summary>
    public Result<Unit> UpdateStorageQuota(long newQuotaBytes)
    {
        var quotaResult = StorageQuota.Create(newQuotaBytes);
        if (quotaResult.IsFailure)
            return Result.Failure<Unit>(quotaResult.GetErrorOrThrow().Message);

        StorageQuotaBytes = newQuotaBytes;
        UpdatedAt = DateTime.UtcNow;

        return Result.Unit;
    }

    /// <summary>
    /// Activa el usuario
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Desactiva el usuario
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

public class LockoutOptions
{
    public bool AllowedForNewUser { get; set; } = true;
    public int MaxFailedAccessAttempts { get; set; } = 5;
    public TimeSpan DefaultLockoutTimeSpan { get; set; } = TimeSpan.FromMinutes(5.0);
}

