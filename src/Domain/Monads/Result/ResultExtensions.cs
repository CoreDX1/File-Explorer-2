using Domain.ValitMona.Error;

namespace Domain.Monads.Result;

public static class ResultExtensions
{
    public static Result<Unit> Combine(params Result<Unit>[] results)
    {
        var errors = results
            .Where(r => r.IsFailure)
            .Select(r => r.GetErrorOrThrow().Message)
            .ToList();

        return errors.Any() ? Result.Failure<Unit>(string.Join(", ", errors)) : Result.Unit;
    }

    public static string[] GetErrors(params Result<Unit>[] results)
    {
        return results.Where(r => r.IsFailure).Select(r => r.GetErrorOrThrow().Message).ToArray();
    }
}
