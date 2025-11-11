namespace Domain.Monads;

public class MaybeNoneException : Exception
{
    public MaybeNoneException()
        : base("Cannot access value of None") { }

    public MaybeNoneException(string message)
        : base(message) { }
}
