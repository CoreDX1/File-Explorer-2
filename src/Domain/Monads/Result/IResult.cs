using Domain.ValitMona.Error;

namespace Domain.Monads.Result;

public interface IResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    public Errors GetErrorOrThrow();
}

public interface IResult<in TValue> : IResult { }
