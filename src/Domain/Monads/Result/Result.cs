using Domain.Monads.Error;

namespace Domain.Monads.Result;

public static class Result
{
    public static Result<Unit> Unit { get; } = new(Monads.Unit.Instance);

    public static Result<T> Success<T>(T value) => new(value);

    public static Result<T> Failure<T>(Errors error) => new(error);

    public static Result<T> Failure<T>(string message) => new(new LogicError(message));
}

public readonly struct Result<T> : IResult<T>
{
    private readonly T? _value;
    private readonly Errors? _error;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Result(T value)
    {
        _value = value;
        _error = default;
        IsSuccess = true;
    }

    public Result(Errors error)
    {
        _value = default;
        _error = error;
        IsSuccess = false;
    }

    // public Result(List<Errors> error)
    // {
    //     _value = default;
    //     _Listerror.Add(error);
    //     IsSuccess = false;
    // }

    public T Value => IsSuccess ? _value : throw new InvalidOperationException("No value");

    public Errors GetErrorOrThrow()
    {
        return IsSuccess ? throw new InvalidOperationException("No error") : _error;
    }

    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        return IsSuccess ? new Result<TNew>(mapper(_value)) : new Result<TNew>(_error);
    }

    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
    {
        return IsSuccess ? binder(_value) : new Result<TNew>(_error);
    }

    public Result<TNew> Bind<TNew>(Func<Result<TNew>> binder)
    {
        return IsSuccess ? binder() : new Result<TNew>(_error);
    }

    // public TResult Match<TResult>(Func<T, TResult> success, Func<Errors, TResult> failure) =>
    //     IsSuccess ? success(_value) : failure(_error);

    // public Result<T> Ensure(Func<T, bool> predicate, Errors error) =>
    //     IsFailure ? this
    //     : predicate(_value) ? this
    //     : new Result<T>(error);

    public Result<T> IfSuccess(Action<T> action)
    {
        if (IsSuccess)
            action(_value);
        return this;
    }

    public Result<T> IfFailure(Action<Errors> action)
    {
        if (IsFailure)
            action(_error);
        return this;
    }

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Errors error) => new(error);

    public override bool Equals(object? obj) => obj is Result<T> other && Equals(other);

    public bool Equals(Result<T> other) =>
        IsSuccess == other.IsSuccess
        && (
            IsSuccess
                ? EqualityComparer<T>.Default.Equals(_value, other._value)
                : EqualityComparer<Errors>.Default.Equals(_error, other._error)
        );

    public override int GetHashCode() => HashCode.Combine(IsSuccess, _value, _error);

    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);
}
