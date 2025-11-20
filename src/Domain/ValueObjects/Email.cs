using Domain.Monads;
using Domain.Monads.Result;

namespace Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@', StringComparison.Ordinal))
            return Result.Failure<Email>("Invalid email adress");

        return Result.Success(new Email(value));
    }

    public static Result<Unit> Validate(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@', StringComparison.Ordinal))
            return Result.Failure<Unit>("Invalid email adress");

        return Result.Unit;
    }

    public static implicit operator string(Email email) => email?.Value ?? throw new ArgumentNullException(nameof(email));
}
