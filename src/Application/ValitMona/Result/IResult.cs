using Application.ValitMona.Error;

namespace Application.ValitMona.Result;

public interface IResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    public Errors GetErrorOrThrow();
}

public interface IResult<in TValue> : IResult { }
