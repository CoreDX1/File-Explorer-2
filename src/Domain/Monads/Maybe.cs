using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Domain.Monads;

public static class Maybe
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Maybe<T> From<T>(T? value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Maybe<T> From<T>(Maybe<T> maybe) => new(maybe);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Maybe<T> Some<T>(T value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        return new(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Maybe<T> Some<T>(Maybe<T> maybe)
    {
        if (maybe.IsNone)
            throw new MaybeNoneException();

        return new(maybe);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Maybe<T> None<T>() => Maybe<T>.None;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Maybe<Unit> None() => Maybe<Unit>.None;
}
