namespace Domain.Monads;

/// <summary>
/// Representa un valor opcional.
/// </summary>
public readonly struct Maybe<TValue> : IEquatable<Maybe<TValue>>
{
    public bool IsSome => Value is not null;
    public bool IsNone => Value is null;
    public static readonly Maybe<TValue> None = default;
    public TValue? Value { get; }

    public Maybe(TValue? value) => Value = value;

    /// <summary>
    /// Obtiene el valor o lanza excepción si es None.
    /// </summary>
    public TValue GetValueOrThrow() => Value ?? throw new MaybeNoneException();

    /// <summary>
    /// Obtiene el valor o retorna el valor por defecto.
    /// </summary>
    public TValue GetValueOr(TValue defaultValue) => Value ?? defaultValue;

    /// <summary>
    /// Obtiene el valor o ejecuta la función por defecto.
    /// </summary>
    public TValue GetValueOr(Func<TValue> defaultValue) => Value ?? defaultValue();

    /// <summary>
    /// Transforma el valor si existe.
    /// </summary>
    public Maybe<TNew> Map<TNew>(Func<TValue, TNew?> map) =>
        IsSome ? new Maybe<TNew>(map(Value!)) : Maybe<TNew>.None;

    /// <summary>
    /// Transforma el valor si existe (async).
    /// </summary>
    public async Task<Maybe<TNew>> Map<TNew>(Func<TValue, Task<TNew?>> map) =>
        IsSome ? new Maybe<TNew>(await map(Value!)) : Maybe<TNew>.None;

    /// <summary>
    /// Ejecuta una acción si el valor existe.
    /// </summary>
    public Maybe<TValue> IfSome(Action<TValue> action)
    {
        if (IsSome)
            action(Value!);
        return this;
    }

    /// <summary>
    /// Ejecuta una acción si el valor existe (async).
    /// </summary>
    public async Task<Maybe<TValue>> IfSome(Func<TValue, Task> action)
    {
        if (IsSome)
            await action(Value!);
        return this;
    }

    /// <summary>
    /// Ejecuta una función según el estado (Some o None).
    /// </summary>
    public TResult Match<TResult>(Func<TValue, TResult> some, Func<TResult> none) =>
        IsSome ? some(Value!) : none();

    public bool Equals(Maybe<TValue> other) => IsSome ? Value!.Equals(other.Value) : other.IsNone;

    public override bool Equals(object? obj) => obj is Maybe<TValue> other && Equals(other);

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    public override string ToString() => IsSome ? $"Some({Value})" : "None";

    public static bool operator ==(Maybe<TValue> left, Maybe<TValue> right) => left.Equals(right);

    public static bool operator !=(Maybe<TValue> left, Maybe<TValue> right) => !left.Equals(right);

    public static implicit operator Maybe<TValue>(TValue? value) => new(value);
}
