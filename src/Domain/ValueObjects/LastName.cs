using System.Text.RegularExpressions;
using Domain.Monads;
using Domain.Monads.Result;

namespace Domain.ValueObjects;

public sealed record LastName
{
    public string Value { get; }

    private LastName(string value) => Value = value;

    public static Result<LastName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LastName>("Last name is required");

        if (value.Length > 50)
            return Result.Failure<LastName>("Last name must not exceed 50 characters");

        if (!Regex.IsMatch(value, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            return Result.Failure<LastName>("Last name can only contain letters");

        return Result.Success(new LastName(value));
    }

    public static Result<Unit> Validate(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<Unit>("Last name is required");

        if (lastName.Length > 50)
            return Result.Failure<Unit>("Last name must not exceed 50 characters");

        if (!Regex.IsMatch(lastName, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            return Result.Failure<Unit>("Last name can only contain letters");

        return Result.Unit;
    }

    public static implicit operator string(LastName lastName) => lastName.Value;
}
