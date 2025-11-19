namespace Application.DTOs.Response;

public class ApiResult<T>
{
    public T? Data { get; set; }

    public ResponseMetadata? Metadata { get; set; }

    public static ApiResult<T> Success(T data, string message, int code)
    {
        return new ApiResult<T>
        {
            Data = data,
            Metadata = new ResponseMetadata(code, message, null),
        };
    }

    public static ApiResult<T> Success(string message, int code)
    {
        return new ApiResult<T> { Metadata = new ResponseMetadata(code, message, null) };
    }

    public static ApiResult<T> Error(string message, int code)
    {
        return new ApiResult<T> { Metadata = new ResponseMetadata(code, message, null) };
    }

    public static ApiResult<T> Error(string[] errors, int code)
    {
        return new ApiResult<T>
        {
            Metadata = new ResponseMetadata(code, string.Join(", ", errors), errors),
        };
    }
}

public sealed record ResponseMetadata(int? StatusCode, string? Message, string[]? Errors);
