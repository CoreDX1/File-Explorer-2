using System.Text.RegularExpressions;
using Domain.Monads;
using Domain.Monads.Result;

namespace Domain.ValueObjects;

public sealed record Phone
{
    public string Value { get; }

    private Phone(string value) => Value = value;

    public static Result<Phone> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Phone>("Phone number is required");

        var cleaned = Regex.Replace(value, @"[^\d+]", "");

        if (cleaned.Length < 8 || cleaned.Length > 15)
            return Result.Failure<Phone>("Phone number must be between 8 and 15 digits");

        return Result.Success(new Phone(cleaned));
    }

    public static Result<Unit> Validate(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Result.Failure<Unit>("Phone number is required");

        var cleaned = Regex.Replace(phone, @"[^\d+]", "");

        if (cleaned.Length < 8 || cleaned.Length > 15)
            return Result.Failure<Unit>("Phone number must be between 8 and 15 digits");

        return Result.Unit;
    }

    public static implicit operator string(Phone phone) =>
        phone?.Value ?? throw new ArgumentNullException(nameof(phone));
}
