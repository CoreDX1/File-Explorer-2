namespace Application.Options;

public abstract class Optional<T>
{
    private Optional() { }

    internal sealed class Some(T value) : Optional<T>
    {
        public T Value { get; } = value;
    }

    internal sealed class None : Optional<T> { }
}

public static class Optional
{
    public static Optional<T> Some<T>(T value) => new Optional<T>.Some(value);

    public static Optional<T> None<T>() => new Optional<T>.None();
}
