namespace Domain.ValitMona.Error;

public abstract class Errors
{
    public abstract string Message { get; }
}

public class LogicError : Errors
{
    public override string Message { get; }

    public LogicError(string message) => Message = message;
}
