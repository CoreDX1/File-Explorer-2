using System.Text.RegularExpressions;
using Domain.Monads;
using Domain.Monads.Result;

namespace Domain.ValueObjects;

public sealed record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public string Value { get; }

    private Email(string value) => Value = value.ToLowerInvariant();

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Email>("Email is required");

        if (value.Length > 254)
            return Result.Failure<Email>("Email cannot exceed 254 characters");

        if (!EmailRegex.IsMatch(value))
            return Result.Failure<Email>("Invalid email format");

        return Result.Success(new Email(value));
    }

    public static Result<Unit> Validate(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<Unit>("Email is required");

        if (email.Length > 254)
            return Result.Failure<Unit>("Email cannot exceed 254 characters");

        if (!EmailRegex.IsMatch(email))
            return Result.Failure<Unit>("Invalid email format");

        return Result.Unit;
    }

    public static implicit operator string(Email email) =>
        email?.Value ?? throw new ArgumentNullException(nameof(email));
}
