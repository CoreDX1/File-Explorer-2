using Domain.Monads;
using Domain.Monads.Result;

namespace Domain.ValueObjects;

public sealed record FirstName
{
    public string Value { get; }

    private FirstName(string value) => Value = value;

    public static Result<FirstName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<FirstName>("First name is required");

        if (value.Length > 50)
            return Result.Failure<FirstName>("First name must not exceed 50 characters");

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            return Result.Failure<FirstName>("First name can only contain letters");

        return Result.Success(new FirstName(value));
    }

    public static Result<Unit> Validate(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<Unit>("First name is required");

        if (firstName.Length > 50)
            return Result.Failure<Unit>("First name must not exceed 50 characters");

        if (!System.Text.RegularExpressions.Regex.IsMatch(firstName, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            return Result.Failure<Unit>("First name can only contain letters");

        return Result.Unit;
    }

    public static implicit operator string(FirstName firstName) => firstName?.Value ?? throw new ArgumentNullException(nameof(firstName));
}
