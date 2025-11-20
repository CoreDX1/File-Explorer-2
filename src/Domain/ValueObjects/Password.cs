using Domain.Monads;
using Domain.Monads.Result;

namespace Domain.ValueObjects;

public sealed record Password
{
    public string Value { get; }

    private Password(string value) => Value = value;

    public static Result<Password> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 8)
            return Result.Failure<Password>("Password must be at least 8 characters");

        return Result.Success(new Password(value));
    }

    public static Result<Unit> ValidatePasswordIfProvided(string password)
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

        if (!password.Any(c => "@$!%*?&#^_-".Contains(c, StringComparison.Ordinal)))
            return Result.Failure<Unit>(
                "La contraseña debe contener al menos un carácter especial (@$!%*?&#^_-)"
            );

        return Result.Unit;
    }

    public static implicit operator string(Password password) => password?.Value ?? throw new ArgumentNullException(nameof(password));
}
