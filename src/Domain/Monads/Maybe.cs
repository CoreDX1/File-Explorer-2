namespace Domain.Monads;

public static class Maybe
{
    public static Maybe<T> From<T>(T? value) => new(value);

    public static Maybe<T> Some<T>(T value) =>
        value is null ? throw new ArgumentNullException(nameof(value)) : new(value);

    public static Maybe<T> None<T>() => Maybe<T>.None;
}
