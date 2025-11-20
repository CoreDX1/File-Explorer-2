namespace Domain.Monads;

public class MaybeNoneException : Exception
{
    public MaybeNoneException()
        : base("Cannot access value of None") { }

    public MaybeNoneException(string message)
        : base(message) { }

    public MaybeNoneException(string message, Exception innerException)
        : base(message, innerException) { }
}
