namespace Application.Options;

public abstract class Option<T>
{
    private Option() { }

    public sealed class Some : Option<T>
    {
        public T Value { get; }

        public Some(T value) => Value = value;
    }

    public sealed class None : Option<T> { }

    public static Option<T> FromValue(T value) => new Some(value);

    public static Option<T> Empty() => new None();
}

public static class Option
{
    public static Option<T> Some<T>(T value) => Option<T>.FromValue(value);

    public static Option<T> None<T>() => Option<T>.Empty();
}
